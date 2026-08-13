namespace FinBineBackend.LoggingLogic.LogDbWrite.Models
{
    public class LogDbRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Which SAST calendar day this log belongs to. Used to reset the
        // live dashboard buffer at midnight, and later for cleaning up
        // logs older than 60 days.
        public DateOnly IngestionDate { get; set; }

        public string Type { get; set; } = string.Empty;

        public string Source { get; set; } = string.Empty;

        public string Level { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}