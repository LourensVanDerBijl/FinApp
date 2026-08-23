namespace FinBineBackend.UserGroupRegistration.Models
{
    public class JoinGroupRequest
    {
        // The requester's current Firebase ID token — same reasoning as
        // CreateGroupRequest.Token: re-verified here regardless of any
        // frontend route guard.
        public string Token { get; set; } = string.Empty;

        // The full Firestore document ID of the group being requested,
        // e.g. "fb_group_000006" — not a separate short invite code.
        public string GroupId { get; set; } = string.Empty;
    }
}
