using System.Text.Json;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using FinBineBackend.AdminAuthentication.Models;
using FinBineBackend.AdminAuthentication.Logs.Services;

namespace FinBineBackend.AdminAuthentication.Services
{
    public class AdminAuthService
    {
        private const string CredentialsPath = "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json";
        private const string AdminCollectionName = "fb_admin_users";

        private readonly FirestoreDb _firestoreDb;
        private readonly AuthLoggingService _authLogger;

        public AdminAuthService(AuthLoggingService authLogger)
        {
            _authLogger = authLogger;

            // Load the credentials file once, using the newer,
            // non-deprecated CredentialFactory approach.
            var credential = CredentialFactory.FromFile(CredentialsPath, "service_account");

            // Initialize Firebase only once
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential
                });
            }

            // Firestore needs to know which Google Cloud project to talk to.
            // We read that project ID out of the same credentials file.
            var projectId = ReadProjectIdFromCredentialsFile(CredentialsPath);

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                GoogleCredential = credential
            }.Build();
        }

        public async Task<AdminLoginResponse> VerifyTokenAsync(string token, string ipAddress)
        {
            try
            {
                // Step 1: Confirm the login token is genuine and not expired.
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                string uid = decodedToken.Uid;

                // Step 2: Confirm this person actually has a document in
                // fb_admin_users. A valid Firebase login on its own is NOT
                // enough to be treated as an admin.
                AdminAccount? adminAccount = await FindAdminAccountByUidAsync(uid);

                if (adminAccount == null)
                {
                    _authLogger.LogAdminNotRegistered(ipAddress);

                    return new AdminLoginResponse
                    {
                        Success = false,
                        Message = "This account is not registered as a FinBine admin."
                    };
                }

                _authLogger.LogAdminLoginSuccess(adminAccount.PreferName, adminAccount.Surname, adminAccount.AccountType);

                return new AdminLoginResponse
                {
                    Success = true,
                    Message = "Admin authenticated successfully",
                    PreferName = adminAccount.PreferName,
                    Surname = adminAccount.Surname,
                    AccountType = adminAccount.AccountType
                };
            }
            catch (FirebaseAuthException ex)
            {
                // Different kinds of token problems get different log levels.
                if (ex.AuthErrorCode == AuthErrorCode.ExpiredIdToken)
                {
                    _authLogger.LogTokenExpired(ipAddress);
                }
                else if (ex.AuthErrorCode == AuthErrorCode.RevokedIdToken)
                {
                    _authLogger.LogTokenRevoked(ipAddress);
                }
                else if (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    _authLogger.LogTokenInvalid(ipAddress);
                }
                else
                {
                    _authLogger.LogAuthErrorOther(ex.AuthErrorCode?.ToString() ?? "Unknown", ipAddress);
                }

                return new AdminLoginResponse
                {
                    Success = false,
                    Message = $"Invalid token: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _authLogger.LogSystemError(ex.Message);

                return new AdminLoginResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }

        // Looks in the fb_admin_users collection for a document whose
        // "firebase_uid" field matches the UID from the login token.
        // Returns null if no such document exists — meaning this person
        // is NOT a registered admin, even if their Firebase login is valid.
        private async Task<AdminAccount?> FindAdminAccountByUidAsync(string uid)
        {
            Query query = _firestoreDb
                .Collection(AdminCollectionName)
                .WhereEqualTo("firebase_uid", uid)
                .Limit(1);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return null;
            }

            return snapshot.Documents[0].ConvertTo<AdminAccount>();
        }

        // Pulls "project_id" out of the Firebase service account JSON file.
        private static string ReadProjectIdFromCredentialsFile(string credentialsPath)
        {
            using var stream = File.OpenRead(credentialsPath);
            using var document = JsonDocument.Parse(stream);

            return document.RootElement.GetProperty("project_id").GetString()
                ?? throw new InvalidOperationException(
                    "Could not find 'project_id' in the Firebase service account file.");
        }
    }
}