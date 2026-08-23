using Google.Cloud.Firestore;

namespace FinBineBackend.UserAccRegistration.Models
{
    // Shape of a document inside the "fb_users" Firestore collection.
    // This is what Admin's Groups page reads/displays — never the
    // financial data, which lives only in Postgres.
    [FirestoreData]
    public class FirestoreUserAccount
    {
        [FirestoreProperty("display_name")]
        public string DisplayName { get; set; } = string.Empty;

        [FirestoreProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [FirestoreProperty("last_name")]
        public string LastName { get; set; } = string.Empty;

        [FirestoreProperty("account_email")]
        public string AccountEmail { get; set; } = string.Empty;

        [FirestoreProperty("firebase_uid")]
        public string FirebaseUid { get; set; } = string.Empty;

        [FirestoreProperty("joined_at")]
        public string JoinedAt { get; set; } = string.Empty;

        [FirestoreProperty("account_type")]
        public string AccountType { get; set; } = string.Empty;

        // Both null while GroupStatus is None or Terminated. Set to the
        // target group as soon as a join request is submitted (while
        // Pending) — the frontend used to check GroupId alone to decide
        // whether to redirect to "create or join a group"; it now reads
        // GroupStatus instead, since GroupId can be non-null while
        // Pending or Suspended too.
        [FirestoreProperty("group_id")]
        public string? GroupId { get; set; } = null;

        [FirestoreProperty("group_name")]
        public string? GroupName { get; set; } = null;

        // One of GroupMembershipStatus.{None,Pending,Active,Suspended,Terminated}.
        // None: no group, GroupId null. Pending: join request submitted,
        // GroupId points at the target group, awaiting owner approval.
        // Active: normal member. Suspended: owner suspended this member
        // — GroupId still points at that group so the owner can reinstate
        // them, but login no longer sends them to the dashboard. A user
        // who is Pending or Suspended can submit a new join request for a
        // different group at any time — that overwrites GroupId/GroupName
        // and resets GroupStatus to Pending on the new target, abandoning
        // whatever the old one pointed at (a stale request, or a
        // suspended membership the old owner can no longer reinstate).
        // Terminated: removed from the group entirely (either
        // by the 2-month auto-sweep on Suspended, not yet built, or some
        // other removal), GroupId null again, same routing as None.
        [FirestoreProperty("group_status")]
        public string GroupStatus { get; set; } = GroupMembershipStatus.None;

        [FirestoreProperty("signInMethod")]
        public string SignInMethod { get; set; } = string.Empty;

        // Account standing — Active, Suspended, or Terminated. Not
        // related to group membership.
        [FirestoreProperty("member_status")]
        public string MemberStatus { get; set; } = "Active";

        [FirestoreProperty("is_owner")]
        public bool IsOwner { get; set; } = false;

        [FirestoreProperty("country")]
        public string Country { get; set; } = string.Empty;

        [FirestoreProperty("currency")]
        public string Currency { get; set; } = string.Empty;

        [FirestoreProperty("timezone")]
        public string Timezone { get; set; } = string.Empty;

        [FirestoreProperty("last_activity")]
        public string? LastActivity { get; set; } = null;

        [FirestoreDocumentId]
        public string UserId { get; set; } = string.Empty;
    }
}