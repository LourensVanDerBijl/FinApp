<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import './loginAdmin.css'

// ✅ Firebase imports
import { auth } from '../../firebase/firebaseManager.js'
import { 
  signInWithEmailAndPassword, 
  GoogleAuthProvider, 
  OAuthProvider, 
  signInWithPopup 
} from 'firebase/auth'

const email = ref('')
const password = ref('')
const router = useRouter()

// 🔑 Email/password login
async function handleLogin() {
  try {
    const userCredential = await signInWithEmailAndPassword(auth, email.value, password.value)
    const token = await userCredential.user.getIdToken()

    const response = await fetch('https://localhost:5001/api/admin/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const data = await response.json()
    if (data.success) {
      router.push('/admin/dashboard')
    } else {
      alert(`Login failed: ${data.message}`)
    }
  } catch (error) {
    console.error("Frontend error:", error)
    alert(`Error: ${error.message}`)
  }
}

// 🔑 Generic SSO popup login
async function loginWithProvider(providerName) {
  let provider
  if (providerName === 'google') {
    provider = new GoogleAuthProvider()
  } else if (providerName === 'microsoft') {
    provider = new OAuthProvider('microsoft.com')
  } else if (providerName === 'yahoo') {
    provider = new OAuthProvider('yahoo.com')
  }

  try {
    const result = await signInWithPopup(auth, provider)
    const token = await result.user.getIdToken()

    const response = await fetch('https://localhost:5001/api/admin/login', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const data = await response.json()
    if (data.success) {
      router.push('/admin/dashboard')
    } else {
      alert(`SSO Login failed: ${data.message}`)
    }
  } catch (error) {
    console.error("SSO popup error:", error)
    if (error.code === 'auth/popup-blocked') {
      alert("Popup was blocked. Please allow popups to sign in.")
    } else {
      alert(`SSO Error: ${error.message}`)
    }
  }
}
</script>

<template>
  <div class="split-screen">
    <!-- Sidebar -->
    <aside class="branding">
      <div class="branding-content">
        <h1>FinBine</h1>
        <p>Secure Administrator Portal</p>
      </div>
      <footer class="footer">
        <p>
          &copy; 2026 FinBine · All Rights Reserved · LL Van Der Bijl
          <br />
          <a href="/privacy">Privacy Agreement</a> ·
          <a href="/terms">Terms of Use</a>
        </p>
      </footer>
    </aside>

    <!-- Main Login Area -->
    <main class="login">
      <div class="login-box">
        <div class="security-notice">
          <p>
            <strong>Restricted Access:</strong> Authorized administrators only.
            Unauthorized login attempts will be logged and may lead to disciplinary or legal action.
          </p>
        </div>

        <!-- Email/password form -->
        <form @submit.prevent="handleLogin" class="form">
          <div>
            <label>Email</label>
            <input v-model="email" type="email" placeholder="admin@finbine.com" required />
          </div>
          <div>
            <div class="password-row">
              <label>Password</label>
              <a href="/forgot-password">Forgot password?</a>
            </div>
            <input v-model="password" type="password" placeholder="••••••••" required />
          </div>
          <button type="submit" class="login-button">Login</button>
        </form>

        <!-- SSO dropdown -->
        <div class="sso">
          <label for="sso-select">Sign in with:</label>
          <select id="sso-select" @change="loginWithProvider($event.target.value)">
            <option disabled selected>Select provider</option>
            <option value="google">Google</option>
            <option value="microsoft">Microsoft</option>
            <option value="yahoo">Yahoo</option>
          </select>
        </div>

        <!-- Help link -->
        <div class="help">
          <a href="/help">Need Help?</a>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
/* Scoped styles can stay empty if you’re using loginAdmin.css */
</style>
