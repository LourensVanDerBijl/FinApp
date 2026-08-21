namespace FinBineBackend.UserAccRegistration.Models
{
    public class RegisterUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserId { get; set; }

        // Lets the frontend tell "this account already exists" apart
        // from any other failure, without parsing the message text.
        // Only ever set to "AccountAlreadyExists" — null otherwise.
        public string? ErrorCode { get; set; }
    }
}