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

        // Null while GroupStatus is None or Terminated. Points to a row
        // in the Groups table — the group's name itself isn't
        // duplicated here, only its ID. Kept in sync with the Firestore
        // fb_users document's group_id — see GroupMembershipStatus for
        // what each status means.
        public string? GroupId { get; set; } = null;

        public string GroupStatus { get; set; } = GroupMembershipStatus.None;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}