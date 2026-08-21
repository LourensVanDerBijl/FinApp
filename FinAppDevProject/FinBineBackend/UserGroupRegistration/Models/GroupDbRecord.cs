namespace FinBineBackend.UserGroupRegistration.Models
{
    // The minimal Postgres anchor row for a group. Richer data (name,
    // type, dates, status, etc.) lives in Firestore's fb_groups
    // collection instead — this table exists purely so FinBine's actual
    // financial features have a real row to attach a GroupId to later.
    // Same philosophy as UserDbRecord.
    public class GroupDbRecord
    {
        // Matches the Firestore document ID (e.g. "fb_group_000001") —
        // same idea as UserDbRecord.UserId linking to fb_users.
        public string GroupId { get; set; } = string.Empty;

        // Matches a UserDbRecord/fb_users UserId (e.g. "fb_user_000001").
        public string OwnerUserId { get; set; } = string.Empty;

        // "Free" or "Premium" — kept in sync with the Firestore doc.
        public string GroupType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
