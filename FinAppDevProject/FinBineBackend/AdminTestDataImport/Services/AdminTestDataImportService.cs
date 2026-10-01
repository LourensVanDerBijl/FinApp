using FirebaseAdmin.Auth;
using FinBineBackend.AdminAuthentication.Services;
using FinBineBackend.AdminTestDataImport.Logs.Services;
using FinBineBackend.AdminTestDataImport.Models;
using FinBineBackend.UserAccRegistration.Data;
using FinBineBackend.UserAccRegistration.Models;
using FinBineBackend.UserAccRegistration.Services;
using FinBineBackend.UserGroupRegistration.Data;
using FinBineBackend.UserGroupRegistration.Models;
using FinBineBackend.UserGroupRegistration.Services;
using Microsoft.EntityFrameworkCore;

namespace FinBineBackend.AdminTestDataImport.Services
{
    // Powers the Admin > Development > Test Data page. Takes a whole
    // uploaded JSON file (TestDataPayload — groups, each with one
    // owner and a list of members) and writes every group + user
    // across all 3 stores (Firebase Auth, Firestore, Postgres), using
    // the exact same ID-generation and field-setting calls a real
    // group-creation / join-request would use — so imported test data
    // is indistinguishable from data a real user would have produced.
    //
    // Strict, two-phase: TestDataImportValidator checks the ENTIRE file
    // first and returns every problem found. If there's even one error,
    // nothing is written — no partial import, no "fix and re-upload
    // the same 3 rows" loop. Only once validation passes clean does
    // any write happen, and if a write fails partway through, the
    // *entire* batch (every group/user created so far, this call only)
    // is rolled back — not just the one group that failed.
    public class AdminTestDataImportService
    {
        // Hardcoded, not sourced from the request — this is the actual
        // safety net. ClearAllTestDataAsync unions this with whatever
        // email the frontend sends, so a blank/wrong/missing value from
        // the client can only ever over-protect, never under-protect.
        // Add more emails here if other real accounts need the same
        // guarantee.
        private static readonly string[] AlwaysProtectedEmails = { "lourensvdbijl@gmail.com" };

        private readonly AdminAuthService _adminAuthService;
        private readonly UserFirestoreService _userFirestoreService;
        private readonly GroupFirestoreService _groupFirestoreService;
        private readonly UserDbContext _userDb;
        private readonly GroupDbContext _groupDb;
        private readonly AdminTestDataImportLoggingService _logger;

        public AdminTestDataImportService(
            AdminAuthService adminAuthService,
            UserFirestoreService userFirestoreService,
            GroupFirestoreService groupFirestoreService,
            UserDbContext userDb,
            GroupDbContext groupDb,
            AdminTestDataImportLoggingService logger)
        {
            _adminAuthService = adminAuthService;
            _userFirestoreService = userFirestoreService;
            _groupFirestoreService = groupFirestoreService;
            _userDb = userDb;
            _groupDb = groupDb;
            _logger = logger;
        }

