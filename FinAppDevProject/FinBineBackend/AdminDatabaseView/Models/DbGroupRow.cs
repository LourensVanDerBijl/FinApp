namespace FinBineBackend.AdminDatabaseView.Models
{
    public class DbGroupRow
    {
        public string RowKey { get; set; } = string.Empty;

        // Firestore (fb_groups)
        public bool FirestoreExists { get; set; }
        public string? FirestoreGroupId { get; set; }
        public string? FirestoreGroupName { get; set; }
        public string? FirestoreOwnerUserId { get; set; }
        public string? FirestoreCountryCode { get; set; }
        public string? FirestoreCurrencyCode { get; set; }
        public string? FirestoreTimeZone { get; set; }
        public string? FirestoreGroupType { get; set; }
        public string? FirestoreStatus { get; set; }
        public string? FirestorePaymentStatus { get; set; }
        public string? FirestoreCreatedAt { get; set; }
        public string? FirestoreSubscriptionStartDate { get; set; }
        public string? FirestoreSubscriptionEndDate { get; set; }
        public string? FirestoreLastActivityAt { get; set; }

        // Postgres (Groups table)
        public bool PostgresExists { get; set; }
        public string? PostgresGroupId { get; set; }
        public string? PostgresOwnerUserId { get; set; }
        public string? PostgresGroupType { get; set; }
        public string? PostgresCreatedAt { get; set; }
    }
}