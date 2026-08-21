// src/devUser/data/userSession.js
//
// Firebase tracks WHO is logged in. This tracks the actual FinBine
// profile (display name, account type, group) that pages need to show.
// Re-fetched fresh from the backend on every protected page visit —
// reuses POST /api/user/login, since that already verifies the token
// and returns exactly this profile data.
import { ref } from 'vue'
import { auth } from '../../firebase/firebaseManager.js'

const API_BASE = 'https://localhost:5001'

export const currentUserProfile = ref(null)

export async function loadCurrentUserProfile() {
  const user = auth.currentUser
  if (!user) {
    currentUserProfile.value = null
    return false
  }

  try {
    const token = await user.getIdToken()

    const response = await fetch(`${API_BASE}/api/user/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const result = await response.json()

    if (!result.success) {
      currentUserProfile.value = null
      return false
    }

    currentUserProfile.value = result
    return true
  } catch (err) {
    console.error('Error loading user profile:', err)
    currentUserProfile.value = null
    return false
  }
}

export function clearUserProfile() {
  currentUserProfile.value = null
}