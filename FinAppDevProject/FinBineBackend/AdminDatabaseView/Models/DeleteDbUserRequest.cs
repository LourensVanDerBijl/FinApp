namespace FinBineBackend.AdminDatabaseView.Models
{
    // One click on a merged row should clear it out of all 3 stores.
    // Either identifier alone is enough — the service resolves the
    // other one where it can — because a row on-screen might only
    // exist in one or two of the three systems (an orphan record).
    public class DeleteDbUserRequest
    {
        public string Token { get; set; } = string.Empty;

        // fb_user_###### — matches both Firestore's doc ID and
        // Postgres's UserId.
        public string? UserId { get; set; }

        // Firebase Auth's own UID, used when there's no Firestore
        // document to look it up from.
        public string? FirebaseUid { get; set; }
    }
}
