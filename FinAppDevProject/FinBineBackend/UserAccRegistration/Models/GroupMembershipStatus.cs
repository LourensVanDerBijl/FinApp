namespace FinBineBackend.UserAccRegistration.Models
{
    // The five states a user can be in with respect to group membership.
    // Lives alongside GroupId/GroupName on both FirestoreUserAccount and
    // UserDbRecord — see the comments on those fields for what each
    // value means and how login routing uses it.
    public static class GroupMembershipStatus
    {
        public const string None = "None";
        public const string Pending = "Pending";
        public const string Active = "Active";
        public const string Suspended = "Suspended";
        public const string Terminated = "Terminated";
    }
}
