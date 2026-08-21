namespace FinBineBackend.UserGroupRegistration.Models
{
    public class CreateGroupResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
    }
}
