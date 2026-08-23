namespace FinBineBackend.AdminDatabaseView.Models
{
    public class ListDbUsersResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<DbUserRow> Rows { get; set; } = new();
    }
}
