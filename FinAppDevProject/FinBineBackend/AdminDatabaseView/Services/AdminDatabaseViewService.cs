using System.Security.Cryptography;
using FirebaseAdmin.Auth;
using FinBineBackend.AdminAuthentication.Services;
using FinBineBackend.AdminDatabaseView.Logs.Services;
using FinBineBackend.AdminDatabaseView.Models;
using FinBineBackend.UserAccRegistration.Data;
using FinBineBackend.UserAccRegistration.Models;
using FinBineBackend.UserAccRegistration.Services;
using Microsoft.EntityFrameworkCore;

namespace FinBineBackend.AdminDatabaseView.Services
{
    // Powers the Admin > Development > DbUsers page: a single merged,
    // row-per-human view across the 3 places a user actually lives —
    // Firebase Authentication, the fb_users Firestore collection, and
    // the Postgres Users table — plus the ability to add a manual/test
    // row or delete a row out of all 3 with one click.
    public class AdminDatabaseViewService
    {
        private readonly AdminAuthService _adminAuthService;
        private readonly UserFirestoreService _userFirestoreService;
        private readonly UserDbContext _userDb;
        private readonly AdminDatabaseViewLoggingService _logger;

        public AdminDatabaseViewService(
            AdminAuthService adminAuthService,
            UserFirestoreService userFirestoreService,
            UserDbContext userDb,
            AdminDatabaseViewLoggingService logger)
        {
            _adminAuthService = adminAuthService;
            _userFirestoreService = userFirestoreService;
            _userDb = userDb;
            _logger = logger;
        }

        // ------------------------------------------------------------
        // LIST — merges all 3 sources into one row per person
        // ------------------------------------------------------------
        public async Task<ListDbUsersResponse> ListDbUsersAsync(string token, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new ListDbUsersResponse { Success = false, Message = adminCheck.Message };
            }

            // Pull all 3 sources independently first, then stitch them
            // together in memory — there's no single key that lives
            // natively in all 3 stores (Firebase Auth only knows its
            // own uid), so the join has to happen here.
            List<ExportedUserRecord> firebaseUsers = await ListAllFirebaseAuthUsersAsync();
            List<FirestoreUserAccount> firestoreUsers = await _userFirestoreService.GetAllUsersAsync();
            List<UserDbRecord> postgresUsers = await _userDb.Users.AsNoTracking().ToListAsync();

            var rows = new Dictionary<string, DbUserRow>();

            // Firestore is the "hub" — it carries both its own userId
            // (fb_user_######, shared with Postgres) and the
            // firebase_uid that links to Firebase Auth. Start from it.
            foreach (var fsUser in firestoreUsers)
            {
                var row = new DbUserRow
                {
                    RowKey = fsUser.UserId,
                    FirestoreExists = true,
                    FirestoreUserId = fsUser.UserId,
                    FirestoreDisplayName = fsUser.DisplayName,
                    FirestoreFirstName = fsUser.FirstName,
                    FirestoreLastName = fsUser.LastName,
                    FirestoreAccountEmail = fsUser.AccountEmail,
                    FirestoreAccountType = fsUser.AccountType,
                    FirestoreMemberStatus = fsUser.MemberStatus,
                    FirestoreGroupId = fsUser.GroupId,
                    FirestoreGroupName = fsUser.GroupName,
                    FirestoreCountry = fsUser.Country,
                    FirestoreJoinedAt = fsUser.JoinedAt,
                    FirestoreLastActivity = fsUser.LastActivity,
                    FirebaseUid = fsUser.FirebaseUid
                };

                rows[fsUser.UserId] = row;
            }

            // Postgres shares the same userId as Firestore — attach to
            // the existing row if there is one, otherwise this is a
            // Postgres-only orphan (e.g. Firestore write rolled back
            // but Postgres somehow didn't).
            foreach (var pgUser in postgresUsers)
            {
                if (!rows.TryGetValue(pgUser.UserId, out var row))
                {
                    row = new DbUserRow { RowKey = pgUser.UserId };
                    rows[pgUser.UserId] = row;
                }

                row.PostgresExists = true;
                row.PostgresUserId = pgUser.UserId;
                row.PostgresPreferName = pgUser.PreferName;
                row.PostgresLastName = pgUser.LastName;
                row.PostgresDateOfBirth = pgUser.DateOfBirth.ToString("yyyy-MM-dd");
                row.PostgresAccountType = pgUser.AccountType;
                row.PostgresGroupId = pgUser.GroupId;
                row.PostgresCreatedAt = pgUser.CreatedAt.ToString("o");
            }

