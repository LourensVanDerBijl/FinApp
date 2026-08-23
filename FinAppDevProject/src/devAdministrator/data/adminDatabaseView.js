// src/devAdministrator/data/adminDatabaseView.js
//
// Talks to the AdminDatabaseView backend feature — the merged
// Firebase Auth + Firestore (fb_users) + Postgres (Users) view behind
// Admin > Development > DbUsers. Every call re-sends the admin's
// current Firebase ID token, same convention as loginAdmin.vue.

import { auth } from '../../firebase/firebaseManager.js'

const API_BASE = 'https://localhost:5001'

async function getAdminToken() {
  const user = auth.currentUser
  if (!user) {
    throw new Error('Not signed in as an admin.')
  }
  return await user.getIdToken()
}

async function postJson(path, body) {
  const response = await fetch(`${API_BASE}${path}`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(body)
  })

  const data = await response.json().catch(() => ({}))
  return { ok: response.ok, data }
}

// Returns the merged rows — one per person, across all 3 stores.
export async function fetchDbUsers() {
  const token = await getAdminToken()
  const { data } = await postJson('/api/admin/dev/db-users/list', { token })
  return data
}

// Manual/test-data entry — writes a real row into all 3 stores.
export async function addDbUser(fields) {
  const token = await getAdminToken()
  const { data } = await postJson('/api/admin/dev/db-users/add', { token, ...fields })
  return data
}

// Best-effort delete across all 3 stores. Pass whichever id the row
// actually has — userId (fb_user_######) covers Firestore + Postgres,
// firebaseUid covers Firebase Auth-only orphans.
export async function deleteDbUser({ userId, firebaseUid }) {
  const token = await getAdminToken()
  const { data } = await postJson('/api/admin/dev/db-users/delete', {
    token,
    userId: userId || null,
    firebaseUid: firebaseUid || null
  })
  return data
}
