namespace FinBineBackend.AdminDatabaseView.Models
{
    public class ListDbGroupsRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class ListDbGroupsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<DbGroupRow> Rows { get; set; } = new();
    }
}