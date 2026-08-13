namespace FinBineBackend.PlatformHealth.Models
{
    // Holds the most recent health check result for one service.
    // Kept in memory (see PlatformHealthStore) — not saved to the database.
    public class ServiceHealthStatus
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline"; // Online, Critical, or Offline
        public int? ResponseTimeMs { get; set; }
        public int UptimeMinutes { get; set; } = 0;

        // When this service last transitioned to "up" (Online or Critical).
        // Null means it's currently Offline.
        public DateTime? OnlineSince { get; set; }

        public DateTime LastChecked { get; set; }
        public DateTime? LastHeartbeatLoggedAt { get; set; }
    }
}