        public async Task<ImportTestDataResponse> ImportTestDataAsync(ImportTestDataRequest request, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(request.Token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new ImportTestDataResponse { Success = false, Message = adminCheck.Message };
            }

            // ---- Phase 1: structural + business-rule validation ----
            var errors = TestDataImportValidator.Validate(request.Payload);

            // ---- Phase 1b: duplicate-email pre-check ----
            // Not something the file's own shape can tell you — has to
            // hit Firebase Auth. Checked before any writes so a
            // duplicate anywhere in the batch aborts the whole thing,
            // same "nothing partial" guarantee as the shape checks.
            if (errors.Count == 0)
            {
                errors.AddRange(await FindAlreadyRegisteredEmailsAsync(request.Payload));
            }

            if (errors.Count > 0)
            {
                _logger.LogValidationRejected(errors.Count, request.Payload.Groups.Count);
                return new ImportTestDataResponse
                {
                    Success = false,
                    Message = $"Rejected — {errors.Count} problem(s) found. Nothing was written.",
                    ValidationErrors = errors
                };
            }

            // ---- Phase 2: write everything, whole-batch rollback on any failure ----
            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            int groupsCreated = 0;
            int usersCreated = 0;
            string failedAtSource = "";
            string failedAtDetail = "";

            try
            {
                foreach (var group in request.Payload.Groups)
                {
                    failedAtDetail = group.GroupId;

                    // --- Owner: Firebase Auth -> Firestore fb_users -> Postgres Users ---
                    failedAtSource = "FirebaseAuth (owner)";
                    string ownerUserId = await CreateFullUserAsync(group.Owner, groupId: null, rollbackActions);
                    usersCreated++;

                    // --- Group: Firestore fb_groups -> Postgres Groups, using the owner's real userId ---
                    failedAtSource = "Firestore (fb_groups)";
                    string realGroupId = await _groupFirestoreService.GenerateNextGroupIdAsync();
                    string nowIso = DateTime.UtcNow.ToString("o");

                    var groupAccount = new FirestoreGroupAccount
                    {
                        GroupName = group.GroupName,
                        OwnerUserId = ownerUserId,
                        CountryCode = group.Owner.Country,
                        CurrencyCode = group.Owner.Currency,
                        TimeZone = group.Owner.Timezone,
                        GroupType = group.GroupType,
                        Status = "Active",
                        PaymentStatus = "Unpaid",
                        CreatedAt = nowIso,
                        SubscriptionStartDate = string.Equals(group.GroupType, "Premium", StringComparison.OrdinalIgnoreCase) ? nowIso : null,
                        SubscriptionEndDate = null,
                        LastActivityAt = nowIso
                    };

                    await _groupFirestoreService.CreateGroupDocumentAsync(realGroupId, groupAccount);
                    rollbackActions.Add(("Firestore (fb_groups)", async () => await _groupFirestoreService.DeleteGroupDocumentAsync(realGroupId)));

                    failedAtSource = "PostgreSQL (Groups)";
                    _groupDb.Groups.Add(new GroupDbRecord
                    {
                        GroupId = realGroupId,
                        OwnerUserId = ownerUserId,
                        GroupType = group.GroupType
                    });
                    await _groupDb.SaveChangesAsync();
                    rollbackActions.Add(("PostgreSQL (Groups)", async () =>
                    {
                        var rec = await _groupDb.Groups.FindAsync(realGroupId);
                        if (rec != null) { _groupDb.Groups.Remove(rec); await _groupDb.SaveChangesAsync(); }
                    }
                    ));
                    groupsCreated++;

                    // --- Now stamp the owner's group fields with the REAL GroupId ---
                    failedAtSource = "Firestore (owner group fields)";
                    await _groupFirestoreService.AssignOwnerToGroupAsync(ownerUserId, realGroupId, group.GroupName, group.GroupType);
                    // No separate rollback entry needed — deleting the owner's
                    // fb_users doc entirely (already queued by CreateFullUserAsync)
                    // undoes this too.

                    failedAtSource = "PostgreSQL (owner group fields)";
                    var ownerRecord = await _userDb.Users.FindAsync(ownerUserId)
                        ?? throw new InvalidOperationException($"Postgres Users row for '{ownerUserId}' vanished mid-import.");
                    ownerRecord.GroupId = realGroupId;
                    ownerRecord.AccountType = group.GroupType;
                    ownerRecord.GroupStatus = GroupMembershipStatus.Active;
                    await _userDb.SaveChangesAsync();

                    // --- Members: same 3-store write, pointed at the REAL GroupId from the start ---
                    foreach (var member in group.Members)
                    {
                        failedAtDetail = $"{group.GroupId} / {member.TestId}";
                        failedAtSource = "FirebaseAuth (member)";
                        await CreateFullUserAsync(member, groupId: realGroupId, rollbackActions);
                        usersCreated++;
                    }
                }

                _logger.LogImportSucceeded(groupsCreated, usersCreated);

                return new ImportTestDataResponse
                {
                    Success = true,
                    Message = $"Imported {groupsCreated} group(s) and {usersCreated} user(s).",
                    GroupsCreated = groupsCreated,
                    UsersCreated = usersCreated
                };
            }
            catch (Exception ex)
            {
                _logger.LogImportFailed(failedAtSource, failedAtDetail, ex.Message);
                await RollbackAsync(rollbackActions);

                return new ImportTestDataResponse
                {
                    Success = false,
                    Message = $"Import failed at {failedAtSource} ({failedAtDetail}) — every group/user created in this batch was rolled back. {ex.Message}"
                };
            }
        }

