using FinBineBackend.LoggingLogic.Logs.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FinBineBackend.LoggingLogic.LogDbWrite.Services
{
    public class LogDbBackgroundWorker : BackgroundService
    {
        private readonly IServiceProvider _services;

        public LogDbBackgroundWorker(IServiceProvider services)
        {
            _services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var dbWriter = scope.ServiceProvider.GetRequiredService<LogDbWriteService>();
                var systemLogger = scope.ServiceProvider.GetRequiredService<LoggingSystemLoggingService>();

                var logs = LogQueue.DrainArchive();

                try
                {
                    foreach (var log in logs)
                    {
                        await dbWriter.WriteLogAsync(log);
                    }
                }
                catch (Exception ex)
                {
                    // Previously: this would silently kill the worker
                    // forever. Now: it's logged, and the loop continues
                    // trying again on the next 10-minute cycle.
                    systemLogger.LogDatabaseSaveFailed(ex);
                }

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}