// src/devAdministrator/data/adminSession.js
//
// Firebase tracks WHO is logged in. This tracks the actual FinBine
// admin profile (display name, role) that admin pages need to show.
// Re-fetched fresh from the backend on every protected admin page
// visit — reuses POST /api/admin/login, since that already verifies
// the token and returns exactly this profile data (same pattern
// userSession.js uses for the member/user side).
import { ref } from 'vue'
import { onAuthStateChanged } from 'firebase/auth'
import { auth } from '../../firebase/firebaseManager.js'

const API_BASE = 'https://localhost:5001'

// Shape returned by POST /api/admin/login (see AdminLoginResponse.cs):
// { success, message, preferName, surname, accountType }
// There is no separate "role" field — accountType (e.g. "Super Admin")
// is the closest equivalent.
export const currentAdminProfile = ref(null)

// auth.currentUser is null for a brief moment on a fresh page load,
// while Firebase is still restoring the persisted session. Reading
// it immediately — before that restore finishes — reads as "nobody's
// signed in" even when an admin is. Waiting for onAuthStateChanged to
// fire once avoids that race. (This is the same fix applied to
// getAdminToken() in mockData.js.)
function waitForAuthReady() {
  return new Promise((resolve) => {
    const unsubscribe = onAuthStateChanged(auth, (user) => {
      unsubscribe()
      resolve(user)
    })
  })
}

export async function loadCurrentAdminProfile() {
  const user = auth.currentUser ?? await waitForAuthReady()
  if (!user) {
    currentAdminProfile.value = null
    return false
  }

  try {
    const token = await user.getIdToken()

    const response = await fetch(`${API_BASE}/api/admin/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const result = await response.json()

    if (!result.success) {
      currentAdminProfile.value = null
      return false
    }

    currentAdminProfile.value = result
    return true
  } catch (err) {
    console.error('Error loading admin profile:', err)
    currentAdminProfile.value = null
    return false
  }
}

export function clearAdminProfile() {
  currentAdminProfile.value = null
}
