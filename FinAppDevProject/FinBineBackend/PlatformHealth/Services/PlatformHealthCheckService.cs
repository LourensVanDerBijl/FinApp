using System.Diagnostics;
using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Npgsql;
using FinBineBackend.PlatformHealth.Models;
using FinBineBackend.PlatformHealth.Logs.Services;

namespace FinBineBackend.PlatformHealth.Services
{
    public class PlatformHealthCheckService
    {
        private const string CredentialsPath = "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json";
        private const int CriticalThresholdMs = 200;
        private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromMinutes(30);

        private readonly PlatformHealthLoggingService _healthLogger;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly FirestoreDb _firestoreDb;

        public PlatformHealthCheckService(
            PlatformHealthLoggingService healthLogger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _healthLogger = healthLogger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;

            // Same credential-loading approach as AdminAuthService.
            var credential = CredentialFactory.FromFile(CredentialsPath, "service_account");
            var projectId = ReadProjectIdFromCredentialsFile(CredentialsPath);

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                GoogleCredential = credential
            }.Build();
        }

        // Checks all four services. Called every 2 minutes by the
        // background worker, and also on-demand by the refresh button.
        public async Task RunHealthChecksAsync()
        {
            await CheckServiceAsync("C#", CheckBackendAsync);
            await CheckServiceAsync("PostgreSQL", CheckPostgreSqlAsync);
            await CheckServiceAsync("Firebase", CheckFirebaseAsync);
            await CheckServiceAsync("Netlify", CheckNetlifyAsync);
        }

        // Runs one service's check, compares the result to its previous
        // status, logs anything that actually changed, and updates the
        // shared in-memory store the dashboard reads from.
        private async Task CheckServiceAsync(string serviceName, Func<Task<int?>> checkFunc)
        {
            var current = PlatformHealthStore.GetOrCreate(serviceName);
            var isFirstCheck = current.LastChecked == default;
            var previousStatus = current.Status;

            int? latencyMs;
            try
            {
                latencyMs = await checkFunc();
            }
            catch (Exception ex)
            {
                _healthLogger.LogHealthCheckException(serviceName, ex);
                latencyMs = null;
            }

            string newStatus = latencyMs == null
                ? "Offline"
                : latencyMs > CriticalThresholdMs
                    ? "Critical"
                    : "Online";

            // Uptime clock: Online and Critical both count as "up" — only
            // a fully Offline result resets it.
            if (newStatus == "Offline")
            {
                current.OnlineSince = null;
                current.UptimeMinutes = 0;
            }
            else
            {
                current.OnlineSince ??= DateTime.UtcNow;
                current.UptimeMinutes = (int)(DateTime.UtcNow - current.OnlineSince.Value).TotalMinutes;
            }

            current.Status = newStatus;
            current.ResponseTimeMs = latencyMs;
            current.LastChecked = DateTime.UtcNow;

            // Only log when something actually changed.
            if (isFirstCheck)
            {
                _healthLogger.LogInitialStatus(serviceName, newStatus);
            }
            else if (newStatus != previousStatus)
            {
                if (newStatus == "Offline")
                    _healthLogger.LogServiceDown(serviceName);
                else if (newStatus == "Critical")
                    _healthLogger.LogServiceDegraded(serviceName, latencyMs ?? 0);
                else if (newStatus == "Online")
                    _healthLogger.LogServiceRecovered(serviceName);
            }

            // Heartbeat snapshot every 30 minutes, regardless of change.
            if (latencyMs != null &&
                (current.LastHeartbeatLoggedAt == null ||
                 DateTime.UtcNow - current.LastHeartbeatLoggedAt >= HeartbeatInterval))
            {
                _healthLogger.LogHeartbeat(serviceName, latencyMs.Value);
                current.LastHeartbeatLoggedAt = DateTime.UtcNow;
            }
        }

        // If this code is running at all, the C# backend is up by definition.
        private Task<int?> CheckBackendAsync()
        {
            return Task.FromResult<int?>(0);
        }

        private async Task<int?> CheckPostgreSqlAsync()
        {
            var connectionString = _configuration.GetConnectionString("FinBineDatabase");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                await using var command = new NpgsqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync();

                stopwatch.Stop();
                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch
            {
                return null; // Unreachable
            }
        }

        private async Task<int?> CheckFirebaseAsync()
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Smallest possible real read — just ask for one document.
                var query = _firestoreDb.Collection("fb_admin_users").Limit(1);
                await query.GetSnapshotAsync();

                stopwatch.Stop();
                return (int)stopwatch.ElapsedMilliseconds;
            }
            catch
            {
                return null;
            }
        }

        private async Task<int?> CheckNetlifyAsync()
        {
            var netlifyUrl = _configuration["PlatformHealth:NetlifyUrl"];

            if (string.IsNullOrEmpty(netlifyUrl))
                return null;

            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(10);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                var response = await client.GetAsync(netlifyUrl);
                stopwatch.Stop();

                return response.IsSuccessStatusCode ? (int)stopwatch.ElapsedMilliseconds : null;
            }
            catch
            {
                return null;
            }
        }

        private static string ReadProjectIdFromCredentialsFile(string credentialsPath)
        {
            using var stream = File.OpenRead(credentialsPath);
            using var document = JsonDocument.Parse(stream);

            return document.RootElement.GetProperty("project_id").GetString()
                ?? throw new InvalidOperationException(
                    "Could not find 'project_id' in the Firebase service account file.");
        }
    }
}