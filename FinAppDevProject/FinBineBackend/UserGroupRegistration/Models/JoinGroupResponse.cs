namespace FinBineBackend.UserGroupRegistration.Models
{
    public class JoinGroupResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // Populated on success so the frontend can show the "waiting for
        // approval" state immediately without a second round trip.
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public string? GroupStatus { get; set; }
    }
}
