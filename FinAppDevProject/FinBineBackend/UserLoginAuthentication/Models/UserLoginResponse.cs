namespace FinBineBackend.UserLoginAuthentication.Models
{
    public class UserLoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public string? UserId { get; set; }
        public string? DisplayName { get; set; }
        public string? AccountType { get; set; }

        // GroupId/GroupName are null only while GroupStatus is None or
        // Terminated. The frontend now routes on GroupStatus (not just
        // whether GroupId is null) — see GroupMembershipStatus for what
        // each value means.
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupStatus { get; set; }
    }
}