namespace FinBineBackend.UserAccRegistration.Models
{
    // The minimal Postgres anchor row for a user. This is intentionally
    // small — richer profile data (display name, sign-in method, country,
    // etc.) lives in Firestore's fb_users collection instead. This table
    // exists so FinBine's actual financial features have a real row to
    // attach to later.
    public class UserDbRecord
    {
        // Matches the Firestore document ID (e.g. "fb_user_000001") —
        // NOT the Firebase UID. Keeps the two systems linked by the same
        // human-readable ID, same idea as fb_admin_users.
        public string UserId { get; set; } = string.Empty;

        public string PreferName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        // "Free" or "Premium" for now.
        public string AccountType { get; set; } = string.Empty;

        // Null until the user creates or joins a group. Points to a row
        // in a future Groups table — the group's name itself isn't
        // duplicated here, only its ID.
        public string? GroupId { get; set; } = null;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}