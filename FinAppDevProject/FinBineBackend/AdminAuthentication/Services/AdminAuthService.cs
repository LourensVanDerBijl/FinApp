using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using FinBineBackend.AdminAuthentication.Models;

namespace FinBineBackend.AdminAuthentication.Services
{
    public class AdminAuthService
    {
        public AdminAuthService()
        {
            // Initialize Firebase only once
            if (FirebaseApp.DefaultInstance == null)
            {
                var credential = GoogleCredential.FromFile(
                    "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json"
                );

                FirebaseApp.Create(new AppOptions()
                {
                    Credential = credential
                });
            }
        }

        public async Task<AdminLoginResponse> VerifyTokenAsync(string token)
        {
            try
            {
                // Verify the token with Firebase
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                // Extract user info
                string uid = decodedToken.Uid;
                string? email = decodedToken.Claims.ContainsKey("email")
                    ? decodedToken.Claims["email"].ToString()
                    : null;

                // ✅ No admin claim check needed
                return new AdminLoginResponse
                {
                    Success = true,
                    Message = "Admin authenticated successfully",
                    AdminId = uid,
                    Email = email
                };
            }
            catch (FirebaseAuthException ex)
            {
                return new AdminLoginResponse
                {
                    Success = false,
                    Message = $"Invalid token: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                return new AdminLoginResponse
                {
                    Success = false,
                    Message = $"Unexpected error: {ex.Message}"
                };
            }
        }
    }
}