            // Firebase Auth only knows its own uid — match it against
            // whichever row already claims that firebase_uid. If none
            // do, it's a Firebase-only orphan (a real login with no
            // profile behind it at all).
            var rowsByFirebaseUid = rows.Values
                .Where(r => !string.IsNullOrEmpty(r.FirebaseUid))
                .ToDictionary(r => r.FirebaseUid!, r => r);

            foreach (var fbUser in firebaseUsers)
            {
                if (!rowsByFirebaseUid.TryGetValue(fbUser.Uid, out var row))
                {
                    row = new DbUserRow { RowKey = fbUser.Uid };
                    rows[fbUser.Uid] = row;
                }

                row.FirebaseAuthExists = true;
                row.FirebaseUid = fbUser.Uid;
                row.FirebaseEmail = fbUser.Email;
                row.FirebaseEmailVerified = fbUser.EmailVerified;
                row.FirebaseDisabled = fbUser.Disabled;
                row.FirebaseCreatedAt = fbUser.UserMetaData?.CreationTimestamp?.ToString("o");
                row.FirebaseLastSignIn = fbUser.UserMetaData?.LastSignInTimestamp?.ToString("o");
            }

            var sortedRows = rows.Values
                .OrderBy(r => r.RowKey)
                .ToList();

            return new ListDbUsersResponse
            {
                Success = true,
                Message = $"{sortedRows.Count} row(s) found.",
                Rows = sortedRows
            };
        }

        // ------------------------------------------------------------
        // ADD — manual/test-data entry, written to all 3 stores
        // ------------------------------------------------------------
        public async Task<AddDbUserResponse> AddDbUserAsync(AddDbUserRequest request, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(request.Token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new AddDbUserResponse { Success = false, Message = adminCheck.Message };
            }

            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new AddDbUserResponse { Success = false, Message = "Email is required." };
            }

            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string? firebaseUid = null;
            string? userId = null;
            string failedAtSource = "FirebaseAuth";
            bool generatedPassword = string.IsNullOrWhiteSpace(request.Password);
            string password = generatedPassword ? GenerateTempPassword() : request.Password!;

            try
            {
                // Step 1 — Firebase Auth. Still a real account, since
                // both other stores are keyed off its uid — but no
                // welcome/reset email, this is admin-entered test data.
                failedAtSource = "FirebaseAuth";
                var userArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = password,
                    DisplayName = request.DisplayName,
                    EmailVerified = false
                };

                UserRecord firebaseUser;
                try
                {
                    firebaseUser = await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
                }
                catch (FirebaseAuthException ex)
                {
                    _logger.LogAddFailed("FirebaseAuth", request.Email, ex.Message);

                    bool alreadyExists = ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists;
                    return new AddDbUserResponse
                    {
                        Success = false,
                        Message = alreadyExists
                            ? "An account with this email already exists."
                            : "Could not create the Firebase Auth account."
                    };
                }

