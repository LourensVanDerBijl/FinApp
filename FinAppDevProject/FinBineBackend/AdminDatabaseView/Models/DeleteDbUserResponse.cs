namespace FinBineBackend.AdminDatabaseView.Models
{
    // Deletion is best-effort per store, not all-or-nothing — unlike
    // registration's rollback, there's nothing to "undo" a delete
    // into. Reporting each store separately lets the admin see exactly
    // which of the 3 actually had a record to remove.
    public class DeleteDbUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public bool FirebaseAuthDeleted { get; set; }
        public bool FirestoreDeleted { get; set; }
        public bool PostgresDeleted { get; set; }
    }
}
