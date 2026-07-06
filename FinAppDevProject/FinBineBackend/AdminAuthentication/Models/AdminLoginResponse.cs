namespace FinBineBackend.AdminAuthentication.Models
{
    public class AdminLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AdminId { get; set; }
        public string? Email { get; set; }
    }
}