        // ------------------------------------------------------------
        // CLEAR ALL — wipes every user row across all 3 stores except
        // whatever is in AlwaysProtectedEmails ∪ request.ExcludeEmail,
        // AND every group not owned by one of those protected users.
        //
        // Best-effort per row, not all-or-nothing: this is already a
        // "wipe and start fresh" operation, so one failed row
        // shouldn't block clearing the other 100. Every failure is
        // collected and returned rather than silently swallowed.
        // ------------------------------------------------------------
        public async Task<ClearTestDataResponse> ClearAllTestDataAsync(ClearTestDataRequest request, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(request.Token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new ClearTestDataResponse { Success = false, Message = adminCheck.Message };
            }

            var protectedEmails = new HashSet<string>(AlwaysProtectedEmails, StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(request.ExcludeEmail))
            {
                protectedEmails.Add(request.ExcludeEmail.Trim());
            }

            // Resolve which actual rows those protected emails point
            // to, across all 3 stores. If NONE of the protected emails
            // resolve to a real Firebase Auth account, refuse outright
            // — proceeding would mean "delete everyone" with no
            // survivor, and that's never the intent of this button.
            List<ExportedUserRecord> firebaseUsers = await ListAllFirebaseAuthUsersAsync();
            List<FirestoreUserAccount> firestoreUsers = await _userFirestoreService.GetAllUsersAsync();
            List<UserDbRecord> postgresUsers = await _userDb.Users.ToListAsync();

            var protectedFirebaseUids = firebaseUsers
                .Where(u => !string.IsNullOrEmpty(u.Email) && protectedEmails.Contains(u.Email))
                .Select(u => u.Uid)
                .ToHashSet();

            if (protectedFirebaseUids.Count == 0)
            {
                return new ClearTestDataResponse
                {
                    Success = false,
                    Message = $"Refused — none of the protected email(s) ({string.Join(", ", protectedEmails)}) " +
                              "resolved to a real Firebase Auth account. Clearing would have removed every user " +
                              "with no survivor, so nothing was touched."
                };
            }

            // A Firestore/Postgres row is protected if it's owned by a
            // protected Firebase UID, OR its own account email matches
            // — covers a row whose firebase_uid drifted or is missing.
            var protectedUserIds = firestoreUsers
                .Where(u => (u.FirebaseUid != null && protectedFirebaseUids.Contains(u.FirebaseUid))
                            || (!string.IsNullOrEmpty(u.AccountEmail) && protectedEmails.Contains(u.AccountEmail)))
                .Select(u => u.UserId)
                .ToHashSet();

            var errors = new List<string>();
            int firestoreDeleted = 0, postgresDeleted = 0, firebaseDeleted = 0;

            foreach (var fsUser in firestoreUsers)
            {
                if (protectedUserIds.Contains(fsUser.UserId)) continue;
                try
                {
                    await _userFirestoreService.DeleteUserDocumentAsync(fsUser.UserId);
                    firestoreDeleted++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Firestore ({fsUser.UserId}): {ex.Message}");
                }
            }

            foreach (var pgUser in postgresUsers)
            {
                if (protectedUserIds.Contains(pgUser.UserId)) continue;
                try
                {
                    _userDb.Users.Remove(pgUser);
                    await _userDb.SaveChangesAsync();
                    postgresDeleted++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Postgres ({pgUser.UserId}): {ex.Message}");
                }
            }

            foreach (var fbUser in firebaseUsers)
            {
                if (protectedFirebaseUids.Contains(fbUser.Uid)) continue;
                try
                {
                    await FirebaseAuth.DefaultInstance.DeleteUserAsync(fbUser.Uid);
                    firebaseDeleted++;
                }
                catch (Exception ex)
                {
                    errors.Add($"FirebaseAuth ({fbUser.Uid} / {fbUser.Email}): {ex.Message}");
                }
            }

            // ---- Groups: delete every group NOT owned by a protected user ----
            // A group survives only if its OwnerUserId is one of the
            // protected users — everything else gets deleted along
            // with its owner, across both stores.
            List<FirestoreGroupAccount> firestoreGroups = await _groupFirestoreService.GetAllGroupsAsync();
            List<GroupDbRecord> postgresGroups = await _groupDb.Groups.ToListAsync();

            int firestoreGroupsDeleted = 0, postgresGroupsDeleted = 0;

            foreach (var fsGroup in firestoreGroups)
            {
                if (protectedUserIds.Contains(fsGroup.OwnerUserId)) continue;
                try
                {
                    await _groupFirestoreService.DeleteGroupDocumentAsync(fsGroup.GroupId);
                    firestoreGroupsDeleted++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Firestore group ({fsGroup.GroupId}): {ex.Message}");
                }
            }

            foreach (var pgGroup in postgresGroups)
            {
                if (protectedUserIds.Contains(pgGroup.OwnerUserId)) continue;
                try
                {
                    _groupDb.Groups.Remove(pgGroup);
                    await _groupDb.SaveChangesAsync();
                    postgresGroupsDeleted++;
                }
                catch (Exception ex)
                {
                    errors.Add($"Postgres group ({pgGroup.GroupId}): {ex.Message}");
                }
            }

            _logger.LogClearAllCompleted(
                firebaseDeleted, firestoreDeleted, postgresDeleted,
                firestoreGroupsDeleted, postgresGroupsDeleted,
                protectedEmails.Count, errors.Count);

            return new ClearTestDataResponse
            {
                Success = true,
                Message = $"Cleared {firebaseDeleted} Firebase Auth, {firestoreDeleted} Firestore, and {postgresDeleted} Postgres " +
                          $"user row(s), plus {firestoreGroupsDeleted} Firestore and {postgresGroupsDeleted} Postgres group(s). " +
                          $"Protected: {string.Join(", ", protectedEmails)}." +
                          (errors.Count > 0 ? $" {errors.Count} row(s) failed to delete — see errors." : ""),
                FirebaseAuthDeleted = firebaseDeleted,
                FirestoreDeleted = firestoreDeleted,
                PostgresDeleted = postgresDeleted,
                FirestoreGroupsDeleted = firestoreGroupsDeleted,
                PostgresGroupsDeleted = postgresGroupsDeleted,
                ProtectedEmails = protectedEmails.ToList(),
                Errors = errors
            };
        }

