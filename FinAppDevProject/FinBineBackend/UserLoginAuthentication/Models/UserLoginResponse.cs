namespace FinBineBackend.UserLoginAuthentication.Models
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public string? UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? AccountType { get; set; }

        // Both null means "no group yet" — the frontend uses this to
        // decide whether to redirect to a create/join-a-group page.
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
    }
}