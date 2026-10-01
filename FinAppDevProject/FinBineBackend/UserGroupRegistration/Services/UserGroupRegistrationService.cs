using System.Linq;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FinBineBackend.UserGroupRegistration.Models;
using FinBineBackend.UserGroupRegistration.Data;
using FinBineBackend.UserGroupRegistration.Logs.Services;
using FinBineBackend.UserAccRegistration.Data;
using FinBineBackend.UserAccRegistration.Models;

namespace FinBineBackend.UserGroupRegistration.Services
{
    public class UserGroupRegistrationService
    {
        private const int MaxGroupNameLength = 20;
        private static readonly string[] AllowedGroupTypes = { "Premium", "Free" };

        private readonly GroupFirestoreService _groupFirestoreService;
        private readonly GroupDbContext _groupDb;
        private readonly UserDbContext _userDb;
        private readonly UserGroupRegistrationLoggingService _regLogger;

        public UserGroupRegistrationService(
            GroupFirestoreService groupFirestoreService,
            GroupDbContext groupDb,
            UserDbContext userDb,
            UserGroupRegistrationLoggingService regLogger)
        {
            _groupFirestoreService = groupFirestoreService;
            _groupDb = groupDb;
            _userDb = userDb;
            _regLogger = regLogger;
        }

        public async Task<CreateGroupResponse> CreateGroupAsync(CreateGroupRequest request, string ipAddress)
        {
            FirebaseToken decodedToken;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.Token);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    _regLogger.LogTokenMismatch(ipAddress);
                }

