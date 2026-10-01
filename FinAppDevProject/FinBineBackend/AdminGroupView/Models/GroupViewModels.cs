namespace FinBineBackend.AdminGroupView.Models
{
    public class ListGroupsRequest
    {
        // Same convention as every other admin endpoint — the caller's
        // current Firebase ID token, re-verified on every call.
        public string Token { get; set; } = string.Empty;
    }

    public class ListGroupsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public List<GroupViewRow> Groups { get; set; } = new();
    }

    // Powers the Dashboard's stat cards + subscription donut. Deliberately
    // separate from ListGroupsResponse — the dashboard only needs a
    // handful of numbers, not every group's full member list, so it gets
    // its own small response shape rather than forcing the frontend to
    // fetch and count the full /list payload on every dashboard visit.
    public class GroupSummaryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public int TotalGroups { get; set; }
        public int PremiumGroups { get; set; }
        public int FreeGroups { get; set; }

        // Every fb_users document, regardless of group membership status
        // — matches "Total Users" on the Dashboard, not just group members.
        public int TotalUsers { get; set; }
    }

    // One group, shaped for the Admin Groups page — mirrors the fields
    // the frontend's mockData.js "groups" array already used, so the
    // page itself needs the smallest possible change to switch over
    // from mock data to this.
    public class GroupViewRow
    {
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        // "Premium" or "Free" — from FirestoreGroupAccount.GroupType.
        public string AccountType { get; set; } = string.Empty;

        // From FirestoreGroupAccount.Status. Currently always "Active"
        // — there's no suspend/terminate/pending-approval write-path
        // for a group yet, so don't expect to see anything else here
        // until that's built elsewhere.
        public string GroupStatus { get; set; } = string.Empty;

        public string CreatedAt { get; set; } = string.Empty;
        public string LastActivity { get; set; } = string.Empty;

        public string GroupCurrency { get; set; } = string.Empty;
        public string GroupCountry { get; set; } = string.Empty;
        public string GroupTimezone { get; set; } = string.Empty;

        public GroupViewSubscription Subscription { get; set; } = new();

        public GroupViewMember? Owner { get; set; }

        public List<GroupViewMember> Members { get; set; } = new();
    }

    public class GroupViewSubscription
    {
        // Mirrors AccountType — "Premium" or "Free".
        public string Status { get; set; } = string.Empty;

        public bool MembershipPaid { get; set; }

        public string? StartDate { get; set; }
        public string? EndDate { get; set; }

        // From FirestoreGroupAccount.PaymentStatus. Real billing isn't
        // built yet, so this is currently always "Unpaid".
        public string PaymentStatus { get; set; } = string.Empty;
    }

    // One member (or the owner) of a group, from a "fb_users" document.
    public class GroupViewMember
    {
        public string UserId { get; set; } = string.Empty;
        public string PreferredName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string SignInMethod { get; set; } = string.Empty;

        // Lowercased on purpose ("pending"/"active"/"suspended") to
        // match the frontend's existing status-badge/CSS-class logic.
        // This is FirestoreUserAccount.GroupStatus — the member's
        // standing WITHIN this group — not MemberStatus, which is a
        // different field (overall account standing, unrelated to
        // group membership). Easy to mix up; see the AdminGroupView
        // README for more on this.
        public string MemberStatus { get; set; } = string.Empty;

        public bool IsOwner { get; set; }

        public string Country { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;

        public string? JoinedAt { get; set; }
        public string? LastActivity { get; set; }
    }
}
