using Google.Cloud.Firestore;

namespace FinBineBackend.UserAccRegistration.Models
{
    // Shape of a document inside the "fb_users" Firestore collection.
    // This is what Admin's Groups page reads/displays — never the
    // financial data, which lives only in Postgres.
    [FirestoreData]
    public class FirestoreUserAccount
    {
        [FirestoreProperty("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [FirestoreProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [FirestoreProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [FirestoreProperty("account_email")]
        public string AccountEmail { get; set; } = string.Empty;

        [FirestoreProperty("firebase_uid")]
        public string FirebaseUid { get; set; } = string.Empty;

        [FirestoreProperty("joined_at")]
        public string JoinedAt { get; set; } = string.Empty;

        [FirestoreProperty("account_type")]
        public string AccountType { get; set; } = string.Empty;

        // Both null until the user creates or joins a group — the
        // frontend checks for this to decide whether to redirect them to
        // a "create or join a group" page after login.
        [FirestoreProperty("group_id")]
        public string? GroupId { get; set; } = null;

        [FirestoreProperty("group_name")]
        public string? GroupName { get; set; } = null;

        [FirestoreProperty("signInMethod")]
        public string SignInMethod { get; set; } = string.Empty;

        // Account standing — Active, Suspended, or Terminated. Not
        // related to group membership.
        [FirestoreProperty("member_status")]
        public string MemberStatus { get; set; } = "Active";

        [FirestoreProperty("is_owner")]
        public bool IsOwner { get; set; } = false;

        [FirestoreProperty("country")]
        public string Country { get; set; } = string.Empty;

        [FirestoreProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [FirestoreProperty("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [FirestoreProperty("last_activity")]
        public string? LastActivity { get; set; } = null;

        [FirestoreDocumentId]
        public string UserId { get; set; } = string.Empty;
    }
}