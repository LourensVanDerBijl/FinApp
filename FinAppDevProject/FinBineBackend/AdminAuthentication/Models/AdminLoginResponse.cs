namespace FinBineBackend.AdminAuthentication.Models
{
    public class AdminLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? PreferName { get; set; }
        public string? Surname { get; set; }
        public string? AccountType { get; set; }
    }
}