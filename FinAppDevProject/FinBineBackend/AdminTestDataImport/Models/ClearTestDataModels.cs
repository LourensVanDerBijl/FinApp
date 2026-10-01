namespace FinBineBackend.AdminTestDataImport.Models
{
    public class ClearTestDataRequest
    {
        public string Token { get; set; } = string.Empty;

        // The frontend always sends this pre-filled with the protected
        // email — but the backend does NOT trust it alone. See
        // AdminTestDataImportService.AlwaysProtectedEmails: whatever is
        // sent here is combined with that hardcoded list, never used to
        // replace it. A blank/wrong value here can only over-protect,
        // never under-protect.
        public string ExcludeEmail { get; set; } = string.Empty;
    }

    public class ClearTestDataResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public int FirebaseAuthDeleted { get; set; }
        public int FirestoreDeleted { get; set; }
        public int PostgresDeleted { get; set; }
        public int FirestoreGroupsDeleted { get; set; }
        public int PostgresGroupsDeleted { get; set; }

        // Every email that was protected from deletion this run.
        public List<string> ProtectedEmails { get; set; } = new();

        // Non-fatal per-row failures — the run continues past these
        // (best-effort, not all-or-nothing, since a clear-all is
        // already a "start fresh" operation).
        public List<string> Errors { get; set; } = new();
    }
}