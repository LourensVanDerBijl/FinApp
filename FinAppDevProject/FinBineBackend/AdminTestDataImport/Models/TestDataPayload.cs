namespace FinBineBackend.AdminTestDataImport.Models
{
    // Mirrors the JSON file shape produced for the Test Data tab
    // exactly — field-for-field, strict. If the uploaded file's shape
    // drifts from this (a renamed field, a missing one, an extra one
    // on a required object), System.Text.Json either leaves a
    // property at its default (caught by ValidateAsync's required-field
    // checks) or the whole deserialize fails outright. Either way nothing
    // gets written. See TestDataImportValidator for the "fail loudly"
    // rules layered on top of this shape.
    public class TestDataPayload
    {
        public int SchemaVersion { get; set; }
        public string Source { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<TestDataGroupPayload> Groups { get; set; } = new();
    }

    public class TestDataGroupPayload
    {
        // The label used *inside this file only* (e.g. "TSGroup1") —
        // never written to a database as-is. Real fb_group_###### /
        // fb_user_###### IDs are generated at import time, same as a
        // live group-creation call, and members re-point at whichever
        // real GroupId their owner ended up with.
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        // "Free" or "Premium". Must match Owner.AccountType — an
        // owner's account tier always matches their group's tier, same
        // rule UserGroupRegistrationService enforces live.
        public string GroupType { get; set; } = string.Empty;

        public TestDataUserPayload Owner { get; set; } = new();
        public List<TestDataUserPayload> Members { get; set; } = new();
    }

    public class TestDataUserPayload
    {
        // Label only, for error messages — never written anywhere.
        public string TestId { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        // "Free" or "Premium".
        public string AccountType { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public string Timezone { get; set; } = string.Empty;

        // "yyyy-MM-dd".
        public string DateOfBirth { get; set; } = string.Empty;

        // Must equal the parent group's GroupId/GroupName exactly —
        // redundant with nesting, but validated anyway since this file
        // is hand-edited/regenerated and could drift.
        public string GroupId { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;

        // One of GroupMembershipStatus.{None,Pending,Active,Suspended,Terminated}.
        // The owner MUST be "Active" — no other value is accepted for
        // Owner (see the GroupStatus bug this data set exists to catch).
        public string GroupStatus { get; set; } = string.Empty;

        public bool IsOwner { get; set; }
    }
}
