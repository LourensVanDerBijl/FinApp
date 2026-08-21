namespace FinBineBackend.UserAccRegistration.Models
{
    public class RegisterUserSSORequest
    {
        // The Firebase ID token handed back after the person completed
        // the real Google/Microsoft/Yahoo popup on the frontend.
        public string Token { get; set; } = string.Empty;

        // "Google", "Microsoft", or "Yahoo" — used for the SignInMethod
        // field and log messages.
        public string Provider { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

        // Used only as a fallback if the token itself doesn't carry an
        // email claim — the token's own email is treated as the source
        // of truth whenever it's available.
        public string Email { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;
    }
}