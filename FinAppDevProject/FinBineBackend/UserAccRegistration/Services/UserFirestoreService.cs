using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using FinBineBackend.UserAccRegistration.Models;

namespace FinBineBackend.UserAccRegistration.Services
{
    public class UserFirestoreService
    {
        private const string CredentialsPath = "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json";
        private const string UsersCollectionName = "fb_users";
        private const string CountersCollectionName = "_counters";
        private const string UsersCounterDocId = "fb_users";

        private readonly FirestoreDb _firestoreDb;

        public UserFirestoreService()
        {
            var credential = CredentialFactory.FromFile(CredentialsPath, "service_account");
            var projectId = ReadProjectIdFromCredentialsFile(CredentialsPath);

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                GoogleCredential = credential
            }.Build();
        }

        // Atomically reserves the next fb_user_###### ID. Safe even if
        // two people register at the exact same moment — Firestore
        // retries the transaction automatically on a collision, so the
        // same ID can never be handed out twice.
        public async Task<string> GenerateNextUserIdAsync()
        {
            DocumentReference counterRef = _firestoreDb
                .Collection(CountersCollectionName)
                .Document(UsersCounterDocId);

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

                return $"fb_user_{nextSequence:D6}";
            });
        }

        // Writes the actual fb_users document, using the ID already
        // reserved by GenerateNextUserIdAsync.
        public async Task CreateUserDocumentAsync(string userId, FirestoreUserAccount account)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(userId);
            await docRef.SetAsync(account);
        }

        // Used only for rollback — removes a document that was just
        // created moments ago, when a later registration step failed.
        public async Task DeleteUserDocumentAsync(string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(userId);
            await docRef.DeleteAsync();
        }

        // Used by SSO registration to guard against creating a second
        // profile for a Firebase account that already has one — e.g. if
        // someone double-clicks, or retries after a slow response.
        public async Task<FirestoreUserAccount?> FindUserByFirebaseUidAsync(string uid)
        {
            Query query = _firestoreDb
                .Collection(UsersCollectionName)
                .WhereEqualTo("firebase_uid", uid)
                .Limit(1);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            if (snapshot.Count == 0)
            {
                return null;
            }

            return snapshot.Documents[0].ConvertTo<FirestoreUserAccount>();
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