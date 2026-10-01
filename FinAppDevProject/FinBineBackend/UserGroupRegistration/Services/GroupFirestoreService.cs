using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using FinBineBackend.UserAccRegistration.Models;

namespace FinBineBackend.UserGroupRegistration.Services
{
    // Touches two Firestore collections — "fb_groups" (new) and
    // "fb_users" (existing, owned by UserAccRegistration) — kept here
    // rather than split across folders, so this feature's Firestore
    // logic lives entirely in one place per the project's folder
    // convention. Mirrors UserFirestoreService/UserFirestoreLoginService
    // for connection setup and ID generation.
    public class GroupFirestoreService
    {
        private const string CredentialsPath = "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json";
        private const string GroupsCollectionName = "fb_groups";
        private const string UsersCollectionName = "fb_users";
        private const string CountersCollectionName = "_counters";
        private const string GroupsCounterDocId = "fb_groups";

        private readonly FirestoreDb _firestoreDb;

        public GroupFirestoreService()
        {
            var credential = CredentialFactory.FromFile(CredentialsPath, "service_account");
            var projectId = ReadProjectIdFromCredentialsFile(CredentialsPath);

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                GoogleCredential = credential
            }.Build();
        }

        // Looks up the fb_users document for whoever is creating the
        // group, by Firebase UID — same query shape as
        // UserFirestoreLoginService.FindUserByFirebaseUidAsync.
        public async Task<FirestoreUserAccount?> FindOwnerByFirebaseUidAsync(string firebaseUid)
        {
            Query query = _firestoreDb
                .Collection(UsersCollectionName)
                .WhereEqualTo("firebase_uid", firebaseUid)
                .Limit(1);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;

            return snapshot.Documents[0].ConvertTo<FirestoreUserAccount>();
        }

        // Atomically reserves the next fb_group_###### ID. Same
        // transaction pattern as
        // UserFirestoreService.GenerateNextUserIdAsync — safe even if
        // two groups are created at the exact same moment.
        public async Task<string> GenerateNextGroupIdAsync()
        {
            DocumentReference counterRef = _firestoreDb
                .Collection(CountersCollectionName)
                .Document(GroupsCounterDocId);

            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                DocumentSnapshot snapshot = await transaction.GetSnapshotAsync(counterRef);

                long nextSequence = 1;
                if (snapshot.Exists && snapshot.TryGetValue("lastSequence", out long current))
                {
                    nextSequence = current + 1;
                }

                transaction.Set(
                    counterRef,
                    new Dictionary<string, object> { { "lastSequence", nextSequence } },
                    SetOptions.MergeAll
                );

                return $"fb_group_{nextSequence:D6}";
            });
        }

        public async Task CreateGroupDocumentAsync(string groupId, Models.FirestoreGroupAccount group)
        {
            DocumentReference docRef = _firestoreDb.Collection(GroupsCollectionName).Document(groupId);
            await docRef.SetAsync(group);
        }

        // Rollback-only — removes a group document created moments ago,
        // when the following step (assigning the owner) fails.
        public async Task DeleteGroupDocumentAsync(string groupId)
        {
            DocumentReference docRef = _firestoreDb.Collection(GroupsCollectionName).Document(groupId);
            await docRef.DeleteAsync();
        }

        // Marks the creator as the owner of their new group on their
        // own fb_users document, upgrades their account_type to match
        // the group's type, and sets group_status to Active — an owner
        // is a confirmed member of their own group immediately, no
        // approval step (unlike a join request, which lands on
        // Pending).
        public async Task AssignOwnerToGroupAsync(string ownerUserId, string groupId, string groupName, string groupType)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(ownerUserId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "group_id", groupId },
                { "group_name", groupName },
                { "is_owner", true },
                { "account_type", groupType },
                { "group_status", GroupMembershipStatus.Active }
            });
        }

        // Rollback-only — undoes AssignOwnerToGroupAsync if a later
        // step fails after it already ran. previousAccountType and
        // previousGroupStatus restore whatever the owner had before
        // group creation started (normally "Free"/None, but could be a
        // stale Suspended/Pending status if they had one).
        public async Task RevertOwnerGroupAssignmentAsync(string ownerUserId, string previousAccountType, string previousGroupStatus)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(ownerUserId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "group_id", null! },
                { "group_name", null! },
                { "is_owner", false },
                { "account_type", previousAccountType },
                { "group_status", previousGroupStatus }
            });
        }

        // Looks up a group by its full document ID (e.g. "fb_group_000006")
        // — used to confirm a group actually exists before a join request
        // is accepted, and to read its name/type for the requester's
        // fb_users document.
        public async Task<Models.FirestoreGroupAccount?> FindGroupByIdAsync(string groupId)
        {
            DocumentReference docRef = _firestoreDb.Collection(GroupsCollectionName).Document(groupId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            if (!snapshot.Exists) return null;

            return snapshot.ConvertTo<Models.FirestoreGroupAccount>();
        }

        // Used by the admin Database View page's "Firestore Groups" list
        // — returns every fb_groups document, unfiltered. Same idea as
        // UserFirestoreService.GetAllUsersAsync.
        public async Task<List<Models.FirestoreGroupAccount>> GetAllGroupsAsync()
        {
            QuerySnapshot snapshot = await _firestoreDb.Collection(GroupsCollectionName).GetSnapshotAsync();
            return snapshot.Documents.Select(doc => doc.ConvertTo<Models.FirestoreGroupAccount>()).ToList();
        }

        // Records a join request on the requester's own fb_users
        // document — group_id/group_name point at the target group and
        // group_status moves to Pending. Deliberately does NOT touch
        // is_owner or account_type: those only change on approval (not
        // built yet), never at request time.
        public async Task SubmitJoinRequestAsync(string requesterUserId, string groupId, string groupName)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(requesterUserId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "group_id", groupId },
                { "group_name", groupName },
                { "group_status", GroupMembershipStatus.Pending }
            });
        }

        // Rollback-only — undoes SubmitJoinRequestAsync if the following
        // Postgres write fails. Restores exactly whatever the requester's
        // group fields were before the request (null/null/None in the
        // common case, or their previous Suspended group's fields if
        // they were requesting to switch groups).
        public async Task RevertJoinRequestAsync(string requesterUserId, string? previousGroupId, string? previousGroupName, string previousGroupStatus)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(requesterUserId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "group_id", previousGroupId! },
                { "group_name", previousGroupName! },
                { "group_status", previousGroupStatus }
            });
        }

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