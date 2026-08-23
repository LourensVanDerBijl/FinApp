namespace FinBineBackend.AdminDatabaseView.Models
{
    // One merged row = one human, assembled from up to 3 different
    // sources. RowKey is whatever we could match the row on — usually
    // the fb_user_###### id, but falls back to the Firebase UID when a
    // Firestore/Postgres record doesn't exist (an orphaned Firebase
    // Auth account, e.g. after a partial registration failure).
    //
    // Each *Exists flag tells the frontend whether that block of fields
    // is real data or just empty defaults — this is what lets the UI
    // clearly show "this row has no Postgres record" instead of
    // silently rendering blank/zero values as if they were real.
    public class DbUserRow
    {
        public string RowKey { get; set; } = string.Empty;

        // ------------------------------------------------------------
        // Firebase Authentication
        // ------------------------------------------------------------
        public bool FirebaseAuthExists { get; set; }
        public string? FirebaseUid { get; set; }
        public string? FirebaseEmail { get; set; }
        public bool FirebaseEmailVerified { get; set; }
        public bool FirebaseDisabled { get; set; }
        public string? FirebaseCreatedAt { get; set; }
        public string? FirebaseLastSignIn { get; set; }

        // ------------------------------------------------------------
        // Firestore (fb_users)
        // ------------------------------------------------------------
        public bool FirestoreExists { get; set; }
        public string? FirestoreUserId { get; set; }
        public string? FirestoreDisplayName { get; set; }
        public string? FirestoreFirstName { get; set; }
        public string? FirestoreLastName { get; set; }
        public string? FirestoreAccountEmail { get; set; }
        public string? FirestoreAccountType { get; set; }
        public string? FirestoreMemberStatus { get; set; }
        public string? FirestoreGroupId { get; set; }
        public string? FirestoreGroupName { get; set; }
        public string? FirestoreCountry { get; set; }
        public string? FirestoreJoinedAt { get; set; }
        public string? FirestoreLastActivity { get; set; }

        // ------------------------------------------------------------
        // Postgres (Users table)
        // ------------------------------------------------------------
        public bool PostgresExists { get; set; }
        public string? PostgresUserId { get; set; }
        public string? PostgresPreferName { get; set; }
        public string? PostgresLastName { get; set; }
        public string? PostgresDateOfBirth { get; set; }
        public string? PostgresAccountType { get; set; }
        public string? PostgresGroupId { get; set; }
        public string? PostgresCreatedAt { get; set; }
    }
}