                firebaseUid = firebaseUser.Uid;
                rollbackActions.Add(("FirebaseAuth", async () => await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid)));

                // Step 2 — Firestore.
                failedAtSource = "Firestore";
                userId = await _userFirestoreService.GenerateNextUserIdAsync();

                var firestoreAccount = new FirestoreUserAccount
                {
                    DisplayName = request.DisplayName,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    AccountEmail = request.Email,
                    FirebaseUid = firebaseUid,
                    JoinedAt = DateTime.UtcNow.ToString("o"),
                    AccountType = request.AccountType,
                    GroupId = null,
                    GroupName = null,
                    SignInMethod = "Manual (Admin)",
                    MemberStatus = "Active",
                    IsOwner = false,
                    Country = request.Country,
                    Currency = request.Currency,
                    Timezone = request.Timezone,
                    LastActivity = null
                };

                await _userFirestoreService.CreateUserDocumentAsync(userId, firestoreAccount);
                rollbackActions.Add(("Firestore", async () => await _userFirestoreService.DeleteUserDocumentAsync(userId)));

                // Step 3 — Postgres.
                failedAtSource = "PostgreSQL";
                var userDbRecord = new UserDbRecord
                {
                    UserId = userId,
                    PreferName = request.DisplayName,
                    LastName = request.LastName,
                    DateOfBirth = request.DateOfBirth,
                    AccountType = request.AccountType,
                    GroupId = null
                };

                _userDb.Users.Add(userDbRecord);
                await _userDb.SaveChangesAsync();

                _logger.LogManualUserAdded(userId, request.Email);

                return new AddDbUserResponse
                {
                    Success = true,
                    Message = "User record added to all 3 tables.",
                    UserId = userId,
                    GeneratedPassword = generatedPassword ? password : null
                };
            }
            catch (Exception ex)
            {
                _logger.LogAddFailed(failedAtSource, request.Email, ex.Message);
                await RollbackAsync(rollbackActions, userId, firebaseUid);

                return new AddDbUserResponse
                {
                    Success = false,
                    Message = "Add failed — any partial rows were rolled back."
                };
            }
        }

        // ------------------------------------------------------------
        // DELETE — best-effort across all 3 stores
        // ------------------------------------------------------------
        public async Task<DeleteDbUserResponse> DeleteDbUserAsync(DeleteDbUserRequest request, string ipAddress)
        {
            var adminCheck = await _adminAuthService.VerifyTokenAsync(request.Token, ipAddress);
            if (!adminCheck.Success)
            {
                _logger.LogUnauthorizedAccess(ipAddress);
                return new DeleteDbUserResponse { Success = false, Message = adminCheck.Message };
            }

            if (string.IsNullOrWhiteSpace(request.UserId) && string.IsNullOrWhiteSpace(request.FirebaseUid))
            {
                return new DeleteDbUserResponse { Success = false, Message = "UserId or FirebaseUid is required." };
            }

            string identifier = request.UserId ?? request.FirebaseUid!;
            string? firebaseUid = request.FirebaseUid;

            // If we only have the userId, look up its Firestore doc to
            // find the matching firebase_uid before it's deleted.
            if (string.IsNullOrEmpty(firebaseUid) && !string.IsNullOrEmpty(request.UserId))
            {
                var fsUser = await _userFirestoreService.GetUserByIdAsync(request.UserId);
                firebaseUid = fsUser?.FirebaseUid;
            }

            bool firebaseAuthDeleted = false;
            bool firestoreDeleted = false;
            bool postgresDeleted = false;

            if (!string.IsNullOrEmpty(firebaseUid))
            {
                try
                {
                    await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid);
                    firebaseAuthDeleted = true;
                }
                catch (FirebaseAuthException ex)
                {
                    _logger.LogDeleteFailed($"FirebaseAuth:{firebaseUid}", ex.Message);
                }
            }

            if (!string.IsNullOrEmpty(request.UserId))
            {
                try
                {
                    await _userFirestoreService.DeleteUserDocumentAsync(request.UserId);
                    firestoreDeleted = true;
                }
                catch (Exception ex)
                {
                    _logger.LogDeleteFailed($"Firestore:{request.UserId}", ex.Message);
                }

                try
                {
                    var pgRecord = await _userDb.Users.FindAsync(request.UserId);
                    if (pgRecord != null)
                    {
                        _userDb.Users.Remove(pgRecord);
                        await _userDb.SaveChangesAsync();
                        postgresDeleted = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDeleteFailed($"Postgres:{request.UserId}", ex.Message);
                }
            }

            _logger.LogUserDeleted(identifier, firebaseAuthDeleted, firestoreDeleted, postgresDeleted);

            bool anyDeleted = firebaseAuthDeleted || firestoreDeleted || postgresDeleted;

            return new DeleteDbUserResponse
            {
                Success = anyDeleted,
                Message = anyDeleted
                    ? "Delete completed."
                    : "Nothing was found to delete for this row.",
                FirebaseAuthDeleted = firebaseAuthDeleted,
                FirestoreDeleted = firestoreDeleted,
                PostgresDeleted = postgresDeleted
            };
        }

        // ------------------------------------------------------------
        // Helpers
        // ------------------------------------------------------------
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

        private async Task RollbackAsync(List<(string Source, Func<Task> Action)> rollbackActions, string? userId, string? firebaseUid)
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
                    _logger.LogAddRollbackFailed(source, userId, firebaseUid, rollbackEx.Message);
                }
            }
        }

        private static string GenerateTempPassword()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));
        }
    }
}
