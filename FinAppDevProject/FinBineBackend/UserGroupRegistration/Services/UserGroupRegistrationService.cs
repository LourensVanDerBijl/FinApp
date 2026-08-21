using System.Linq;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FinBineBackend.UserGroupRegistration.Models;
using FinBineBackend.UserGroupRegistration.Data;
using FinBineBackend.UserGroupRegistration.Logs.Services;

namespace FinBineBackend.UserGroupRegistration.Services
{
    public class UserGroupRegistrationService
    {
        private const int MaxGroupNameLength = 20;
        private static readonly string[] AllowedGroupTypes = { "Premium", "Free" };

        private readonly GroupFirestoreService _groupFirestoreService;
        private readonly GroupDbContext _groupDb;
        private readonly UserGroupRegistrationLoggingService _regLogger;

        public UserGroupRegistrationService(
            GroupFirestoreService groupFirestoreService,
            GroupDbContext groupDb,
            UserGroupRegistrationLoggingService regLogger)
        {
            _groupFirestoreService = groupFirestoreService;
            _groupDb = groupDb;
            _regLogger = regLogger;
        }

        public async Task<CreateGroupResponse> CreateGroupAsync(CreateGroupRequest request, string ipAddress)
        {
            // Step 1 — the token must check out. This is the "is the
            // login still legit" guard the frontend route guard alone
            // can't provide — same verification UserLoginAuthService
            // does on every protected page load.
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
                // Expired/revoked tokens are a normal, everyday
                // occurrence (sessions naturally time out) —
                // deliberately not logged, same as UserLoginAuthService.

                return new CreateGroupResponse
                {
                    Success = false,
                    Message = "Your session could not be verified. Please sign in again."
                };
            }

            // Step 2 — server-side validation. Never trust the frontend
            // alone, even though it already enforces the 20-char limit.
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

            // Normalize casing regardless of exactly what the frontend sent.
            string groupType = string.Equals(requestedType, "Premium", StringComparison.OrdinalIgnoreCase)
                ? "Premium"
                : "Free";

            // Step 3 — the creator must have a real FinBine profile, and
            // must not already belong to a group.
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

            // Step 4 — create the group across all three systems:
            // fb_groups -> fb_users (owner assignment) -> Postgres
            // anchor row. All-or-nothing: if any step fails, every step
            // that already succeeded is undone, in reverse order — same
            // pattern as UserRegistrationService's Firebase Auth ->
            // Firestore -> Postgres flow. This is what makes sure a
            // partial failure never leaves the owner's fb_users doc
            // pointing at a group that doesn't fully exist.
            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string? groupId = null;
            string failedAtSource = "Firestore";

            try
            {
                failedAtSource = "Firestore (fb_groups)";
                groupId = await _groupFirestoreService.GenerateNextGroupIdAsync();

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

                await _groupFirestoreService.CreateGroupDocumentAsync(groupId, groupAccount);
                rollbackActions.Add(("Firestore (fb_groups)", async () => await _groupFirestoreService.DeleteGroupDocumentAsync(groupId)));

                failedAtSource = "Firestore (fb_users)";
                await _groupFirestoreService.AssignOwnerToGroupAsync(owner.UserId, groupId, groupName);
                rollbackActions.Add(("Firestore (fb_users)", async () => await _groupFirestoreService.RevertOwnerGroupAssignmentAsync(owner.UserId)));

                failedAtSource = "PostgreSQL";
                await SavePostgresRecordAsync(groupId, owner.UserId, groupType);

                // All three succeeded. Deliberately not logged — only
                // problems get logged here, not every clean group creation.
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
                _regLogger.LogRollbackTriggered(failedAtSource, owner.UserId, groupId, ex.Message);
                await RollbackAsync(rollbackActions, owner.UserId, groupId);

                return new CreateGroupResponse
                {
                    Success = false,
                    Message = "Group creation failed. Please try again."
                };
            }
        }

        private async Task SavePostgresRecordAsync(string groupId, string ownerUserId, string groupType)
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

        // Undoes whatever already succeeded, most recent first. If a
        // rollback action itself fails, that's logged as Critical — it
        // means a human needs to manually clean up an orphaned record,
        // same convention as UserRegistrationService.RollbackAsync.
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
