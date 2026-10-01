namespace FinBineBackend.AdminTestDataImport.Models
{
    public class ImportTestDataRequest
    {
        public string Token { get; set; } = string.Empty;
        public TestDataPayload Payload { get; set; } = new();
    }

    public class ImportTestDataResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // Populated on a validation failure — every problem found, not
        // just the first, so a bad file can be fixed in one pass
        // instead of being resubmitted error-by-error.
        public List<string> ValidationErrors { get; set; } = new();

        public int GroupsCreated { get; set; }
        public int UsersCreated { get; set; }
    }
}
