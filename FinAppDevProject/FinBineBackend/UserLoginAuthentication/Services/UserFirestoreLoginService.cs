using System.Text.Json;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using FinBineBackend.UserAccRegistration.Models;

namespace FinBineBackend.UserLoginAuthentication.Services
{
    public class UserFirestoreLoginService
    {
        private const string CredentialsPath = "finbineadmin-firebase-adminsdk-fbsvc-ed0487c71b.json";
        private const string UsersCollectionName = "fb_users";

        private readonly FirestoreDb _firestoreDb;

        public UserFirestoreLoginService()
        {
            var credential = CredentialFactory.FromFile(CredentialsPath, "service_account");
            var projectId = ReadProjectIdFromCredentialsFile(CredentialsPath);

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                GoogleCredential = credential
            }.Build();
        }

        public async Task<FirestoreUserAccount?> FindUserByFirebaseUidAsync(string uid)
        {
            Query query = _firestoreDb
                .Collection(UsersCollectionName)
                .WhereEqualTo("firebase_uid", uid)
                .Limit(1);

            QuerySnapshot snapshot = await query.GetSnapshotAsync();
            if (snapshot.Count == 0) return null;

            return snapshot.Documents[0].ConvertTo<FirestoreUserAccount>();
        }

        // Best-effort — called only after a successful login. If this
        // write fails, the login itself still succeeds; see
        // UserLoginAuthService for how that failure gets handled.
        public async Task UpdateLastActivityAsync(string userId)
        {
            DocumentReference docRef = _firestoreDb.Collection(UsersCollectionName).Document(userId);
            await docRef.UpdateAsync("last_activity", DateTime.UtcNow.ToString("o"));
        }

        private static string ReadProjectIdFromCredentialsFile(string credentialsPath)
        {
            using var stream = File.OpenRead(credentialsPath);
            using var document = JsonDocument.Parse(stream);
            return document.RootElement.GetProperty("project_id").GetString()
                ?? throw new InvalidOperationException("Could not find 'project_id' in the Firebase service account file.");
        }
    }
}