using FinBineBackend.AdminAuthentication.Services;
using FinBineBackend.AdminGroupView.Logs.Services;
using FinBineBackend.AdminGroupView.Models;
using FinBineBackend.UserAccRegistration.Models;
using FinBineBackend.UserAccRegistration.Services;
using FinBineBackend.UserGroupRegistration.Models;
using FinBineBackend.UserGroupRegistration.Services;

namespace FinBineBackend.AdminGroupView.Services
{
    // Powers the Admin > Groups page. Read-only — unlike AdminDatabaseView,
    // there's no add/delete here, just a merged, nested view: every group
    // from "fb_groups", with its owner and members attached from
    // "fb_users". Firestore is the only source used (see the comment on
    // FirestoreUserAccount: it's explicitly what the Groups page reads),
    // not Postgres — the Postgres Groups/Users tables only carry the
    // minimal anchor rows, none of the display fields this page needs.
    public class AdminGroupViewService
    {
        private readonly AdminAuthService _adminAuthService;
        private readonly GroupFirestoreService _groupFirestoreService;
        private readonly UserFirestoreService _userFirestoreService;
        private readonly AdminGroupViewLoggingService _logger;

        public AdminGroupViewService(
            AdminAuthService adminAuthService,
            GroupFirestoreService groupFirestoreService,
            UserFirestoreService userFirestoreService,
            AdminGroupViewLoggingService logger)
        {
            _adminAuthService = adminAuthService;
            _groupFirestoreService = groupFirestoreService;
            _userFirestoreService = userFirestoreService;
            _logger = logger;
        }

        public async Task<ListGroupsResponse> ListGroupsAsync(string token, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new ListGroupsResponse { Success = false, Message = adminCheck.Message };
            }

            try
            {
                // Pulled independently, then joined in memory — same
                // approach AdminDatabaseViewService uses, since there's
                // no single query that returns a group with its members
                // already attached.
                List<FirestoreGroupAccount> allGroups = await _groupFirestoreService.GetAllGroupsAsync();
                List<FirestoreUserAccount> allUsers = await _userFirestoreService.GetAllUsersAsync();

                // Every user currently pointing at a given GroupId —
                // this is "members", regardless of their status within
                // that group (pending/active/suspended all included;
                // the frontend already buckets by status itself).
                var usersByGroupId = allUsers
                    .Where(u => !string.IsNullOrEmpty(u.GroupId))
                    .GroupBy(u => u.GroupId!)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var rows = allGroups.Select(group =>
                {
                    usersByGroupId.TryGetValue(group.GroupId, out var members);
                    members ??= new List<FirestoreUserAccount>();

                    var ownerAccount = members.FirstOrDefault(m => m.IsOwner)
                        ?? allUsers.FirstOrDefault(u => u.UserId == group.OwnerUserId);

                    return new GroupViewRow
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                        AccountType = group.GroupType,
                        GroupStatus = group.Status,
                        CreatedAt = group.CreatedAt,
                        LastActivity = group.LastActivityAt,
                        GroupCurrency = group.CurrencyCode,
                        GroupCountry = group.CountryCode,
                        GroupTimezone = group.TimeZone,
                        Subscription = new GroupViewSubscription
                        {
                            Status = group.GroupType,
                            MembershipPaid = string.Equals(group.PaymentStatus, "Paid", StringComparison.OrdinalIgnoreCase),
                            StartDate = group.SubscriptionStartDate,
                            EndDate = group.SubscriptionEndDate,
                            PaymentStatus = group.PaymentStatus
                        },
                        Owner = ownerAccount == null ? null : ToMember(ownerAccount),
                        Members = members.Select(ToMember).ToList()
                    };
                })
                .OrderBy(r => r.GroupId)
                .ToList();

                return new ListGroupsResponse
                {
                    Success = true,
                    Message = $"{rows.Count} group(s) found.",
                    Groups = rows
                };
            }
            catch (Exception ex)
            {
                _logger.LogSystemError(ex.Message);

                return new ListGroupsResponse
                {
                    Success = false,
                    Message = "Failed to load groups. Please try again."
                };
            }
        }

        // Powers the Dashboard's "Total Groups" / "Total Users" stat
        // cards and the subscription donut. Same admin check, same two
        // Firestore reads as ListGroupsAsync — but returns just counts,
        // not every group's full nested member list, since the
        // dashboard doesn't need that much payload just to show four
        // numbers.
        public async Task<GroupSummaryResponse> GetSummaryAsync(string token, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new GroupSummaryResponse { Success = false, Message = adminCheck.Message };
            }

            try
            {
                List<FirestoreGroupAccount> allGroups = await _groupFirestoreService.GetAllGroupsAsync();
                List<FirestoreUserAccount> allUsers = await _userFirestoreService.GetAllUsersAsync();

                int premiumGroups = allGroups.Count(g => string.Equals(g.GroupType, "Premium", StringComparison.OrdinalIgnoreCase));
                int freeGroups = allGroups.Count(g => string.Equals(g.GroupType, "Free", StringComparison.OrdinalIgnoreCase));

                return new GroupSummaryResponse
                {
                    Success = true,
                    Message = "Summary loaded.",
                    TotalGroups = allGroups.Count,
                    PremiumGroups = premiumGroups,
                    FreeGroups = freeGroups,
                    TotalUsers = allUsers.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogSystemError(ex.Message);

                return new GroupSummaryResponse
                {
                    Success = false,
                    Message = "Failed to load group summary. Please try again."
                };
            }
        }

        // FirestoreUserAccount.GroupStatus (None/Pending/Active/Suspended/
        // Terminated) is what the frontend calls memberStatus, lowercased
        // to match its existing badge/CSS-class logic. Deliberately NOT
        // FirestoreUserAccount.MemberStatus — that field means something
        // else entirely (overall account standing, not group standing).
        private static GroupViewMember ToMember(FirestoreUserAccount user)
        {
            return new GroupViewMember
            {
                UserId = user.UserId,
                PreferredName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.AccountEmail,
                SignInMethod = user.SignInMethod,
                MemberStatus = user.GroupStatus.ToLowerInvariant(),
                IsOwner = user.IsOwner,
                Country = user.Country,
                Currency = user.Currency,
                Timezone = user.Timezone,
                JoinedAt = string.IsNullOrEmpty(user.JoinedAt) ? null : user.JoinedAt,
                LastActivity = user.LastActivity
            };
        }
    }
}
