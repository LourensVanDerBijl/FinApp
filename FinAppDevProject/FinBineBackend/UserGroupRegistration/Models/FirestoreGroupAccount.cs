using Google.Cloud.Firestore;

namespace FinBineBackend.UserGroupRegistration.Models
{
    // Shape of a document inside the "fb_groups" Firestore collection.
    [FirestoreData]
    public class FirestoreGroupAccount
    {
        [FirestoreProperty("group_name")]
        public string GroupName { get; set; } = string.Empty;

        // Points at a document in "fb_users" — the person who created
        // the group.
        [FirestoreProperty("owner_user_id")]
        public string OwnerUserId { get; set; } = string.Empty;

        // Copied from the owner's fb_users profile at creation time —
        // never entered directly for the group.
        [FirestoreProperty("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [FirestoreProperty("currency_code")]
        public string CurrencyCode { get; set; } = string.Empty;

        [FirestoreProperty("timezone")]
        public string TimeZone { get; set; } = string.Empty;

        // "Premium" or "Free".
        [FirestoreProperty("group_type")]
        public string GroupType { get; set; } = string.Empty;

        // Registration status of the group itself — always "Active" on
        // creation. Not shown to the user; a backend/admin-only concern,
        // same idea as MemberStatus on FirestoreUserAccount.
        [FirestoreProperty("status")]
        public string Status { get; set; } = "Active";

        // Real billing isn't built yet — always "Unpaid" for now, and
        // never shown to the user. Revisit once payments exist.
        [FirestoreProperty("payment_status")]
        public string PaymentStatus { get; set; } = "Unpaid";

        [FirestoreProperty("created_at")]
        public string CreatedAt { get; set; } = string.Empty;

        // Only set (to CreatedAt) when GroupType is "Premium" — null
        // for Free groups.
        [FirestoreProperty("subscription_start_date")]
        public string? SubscriptionStartDate { get; set; } = null;

        // Only ever set later, if/when a Premium subscription is
        // cancelled — always null at creation.
        [FirestoreProperty("subscription_end_date")]
        public string? SubscriptionEndDate { get; set; } = null;

        // Same as CreatedAt on creation; updated later whenever any
        // member of the group logs in.
        [FirestoreProperty("last_activity_at")]
        public string LastActivityAt { get; set; } = string.Empty;

        [FirestoreDocumentId]
        public string GroupId { get; set; } = string.Empty;
    }
}
