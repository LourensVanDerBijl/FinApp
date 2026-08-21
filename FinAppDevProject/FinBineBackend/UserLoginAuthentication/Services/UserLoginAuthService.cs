using FirebaseAdmin;
using FirebaseAdmin.Auth;
using FinBineBackend.UserLoginAuthentication.Models;
using FinBineBackend.UserLoginAuthentication.Logs.Services;

namespace FinBineBackend.UserLoginAuthentication.Services
{
    public class UserLoginAuthService
    {
        private readonly UserFirestoreLoginService _firestoreService;
        private readonly UserLoginLoggingService _loginLogger;

        public UserLoginAuthService(
            UserFirestoreLoginService firestoreService,
            UserLoginLoggingService loginLogger)
        {
            _firestoreService = firestoreService;
            _loginLogger = loginLogger;
        }

        public async Task<UserLoginResponse> VerifyTokenAsync(string token, string ipAddress)
        {
            FirebaseToken decodedToken;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
            }
            catch (FirebaseAuthException ex)
            {
                if (ex.AuthErrorCode == AuthErrorCode.InvalidIdToken)
                {
                    // Structurally invalid / forged — the one case worth
                    // flagging as a possible theft attempt.
                    _loginLogger.LogTokenMismatch(ipAddress);
                }
                // Expired or revoked tokens are normal, everyday
                // occurrences (sessions naturally time out) — deliberately
                // not logged, per instruction to stay quiet on routine
                // outcomes.

                return new UserLoginResponse
                {
                    Success = false,
                    Message = "Your session could not be verified. Please sign in again."
                };
            }

            var account = await _firestoreService.FindUserByFirebaseUidAsync(decodedToken.Uid);

            if (account == null)
            {
                _loginLogger.LogAccountNotFound(ipAddress);
                return new UserLoginResponse
                {
                    Success = false,
                    Message = "We couldn't find a FinBine profile for this account."
                };
            }

            if (!string.Equals(account.MemberStatus, "Active", StringComparison.OrdinalIgnoreCase))
            {
                _loginLogger.LogAccountNotActive(account.UserId, account.MemberStatus, ipAddress);
                return new UserLoginResponse
                {
                    Success = false,
                    Message = $"This account is currently {account.MemberStatus.ToLower()}. Please contact support."
                };
            }

            try
            {
                await _firestoreService.UpdateLastActivityAsync(account.UserId);
            }
            catch (Exception ex)
            {
                _loginLogger.LogLastActivityUpdateFailed(account.UserId, ex.Message);
            }

            return new UserLoginResponse
            {
                Success = true,
                Message = "Login successful",
                UserId = account.UserId,
                DisplayName = account.DisplayName,
                AccountType = account.AccountType,
                GroupId = account.GroupId,
                GroupName = account.GroupName
            };
        }
    }
}