                return new CreateGroupResponse
                {
                    Success = false,
                    Message = "Your session could not be verified. Please sign in again."
                };
            }

            string groupName = request.GroupName?.Trim() ?? string.Empty;
            string requestedType = request.GroupType?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(groupName))
            {
                _regLogger.LogRegistrationRejected("Validation", decodedToken.Uid, "Missing group name.");
                return new CreateGroupResponse { Success = false, Message = "Enter a name for your group." };
            }

            if (groupName.Length > MaxGroupNameLength)
            {
                _regLogger.LogRegistrationRejected("Validation", decodedToken.Uid, "Group name too long.");
                return new CreateGroupResponse
                {
                    Success = false,
                    Message = $"Group name must be {MaxGroupNameLength} characters or fewer."
                };
            }

            if (!AllowedGroupTypes.Contains(requestedType, StringComparer.OrdinalIgnoreCase))
            {
                _regLogger.LogRegistrationRejected("Validation", decodedToken.Uid, $"Invalid group type: {request.GroupType}");
                return new CreateGroupResponse { Success = false, Message = "Select a valid group type." };
            }

            string groupType = string.Equals(requestedType, "Premium", StringComparison.OrdinalIgnoreCase)
                ? "Premium"
                : "Free";

            var owner = await _groupFirestoreService.FindOwnerByFirebaseUidAsync(decodedToken.Uid);
            if (owner == null)
            {
                _regLogger.LogRegistrationRejected("Firestore", decodedToken.Uid, "No matching fb_users profile found.");
                return new CreateGroupResponse
                {
                    Success = false,
                    Message = "We couldn't find a FinBine profile for this account."
                };
            }

            if (!string.IsNullOrEmpty(owner.GroupId))
            {
                _regLogger.LogRegistrationRejected("Validation", owner.UserId, "Already belongs to a group.");
                return new CreateGroupResponse { Success = false, Message = "You're already part of a group." };
            }

            // Captured before any writes happen, so rollback can restore
            // exactly what the owner had before — not a hardcoded guess.
            string previousAccountType = owner.AccountType;
            string previousGroupStatus = string.IsNullOrWhiteSpace(owner.GroupStatus)
                ? GroupMembershipStatus.None
                : owner.GroupStatus;

            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string? groupId = null;
            string failedAtSource = "Firestore";

            try
            {
                failedAtSource = "Firestore (fb_groups)";
                Console.WriteLine("[GroupRegistration] Generating group ID...");
                groupId = await _groupFirestoreService.GenerateNextGroupIdAsync();
                Console.WriteLine("[GroupRegistration] Group ID: " + groupId);

                string nowIso = DateTime.UtcNow.ToString("o");

                var groupAccount = new FirestoreGroupAccount
                {
                    GroupName = groupName,
                    OwnerUserId = owner.UserId,
                    CountryCode = owner.Country,
                    CurrencyCode = owner.Currency,
                    TimeZone = owner.Timezone,
                    GroupType = groupType,
                    Status = "Active",
                    PaymentStatus = "Unpaid",
                    CreatedAt = nowIso,
                    SubscriptionStartDate = groupType == "Premium" ? nowIso : null,
                    SubscriptionEndDate = null,
                    LastActivityAt = nowIso
                };

                Console.WriteLine("[GroupRegistration] Creating fb_groups document...");
                await _groupFirestoreService.CreateGroupDocumentAsync(groupId, groupAccount);
                Console.WriteLine("[GroupRegistration] fb_groups document created OK");
                rollbackActions.Add(("Firestore (fb_groups)", async () => await _groupFirestoreService.DeleteGroupDocumentAsync(groupId)));

                failedAtSource = "Firestore (fb_users)";
                Console.WriteLine("[GroupRegistration] Assigning owner in fb_users (account_type -> " + groupType + ", group_status -> Active)...");
                await _groupFirestoreService.AssignOwnerToGroupAsync(owner.UserId, groupId, groupName, groupType);
                Console.WriteLine("[GroupRegistration] fb_users owner assignment OK");
                rollbackActions.Add(("Firestore (fb_users)", async () => await _groupFirestoreService.RevertOwnerGroupAssignmentAsync(owner.UserId, previousAccountType, previousGroupStatus)));

                failedAtSource = "PostgreSQL (Groups)";
                Console.WriteLine("[GroupRegistration] Saving Postgres Groups row...");
                await SavePostgresGroupRecordAsync(groupId, owner.UserId, groupType);
                Console.WriteLine("[GroupRegistration] Postgres Groups row saved OK");
                rollbackActions.Add(("PostgreSQL (Groups)", async () => await DeletePostgresGroupRecordAsync(groupId)));

                failedAtSource = "PostgreSQL (Users)";
                Console.WriteLine("[GroupRegistration] Updating Postgres Users (GroupId + AccountType -> " + groupType + ")...");
                await UpdatePostgresUserGroupAsync(owner.UserId, groupId, groupType);
                Console.WriteLine("[GroupRegistration] Postgres Users row updated OK");

                return new CreateGroupResponse
                {
                    Success = true,
                    Message = "Group created successfully.",
                    GroupId = groupId,
                    GroupName = groupName
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("========== GROUP CREATION FAILED ==========");
                Console.WriteLine("Failed at: " + failedAtSource);
                Console.WriteLine(ex.ToString());
                Console.WriteLine("=============================================");

                _regLogger.LogRollbackTriggered(failedAtSource, owner.UserId, groupId, ex.Message);
                await RollbackAsync(rollbackActions, owner.UserId, groupId);

                return new CreateGroupResponse
                {
                    Success = false,
                    Message = "Group creation failed. Please try again."
                };
            }
        }

        // Statuses a user may request to join FROM. Only Active is
        // excluded — you can't submit a new request while already a
        // confirmed member somewhere (that requires leaving the group
        // first, not built yet). Pending and Suspended are both
        // allowed: submitting a new request abandons whatever the old
        // one pointed at — a stale pending request is simply replaced,
        // and a suspended user's old group owner loses the ability to
        // reinstate them once this succeeds. Same overwrite, same
        // SubmitJoinRequestAsync call either way.
        private static readonly string[] JoinableFromStatuses =
        {
            GroupMembershipStatus.None,
            GroupMembershipStatus.Pending,
            GroupMembershipStatus.Suspended,
            GroupMembershipStatus.Terminated
        };

        public async Task<JoinGroupResponse> RequestToJoinGroupAsync(JoinGroupRequest request, string ipAddress)
        {
            FirebaseToken decodedToken;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.Token);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    _regLogger.LogTokenMismatch(ipAddress);
                }

                return new JoinGroupResponse
                {
                    Success = false,
                    Message = "Your session could not be verified. Please sign in again."
                };
            }

            string targetGroupId = request.GroupId?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(targetGroupId))
            {
                _regLogger.LogRegistrationRejected("Validation", decodedToken.Uid, "Missing group ID.");
                return new JoinGroupResponse { Success = false, Message = "Enter a Group ID." };
            }

            // Reused from CreateGroupAsync — the name predates this call
            // site, but it's just "find the fb_users doc for this
            // Firebase UID", which applies to a requester exactly the
            // same as it does an owner-to-be.
            var requester = await _groupFirestoreService.FindOwnerByFirebaseUidAsync(decodedToken.Uid);
            if (requester == null)
            {
                _regLogger.LogRegistrationRejected("Firestore", decodedToken.Uid, "No matching fb_users profile found.");
                return new JoinGroupResponse
                {
                    Success = false,
                    Message = "We couldn't find a FinBine profile for this account."
                };
            }

            string requesterStatus = string.IsNullOrWhiteSpace(requester.GroupStatus)
                ? GroupMembershipStatus.None
                : requester.GroupStatus;

            if (!JoinableFromStatuses.Contains(requesterStatus, StringComparer.OrdinalIgnoreCase))
            {
                _regLogger.LogRegistrationRejected("Validation", requester.UserId, $"Cannot request to join while GroupStatus is {requesterStatus}.");
                return new JoinGroupResponse
                {
                    Success = false,
                    Message = "You're already part of a group."
                };
            }

            var targetGroup = await _groupFirestoreService.FindGroupByIdAsync(targetGroupId);
            if (targetGroup == null)
            {
                _regLogger.LogRegistrationRejected("Validation", requester.UserId, $"Group '{targetGroupId}' not found.");
                return new JoinGroupResponse { Success = false, Message = "Group not found. Check the Group ID and try again." };
            }

            if (!string.Equals(targetGroup.Status, "Active", StringComparison.OrdinalIgnoreCase))
            {
                _regLogger.LogRegistrationRejected("Validation", requester.UserId, $"Group '{targetGroupId}' is not active.");
                return new JoinGroupResponse { Success = false, Message = "This group can't accept new members right now." };
            }

            // Captured before any writes happen, so rollback can restore
            // exactly what the requester had before — same reasoning as
            // previousAccountType in CreateGroupAsync. In the common
            // case this is null/null/None; for a Suspended user
            // switching groups it's their old group's id/name/Suspended.
            string? previousGroupId = requester.GroupId;
            string? previousGroupName = requester.GroupName;
            string previousGroupStatus = requesterStatus;

            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string failedAtSource = "Firestore (fb_users)";

            try
            {
                await _groupFirestoreService.SubmitJoinRequestAsync(requester.UserId, targetGroupId, targetGroup.GroupName);
                rollbackActions.Add(("Firestore (fb_users)", async () =>
                    await _groupFirestoreService.RevertJoinRequestAsync(requester.UserId, previousGroupId, previousGroupName, previousGroupStatus)));

                failedAtSource = "PostgreSQL (Users)";
                await UpdatePostgresUserJoinRequestAsync(requester.UserId, targetGroupId);

                return new JoinGroupResponse
                {
                    Success = true,
                    Message = "Your request to join has been sent to the group owner.",
                    GroupId = targetGroupId,
                    GroupName = targetGroup.GroupName,
                    GroupStatus = GroupMembershipStatus.Pending
                };
            }
            catch (Exception ex)
            {
                _regLogger.LogRollbackTriggered(failedAtSource, requester.UserId, targetGroupId, ex.Message);
                await RollbackAsync(rollbackActions, requester.UserId, targetGroupId);

                return new JoinGroupResponse
                {
                    Success = false,
                    Message = "We couldn't submit your request. Please try again."
                };
            }
        }

        // Stamps the requester's Postgres Users row with the target
        // GroupId and moves GroupStatus to Pending. Deliberately does
        // NOT touch AccountType — unlike group creation, membership
        // isn't confirmed yet, so the account tier doesn't change until
        // an owner approves (not built yet).
        private async Task UpdatePostgresUserJoinRequestAsync(string userId, string groupId)
        {
            var userRecord = await _userDb.Users.FindAsync(userId);
            if (userRecord == null)
            {
                throw new InvalidOperationException($"No Postgres Users row found for UserId '{userId}'.");
            }

            userRecord.GroupId = groupId;
            userRecord.GroupStatus = GroupMembershipStatus.Pending;
            await _userDb.SaveChangesAsync();
        }

        private async Task SavePostgresGroupRecordAsync(string groupId, string ownerUserId, string groupType)
        {
            var groupDbRecord = new GroupDbRecord
            {
                GroupId = groupId,
                OwnerUserId = ownerUserId,
                GroupType = groupType
            };

            _groupDb.Groups.Add(groupDbRecord);
            await _groupDb.SaveChangesAsync();
        }

        private async Task DeletePostgresGroupRecordAsync(string groupId)
        {
            var record = await _groupDb.Groups.FindAsync(groupId);
            if (record != null)
            {
                _groupDb.Groups.Remove(record);
                await _groupDb.SaveChangesAsync();
            }
        }

        // Stamps the owner's Postgres Users row with the new GroupId,
        // upgrades AccountType to match the group's type, and moves
        // GroupStatus to Active — mirrors what AssignOwnerToGroupAsync
        // does on the Firestore side. Membership in a Premium group
        // always means a Premium account, and an owner is a confirmed
        // member of their own group from the moment it's created (no
        // approval step, unlike joining).
        private async Task UpdatePostgresUserGroupAsync(string userId, string groupId, string groupType)
        {
            var userRecord = await _userDb.Users.FindAsync(userId);
            if (userRecord == null)
            {
                throw new InvalidOperationException($"No Postgres Users row found for UserId '{userId}'.");
            }

            userRecord.GroupId = groupId;
            userRecord.AccountType = groupType;
            userRecord.GroupStatus = GroupMembershipStatus.Active;
            await _userDb.SaveChangesAsync();
        }

        private async Task RollbackAsync(List<(string Source, Func<Task> Action)> rollbackActions, string ownerUserId, string? groupId)
        {
            for (int i = rollbackActions.Count - 1; i >= 0; i--)
            {
                var (source, action) = rollbackActions[i];
                try
                {
                    await action();
                }
                catch (Exception rollbackEx)
                {
                    _regLogger.LogRollbackFailed(source, ownerUserId, groupId, rollbackEx.Message);
                }
            }
        }
    }
}