namespace FinBineBackend.UserGroupRegistration.Models
{
    public class CreateGroupRequest
    {
        // The creator's current Firebase ID token — re-verified here so
        // group creation can't happen on a stale/expired session, even
        // if the frontend route guard already checked once.
        public string Token { get; set; } = string.Empty;

        public string GroupName { get; set; } = string.Empty;

        // "Premium" or "Free".
        public string GroupType { get; set; } = string.Empty;

        // Deliberately NOT included: OwnerUserID, CountryCode,
        // CurrencyCode, TimeZone. These all come from the owner's own
        // fb_users profile (looked up via the token's Firebase UID) —
        // accepting them from the request would let someone submit
        // fabricated values. Same reasoning as RegisterUserRequest
        // leaving AccountType out.
    }
}
