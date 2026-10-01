// src/devAdministrator/data/adminTestDataImport.js
//
// Talks to the AdminTestDataImport backend feature — behind
// Admin > Development > Test Data. Uploads a whole JSON file (see
// TestDataPayload on the backend) and writes every group/owner/member
// in it across all 3 stores in one strict, all-or-nothing call.
// Same token convention as adminDatabaseView.js.

import { auth } from '../../firebase/firebaseManager.js'

const API_BASE = 'https://localhost:5001'

async function getAdminToken() {
  const user = auth.currentUser
  if (!user) {
    throw new Error('Not signed in as an admin.')
  }
  return await user.getIdToken()
}

// Sends the already-parsed JSON payload (schemaVersion/source/description/groups)
// for strict validation + import. The backend validates the WHOLE file
// before writing anything — a rejection returns validationErrors (every
// problem found, not just the first) and nothing is written.
export async function importTestData(payload) {
  const token = await getAdminToken()

  const response = await fetch(`${API_BASE}/api/admin/dev/test-data/import`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token, payload })
  })

  const data = await response.json().catch(() => ({}))
  return data
}

// Deletes every user row across Firebase Auth + Firestore + Postgres
// EXCEPT excludeEmail. The backend also hardcodes its own protected
// list server-side regardless of what's sent here — this is belt and
// suspenders, not the only safeguard. Does not touch groups.
export async function clearAllTestData(excludeEmail) {
  const token = await getAdminToken()

  const response = await fetch(`${API_BASE}/api/admin/dev/test-data/clear-all`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ token, excludeEmail })
  })

  const data = await response.json().catch(() => ({}))
  return data
}