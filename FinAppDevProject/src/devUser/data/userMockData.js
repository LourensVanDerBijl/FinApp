// src/devUser/data/userMockData.js
//
// Central place for every API call the user-facing pages need — pages
// call functions from here, never fetch() directly.

import { auth } from '../../firebase/firebaseManager.js'
import {
  GoogleAuthProvider,
  OAuthProvider,
  signInWithPopup,
  signInWithEmailAndPassword,
  sendPasswordResetEmail,
  signOut
} from 'firebase/auth'

const API_BASE = 'https://localhost:5001'
const REQUEST_TIMEOUT_MS = 15000

async function fetchWithTimeout(url, options) {
  const controller = new AbortController()
  const timeoutId = setTimeout(() => controller.abort(), REQUEST_TIMEOUT_MS)

  try {
    return await fetch(url, { ...options, signal: controller.signal })
  } finally {
    clearTimeout(timeoutId)
  }
}

// Registers a new user with Email + Password.
// The backend creates the Firebase account itself (no token needed,
// since no popup is involved) and writes fb_users + Postgres. Once that
// succeeds, we trigger Firebase's own password-reset email from here,
// so the person can set their real password.
export async function registerWithEmail(formData) {
  try {
    const response = await fetchWithTimeout(`${API_BASE}/api/user/registration/email`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(formData)
    })

    const result = await response.json()

    if (!result.success) {
      return { success: false, message: result.message, errorCode: result.errorCode || null }
    }

        // Account exists now — try to get Firebase to send the "set your
    // password" email. If just this step fails, the account is still
    // real and usable later, but the person has no way to set a
    // password yet — flag that distinctly rather than claim success.
    try {
      await sendPasswordResetEmail(auth, formData.email)
      return { success: true, emailSent: true, message: result.message }
    } catch (emailErr) {
      console.error('Account created, but the password-reset email failed to send:', emailErr)
      return {
        success: true,
        emailSent: false,
        message: 'Account created, but we could not send the password email. Please contact support.'
      }
    }
  } catch (err) {
    console.error('Error registering with email:', err)
    return { success: false, message: 'Registration failed. Please try again.', errorCode: null }
  }
}

// Registers a new user via SSO (Google, Microsoft, or Yahoo).
export async function registerWithSSO(providerName, formData) {
  let provider
  switch (providerName) {
    case 'Google':
      provider = new GoogleAuthProvider()
      break
    case 'Microsoft':
      provider = new OAuthProvider('microsoft.com')
      break
    case 'Yahoo':
      provider = new OAuthProvider('yahoo.com')
      break
    default:
      return { success: false, message: 'Unsupported sign-in method.', errorCode: null }
  }

  let credential
  try {
    credential = await signInWithPopup(auth, provider)
  } catch (err) {
    if (err.code === 'auth/popup-closed-by-user') {
      return { success: false, message: 'Sign-in was cancelled.', errorCode: null }
    }
    console.error('SSO popup error:', err)
    return { success: false, message: 'Sign-in failed. Please try again.', errorCode: null }
  }

  try {
    const token = await credential.user.getIdToken()

    const response = await fetchWithTimeout(`${API_BASE}/api/user/registration/sso`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        token,
        provider: providerName,
        ...formData
      })
    })

    const result = await response.json()

    if (!result.success) {
      // Backend already rolled back its own side — keep the browser's
      // session in sync by signing out here too.
      await signOut(auth)
      return { success: false, message: result.message, errorCode: result.errorCode || null }
    }

    return { success: true, message: result.message }
  } catch (err) {
    console.error('Error registering with SSO:', err)
    await signOut(auth)
    return { success: false, message: 'Registration failed. Please try again.', errorCode: null }
  }
}
// Logs an existing user in with Email + Password. Only handles the
// actual Firebase sign-in — call loadCurrentUserProfile() from
// userSession.js afterward to get the FinBine profile (group, etc.).
export async function loginWithEmail(email, password) {
  try {
    await signInWithEmailAndPassword(auth, email, password)
    return { success: true }
  } catch (err) {
    console.error('Email login error:', err)
    let message = 'Invalid email or password. Please try again.'
    if (err.code === 'auth/too-many-requests') {
      message = 'Too many attempts. Please wait a moment and try again.'
    }
    return { success: false, message }
  }
}

// Logs an existing user in via SSO (Google, Microsoft, or Yahoo).
export async function loginWithSSO(providerName) {
  let provider
  switch (providerName) {
    case 'Google':
      provider = new GoogleAuthProvider()
      break
    case 'Microsoft':
      provider = new OAuthProvider('microsoft.com')
      break
    case 'Yahoo':
      provider = new OAuthProvider('yahoo.com')
      break
    default:
      return { success: false, message: 'Unsupported sign-in method.' }
  }

  try {
    await signInWithPopup(auth, provider)
    return { success: true }
  } catch (err) {
    if (err.code === 'auth/popup-closed-by-user') {
      return { success: false, message: 'Sign-in was cancelled.' }
    }
    console.error('SSO login error:', err)
    return { success: false, message: 'Sign-in failed. Please try again.' }
  }
}

// Creates a group for the currently signed-in user (owner). Re-fetches
// a fresh ID token here rather than trusting one passed in, since the
// backend re-verifies it anyway (see UserGroupRegistrationService) —
// no point sending a token that might be about to expire.
// groupType must be 'Premium' or 'Free'.
export async function createGroup(groupName, groupType) {
  const user = auth.currentUser
  if (!user) {
    return { success: false, message: 'Your session could not be verified. Please sign in again.' }
  }

  try {
    const token = await user.getIdToken()

    const response = await fetchWithTimeout(`${API_BASE}/api/user/group-registration`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token, groupName, groupType })
    })

    const result = await response.json()

    if (!result.success) {
      return { success: false, message: result.message }
    }

    return { success: true, message: result.message, groupId: result.groupId, groupName: result.groupName }
  } catch (err) {
    console.error('Error creating group:', err)
    return { success: false, message: 'Group creation failed. Please try again.' }
  }
}