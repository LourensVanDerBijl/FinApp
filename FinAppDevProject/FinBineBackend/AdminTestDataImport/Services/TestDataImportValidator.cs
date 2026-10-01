using FinBineBackend.AdminTestDataImport.Models;
using FinBineBackend.UserAccRegistration.Models;

namespace FinBineBackend.AdminTestDataImport.Services
{
    // Pure validation, no I/O. Deliberately strict and deliberately
    // exhaustive — collects every problem in the file instead of
    // bailing on the first one, so ImportTestDataAsync can refuse the
    // whole batch with a complete list in a single round trip. Nothing
    // is ever written unless this returns zero errors.
    public static class TestDataImportValidator
    {
        private const int ExpectedSchemaVersion = 1;
        private static readonly string[] AllowedAccountTypes = { "Free", "Premium" };
        private static readonly string[] AllowedGroupStatuses =
        {
            GroupMembershipStatus.None,
            GroupMembershipStatus.Pending,
            GroupMembershipStatus.Active,
            GroupMembershipStatus.Suspended,
            GroupMembershipStatus.Terminated
        };

        public static List<string> Validate(TestDataPayload payload)
        {
            var errors = new List<string>();

            if (payload.SchemaVersion != ExpectedSchemaVersion)
            {
                errors.Add($"schemaVersion is {payload.SchemaVersion}, expected {ExpectedSchemaVersion}. " +
                           "The importer only accepts an exact match — regenerate the file or update the importer.");
                // Everything else below assumes the field names for this
                // version, so there's no point validating further.
                return errors;
            }

            if (payload.Groups.Count == 0)
            {
                errors.Add("groups is empty — nothing to import.");
                return errors;
            }

            var seenGroupIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var seenTestIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < payload.Groups.Count; i++)
            {
                var group = payload.Groups[i];
                string groupLabel = string.IsNullOrWhiteSpace(group.GroupId) ? $"groups[{i}]" : group.GroupId;

                if (string.IsNullOrWhiteSpace(group.GroupId))
                    errors.Add($"groups[{i}]: groupId is required.");
                else if (!seenGroupIds.Add(group.GroupId))
                    errors.Add($"{groupLabel}: duplicate groupId — every group in the file must be unique.");

                if (string.IsNullOrWhiteSpace(group.GroupName))
                    errors.Add($"{groupLabel}: groupName is required.");

                if (!AllowedAccountTypes.Contains(group.GroupType, StringComparer.OrdinalIgnoreCase))
                    errors.Add($"{groupLabel}: groupType must be \"Free\" or \"Premium\", got \"{group.GroupType}\".");

                if (group.Owner == null)
                {
                    errors.Add($"{groupLabel}: owner is required.");
                }
                else
                {
                    ValidateUser(group.Owner, group, groupLabel, isOwnerSlot: true, errors, seenEmails, seenTestIds);
                }

                for (int m = 0; m < group.Members.Count; m++)
                {
                    ValidateUser(group.Members[m], group, $"{groupLabel} member[{m}]", isOwnerSlot: false, errors, seenEmails, seenTestIds);
                }
            }

            return errors;
        }

        private static void ValidateUser(
            TestDataUserPayload user,
            TestDataGroupPayload group,
            string label,
            bool isOwnerSlot,
            List<string> errors,
            HashSet<string> seenEmails,
            HashSet<string> seenTestIds)
        {
            string who = string.IsNullOrWhiteSpace(user.TestId) ? label : $"{label} ({user.TestId})";

            if (string.IsNullOrWhiteSpace(user.TestId))
                errors.Add($"{label}: testId is required.");
            else if (!seenTestIds.Add(user.TestId))
                errors.Add($"{who}: duplicate testId — every user in the file must be unique.");

            if (string.IsNullOrWhiteSpace(user.Email))
                errors.Add($"{who}: email is required.");
            else if (!seenEmails.Add(user.Email))
                errors.Add($"{who}: duplicate email — every user in the file must be unique.");

            if (string.IsNullOrWhiteSpace(user.Password))
                errors.Add($"{who}: password is required.");

            if (string.IsNullOrWhiteSpace(user.DisplayName))
                errors.Add($"{who}: displayName is required.");

            if (string.IsNullOrWhiteSpace(user.FirstName))
                errors.Add($"{who}: firstName is required.");

            if (string.IsNullOrWhiteSpace(user.LastName))
                errors.Add($"{who}: lastName is required.");

            if (!AllowedAccountTypes.Contains(user.AccountType, StringComparer.OrdinalIgnoreCase))
                errors.Add($"{who}: accountType must be \"Free\" or \"Premium\", got \"{user.AccountType}\".");

            if (string.IsNullOrWhiteSpace(user.Country))
                errors.Add($"{who}: country is required.");

            if (string.IsNullOrWhiteSpace(user.Currency))
                errors.Add($"{who}: currency is required.");

            if (string.IsNullOrWhiteSpace(user.Timezone))
                errors.Add($"{who}: timezone is required.");

            if (!DateOnly.TryParse(user.DateOfBirth, out _))
                errors.Add($"{who}: dateOfBirth must be a valid \"yyyy-MM-dd\" date, got \"{user.DateOfBirth}\".");

            if (!string.Equals(user.GroupId, group.GroupId, StringComparison.OrdinalIgnoreCase))
                errors.Add($"{who}: groupId (\"{user.GroupId}\") does not match the parent group's groupId (\"{group.GroupId}\").");

            if (!string.Equals(user.GroupName, group.GroupName, StringComparison.OrdinalIgnoreCase))
                errors.Add($"{who}: groupName (\"{user.GroupName}\") does not match the parent group's groupName (\"{group.GroupName}\").");

            if (!AllowedGroupStatuses.Contains(user.GroupStatus, StringComparer.OrdinalIgnoreCase))
            {
                errors.Add($"{who}: groupStatus must be one of None/Pending/Active/Suspended/Terminated, got \"{user.GroupStatus}\".");
            }
            else if (isOwnerSlot && !string.Equals(user.GroupStatus, GroupMembershipStatus.Active, StringComparison.OrdinalIgnoreCase))
            {
                errors.Add($"{who}: owner's groupStatus must be \"Active\" — an owner is always a confirmed member of their own group.");
            }

            if (isOwnerSlot && !user.IsOwner)
                errors.Add($"{who}: is in the owner slot but isOwner is false.");

            if (!isOwnerSlot && user.IsOwner)
                errors.Add($"{who}: is in the members list but isOwner is true — only the owner slot may be true.");

            if (isOwnerSlot && !string.Equals(user.AccountType, group.GroupType, StringComparison.OrdinalIgnoreCase))
                errors.Add($"{who}: owner's accountType (\"{user.AccountType}\") must match the group's groupType (\"{group.GroupType}\").");
        }
    }
}
