namespace FinBineBackend.AdminDatabaseView.Models
{
    public class AddDbUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }

        // Only populated when we generated the password ourselves
        // (admin left the field blank) — shown once, never stored.
        public string? GeneratedPassword { get; set; }
    }
}