        // Creates one user across all 3 stores and queues its rollback
        // actions. If groupId is null, the user is written with no
        // group (used for the owner slot, which gets its group fields
        // stamped separately once the REAL GroupId exists — see caller).
        private async Task<string> CreateFullUserAsync(
            TestDataUserPayload user,
            string? groupId,
            List<(string Source, Func<Task> Action)> rollbackActions)
        {
            var firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(new UserRecordArgs
            {
                Email = user.Email,
                Password = user.Password,
                DisplayName = user.DisplayName,
                EmailVerified = false
            });

            string firebaseUid = firebaseUser.Uid;
            rollbackActions.Add(("FirebaseAuth", async () => await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid)));

            string userId = await _userFirestoreService.GenerateNextUserIdAsync();

            var firestoreAccount = new FirestoreUserAccount
            {
                DisplayName = user.DisplayName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                AccountEmail = user.Email,
                FirebaseUid = firebaseUid,
                JoinedAt = DateTime.UtcNow.ToString("o"),
                AccountType = user.AccountType,
                GroupId = groupId,
                GroupName = groupId == null ? null : user.GroupName,
                GroupStatus = groupId == null ? GroupMembershipStatus.None : user.GroupStatus,
                SignInMethod = "Manual (Admin Test Data Import)",
                MemberStatus = "Active",
                IsOwner = user.IsOwner,
                Country = user.Country,
                Currency = user.Currency,
                Timezone = user.Timezone,
                LastActivity = null
            };

            await _userFirestoreService.CreateUserDocumentAsync(userId, firestoreAccount);
            rollbackActions.Add(("Firestore (fb_users)", async () => await _userFirestoreService.DeleteUserDocumentAsync(userId)));

            var userDbRecord = new UserDbRecord
            {
                UserId = userId,
                PreferName = user.DisplayName,
                LastName = user.LastName,
                DateOfBirth = DateOnly.Parse(user.DateOfBirth),
                AccountType = user.AccountType,
                GroupId = groupId,
                GroupStatus = groupId == null ? GroupMembershipStatus.None : user.GroupStatus
            };

            _userDb.Users.Add(userDbRecord);
            await _userDb.SaveChangesAsync();
            rollbackActions.Add(("PostgreSQL (Users)", async () =>
            {
                var rec = await _userDb.Users.FindAsync(userId);
                if (rec != null) { _userDb.Users.Remove(rec); await _userDb.SaveChangesAsync(); }
            }
            ));

            return userId;
        }

        private static async Task<List<string>> FindAlreadyRegisteredEmailsAsync(TestDataPayload payload)
        {
            var problems = new List<string>();
            var allUsers = payload.Groups.SelectMany(g => new[] { g.Owner }.Concat(g.Members));

            foreach (var user in allUsers)
            {
                if (string.IsNullOrWhiteSpace(user.Email)) continue; // already flagged by shape validation

                try
                {
                    await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(user.Email);
                    problems.Add($"{user.TestId} ({user.Email}): a Firebase Auth account with this email already exists.");
                }
                catch (FirebaseAuthException ex) when (ex.AuthErrorCode == AuthErrorCode.UserNotFound)
                {
                    // Good — email is free.
                }
            }

            return problems;
        }

        private async Task RollbackAsync(List<(string Source, Func<Task> Action)> rollbackActions)
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
                    _logger.LogRollbackFailed(source, $"item {i}", rollbackEx.Message);
                }
            }
        }

        // Same shape as AdminDatabaseViewService's private helper of
        // the same name — duplicated rather than shared, per this
        // project's convention of each feature owning its own
        // Firebase/Firestore/Postgres access rather than reaching into
        // another feature's service.
        private static async Task<List<ExportedUserRecord>> ListAllFirebaseAuthUsersAsync()
        {
            var users = new List<ExportedUserRecord>();
            var pagedEnumerable = FirebaseAuth.DefaultInstance.ListUsersAsync(null);

            await foreach (var user in pagedEnumerable)
            {
                users.Add(user);
            }

            return users;
        }
    }
}