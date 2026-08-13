using Google.Cloud.Firestore;

namespace FinBineBackend.AdminAuthentication.Models
{
    // This class describes exactly what a document inside the
    // "fb_admin_users" Firestore collection looks like.
    // The [FirestoreData] / [FirestoreProperty] tags tell Google's
    // library how to match Firestore field names to these properties.
    [FirestoreData]
    public class AdminAccount
    {
        [FirestoreProperty("prefer_name")]
        public string PreferName { get; set; } = string.Empty;

        [FirestoreProperty("surname")]
        public string Surname { get; set; } = string.Empty;

        [FirestoreProperty("account_email")]
        public string AccountEmail { get; set; } = string.Empty;

        [FirestoreProperty("account_type")]
        public string AccountType { get; set; } = string.Empty;

        [FirestoreProperty("nationality")]
        public string Nationality { get; set; } = string.Empty;

        [FirestoreProperty("national_id")]
        public string NationalId { get; set; } = string.Empty;

        [FirestoreProperty("effective_date")]
        public string EffectiveDate { get; set; } = string.Empty;

        [FirestoreProperty("firebase_uid")]
        public string FirebaseUid { get; set; } = string.Empty;

        // Not a Firestore field itself — this gets filled in automatically
        // with the document's ID (e.g. "fb_admin_account_000001") once we
        // read it, so we always know which account this was.
        [FirestoreDocumentId]
        public string AccountId { get; set; } = string.Empty;
    }
}