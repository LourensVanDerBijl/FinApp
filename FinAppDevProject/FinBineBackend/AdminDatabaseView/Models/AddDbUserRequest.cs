namespace FinBineBackend.AdminDatabaseView.Models
{
    // Manual/test-data entry, filled in by an admin on the Development
    // page — NOT the same flow as public self-service registration.
    // Still writes a real row to all 3 stores (a real Firebase Auth
    // account is required, since Firestore/Postgres both key off its
    // UID), but there's no temp-password email step — the admin sets
    // (or we generate) a password directly.
    public class AddDbUserRequest
    {
        public string Token { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        // Optional — if left blank, a random password is generated.
        // Either way it's returned once in the response so the admin
        // can hand it off, since it can't be retrieved again later.
        public string? Password { get; set; }

        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // "Free" or "Premium".
        public string AccountType { get; set; } = "Free";

        public string Country { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }
    }
}
