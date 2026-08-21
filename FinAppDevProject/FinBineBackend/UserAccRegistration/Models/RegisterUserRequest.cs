namespace FinBineBackend.UserAccRegistration.Models
{
    public class RegisterUserRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;

        // Deliberately NOT included: AccountType. Registration always
        // creates a Free account — accepting that from the request would
        // let someone tamper with it to claim Premium for free. Upgrading
        // is a separate, not-yet-built feature.
    }
}