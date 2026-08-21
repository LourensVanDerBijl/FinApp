using System.Security.Cryptography;
using FirebaseAdmin.Auth;
using FinBineBackend.UserAccRegistration.Models;
using FinBineBackend.UserAccRegistration.Data;
using FinBineBackend.UserAccRegistration.Logs.Services;

namespace FinBineBackend.UserAccRegistration.Services
{
    public class UserRegistrationService
    {
        private readonly UserFirestoreService _firestoreService;
        private readonly UserDbContext _userDb;
        private readonly UserRegistrationLoggingService _regLogger;

        public UserRegistrationService(
            UserFirestoreService firestoreService,
            UserDbContext userDb,
            UserRegistrationLoggingService regLogger)
        {
            _firestoreService = firestoreService;
            _userDb = userDb;
            _regLogger = regLogger;
        }

        // ------------------------------------------------------------
        // Email + Password — backend creates the Firebase account
        // itself, no token involved.
        // ------------------------------------------------------------
        public async Task<RegisterUserResponse> RegisterWithEmailAsync(RegisterUserRequest request)
        {
            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string? firebaseUid = null;
            string? userId = null;
            string failedAtSource = "FirebaseAuth";

            try
            {
                // Step 1 — Create the Firebase Authentication account with
                // a throwaway temp password nobody will ever see or use.
                string tempPassword = GenerateTempPassword();

                var userArgs = new UserRecordArgs
                {
                    Email = request.Email,
                    Password = tempPassword,
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
                    _regLogger.LogRegistrationRejected("FirebaseAuth", request.Email, ex.Message);

                    bool alreadyExists = ex.AuthErrorCode == AuthErrorCode.EmailAlreadyExists;

                    return new RegisterUserResponse
                    {
                        Success = false,
                        ErrorCode = alreadyExists ? "AccountAlreadyExists" : null,
                        Message = alreadyExists
                            ? "An account with this email already exists."
                            : "This email address could not be registered. Please try again."
                    };
                }

                firebaseUid = firebaseUser.Uid;
                rollbackActions.Add(("FirebaseAuth", async () => await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid)));

                // Step 2 — Reserve a sequential fb_user_###### ID and write
                // the Firestore profile document.
                failedAtSource = "Firestore";
                userId = await _firestoreService.GenerateNextUserIdAsync();

                var firestoreAccount = BuildFirestoreAccount(request, firebaseUid, "Email + Password");

                await _firestoreService.CreateUserDocumentAsync(userId, firestoreAccount);
                rollbackActions.Add(("Firestore", async () => await _firestoreService.DeleteUserDocumentAsync(userId)));

                // Step 3 — Write the minimal Postgres anchor row.
                failedAtSource = "PostgreSQL";
                await SavePostgresRecordAsync(request, userId);

                // All three succeeded. Deliberately not logged — only
                // problems get logged here, not every clean registration.
                return new RegisterUserResponse
                {
                    Success = true,
                    Message = "Account created. Check your email to set your password.",
                    UserId = userId
                };
            }
            catch (Exception ex)
            {
                _regLogger.LogRollbackTriggered(failedAtSource, request.Email, userId, ex.Message);
                await RollbackAsync(rollbackActions, userId, firebaseUid);

                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        // ------------------------------------------------------------
        // SSO (Google, Microsoft, Yahoo) — the frontend already
        // triggered the real popup and Firebase already created the
        // account; we just verify the token and finish the rest.
        // ------------------------------------------------------------
        public async Task<RegisterUserResponse> RegisterWithSSOAsync(RegisterUserRequest request, string idToken, string providerName)
        {
            var rollbackActions = new List<(string Source, Func<Task> Action)>();
            string? firebaseUid = null;
            string? userId = null;
            string failedAtSource = "FirebaseAuth";

            try
            {
                // Step 1 — Verify the token is genuine.
                FirebaseToken decodedToken;
                try
                {
                    decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                }
                catch (FirebaseAuthException ex)
                {
                    _regLogger.LogRegistrationRejected("FirebaseAuth", request.Email, $"Invalid token: {ex.Message}");
                    return new RegisterUserResponse
                    {
                        Success = false,
                        Message = "We couldn't verify your sign-in. Please try again."
                    };
                }

                firebaseUid = decodedToken.Uid;

                // The token's own email is the verified source of truth —
                // prefer it over whatever the form happened to submit.
                if (decodedToken.Claims.TryGetValue("email", out var tokenEmailObj) &&
                    tokenEmailObj?.ToString() is string tokenEmail &&
                    !string.IsNullOrWhiteSpace(tokenEmail))
                {
                    request.Email = tokenEmail;
                }

                // Guard against double-registration — if this Firebase
                // account already has a profile, don't create a second one.
                var existing = await _firestoreService.FindUserByFirebaseUidAsync(firebaseUid);
                if (existing != null)
                {
                    _regLogger.LogRegistrationRejected("Firestore", request.Email, "Account already registered.");
                    return new RegisterUserResponse
                    {
                        Success = false,
                        ErrorCode = "AccountAlreadyExists",
                        Message = "This account is already registered. Please sign in instead."
                    };
                }

                // If Firestore/Postgres fail below, we undo this claim by
                // deleting the Firebase account. The SSO provider created
                // it, not us — but an incomplete registration shouldn't
                // leave it dangling with no profile behind it.
                rollbackActions.Add(("FirebaseAuth", async () => await FirebaseAuth.DefaultInstance.DeleteUserAsync(firebaseUid)));

                // Step 2 — Firestore
                failedAtSource = "Firestore";
                userId = await _firestoreService.GenerateNextUserIdAsync();

                var firestoreAccount = BuildFirestoreAccount(request, firebaseUid, providerName);

                await _firestoreService.CreateUserDocumentAsync(userId, firestoreAccount);
                rollbackActions.Add(("Firestore", async () => await _firestoreService.DeleteUserDocumentAsync(userId)));

                // Step 3 — Postgres
                failedAtSource = "PostgreSQL";
                await SavePostgresRecordAsync(request, userId);

                return new RegisterUserResponse
                {
                    Success = true,
                    Message = $"Account created with {providerName}.",
                    UserId = userId
                };
            }
            catch (Exception ex)
            {
                _regLogger.LogRollbackTriggered(failedAtSource, request.Email, userId, ex.Message);
                await RollbackAsync(rollbackActions, userId, firebaseUid);

                return new RegisterUserResponse
                {
                    Success = false,
                    Message = "Registration failed. Please try again."
                };
            }
        }

        // ------------------------------------------------------------
        // Shared helpers
        // ------------------------------------------------------------
        private static FirestoreUserAccount BuildFirestoreAccount(RegisterUserRequest request, string firebaseUid, string signInMethod)
        {
            return new FirestoreUserAccount
            {
                DisplayName = request.DisplayName,
                FirstName = request.FirstName,
                LastName = request.LastName,
                AccountEmail = request.Email,
                FirebaseUid = firebaseUid,
                JoinedAt = DateTime.UtcNow.ToString("o"),
                AccountType = "Free",
                GroupId = null,
                GroupName = null,
                SignInMethod = signInMethod,
                MemberStatus = "Active",
                IsOwner = false,
                Country = request.Country,
                Currency = request.Currency,
                Timezone = request.Timezone,
                LastActivity = null
            };
        }

        private async Task SavePostgresRecordAsync(RegisterUserRequest request, string userId)
        {
            var userDbRecord = new UserDbRecord
            {
                UserId = userId,
                PreferName = request.DisplayName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                AccountType = "Free",
                GroupId = null
            };

            _userDb.Users.Add(userDbRecord);
            await _userDb.SaveChangesAsync();
        }

        // Undoes whatever already succeeded, most recent first. If a
        // rollback action itself fails, that's logged as Critical — it
        // means a human needs to manually clean up an orphaned record.
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
                    _regLogger.LogRollbackFailed(source, userId, firebaseUid, rollbackEx.Message);
                }
            }
        }

        private static string GenerateTempPassword()
        {
            // Never shown to anyone, never used to log in — the person
            // sets their real password via the reset-password email.
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(24));
        }
    }
}