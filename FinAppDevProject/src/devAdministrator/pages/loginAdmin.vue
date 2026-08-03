<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import './loginAdmin.css'

import { auth } from '../../firebase/firebaseManager.js'
import { 
  signInWithEmailAndPassword, 
  GoogleAuthProvider, 
  signInWithPopup 
} from 'firebase/auth'

// Lucide icons (correct package)
import { Shield, Check, Users, Mail, Lock, Eye, AlertCircle } from '@lucide/vue'

const email = ref('')
const password = ref('')
const router = useRouter()

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

async function loginWithGoogle() {
  const provider = new GoogleAuthProvider()
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
      alert(`Google Login failed: ${data.message}`)
    }
  } catch (error) {
    console.error("Google SSO error:", error)
    alert(`Google SSO Error: ${error.message}`)
  }
}
</script>

<template>
  <div class="split-screen">
    <!-- Left Branding Panel -->
    <aside class="branding">
      <div class="branding-content">
        <img src="../../assets/SVG/logo.svg" alt="FinBine Logo" class="brand-logo" />
        <h1 class="brand-title">FinBine</h1>
        <h2 class="brand-subtitle">Enterprise Administration</h2>
        <div class="brand-line"></div>
        <ul class="brand-values">
          <li><Shield class="icon-svg" /> <span>Secure.</span></li>
          <li><Check class="icon-svg" /> <span>Reliable.</span></li>
          <li><Users class="icon-svg" /> <span>Trusted.</span></li>
        </ul>
      </div>
      <footer class="footer">
        © 2026 FinBine · 
        <a href="/privacy">Privacy Policy</a> · 
        <a href="/terms">Terms of Service</a>
      </footer>
      <!-- Watermark -->
      <img src="../../assets/SVG/logo.svg" alt="FinBine Watermark" class="branding-watermark" />
    </aside>

    <!-- Right Login Panel -->
    <main class="login">
      <div class="login-box">
        <!-- Administrator Notice -->
        <div class="security-notice">
          <div class="notice-header">
            <Shield class="icon-svg" />
            <span class="notice-title">Administrator Portal</span>
          </div>
          <p>
            This portal is reserved for authorized FinBine administrators.<br />
            Unauthorized access attempts are monitored, logged, and may be investigated in accordance with applicable laws and FinBine security policies.
          </p>
          <div class="notice-divider"></div>
          <div class="notice-warning">
            <AlertCircle class="icon-svg" />
            <span>Access restricted to administrators only.</span>
          </div>
        </div>

        <!-- Group Member Link (outside card) -->
        <a href="/user-login" class="subtle-link">← Group Member? Go to User Login</a>

        <!-- Email/password form -->
        <form @submit.prevent="handleLogin" class="form">
          <div class="input-group">
            <label for="email">Email</label>
            <div class="input-wrapper">
              <Mail class="input-icon" />
              <input id="email" v-model="email" type="email" placeholder="admin@finbine.com" required />
            </div>
          </div>
          <div class="input-group">
            <div class="password-row">
              <label for="password">Password</label>
              <a href="/forgot-password">Forgot password?</a>
            </div>
            <div class="input-wrapper">
              <Lock class="input-icon" />
              <input id="password" v-model="password" type="password" placeholder="Enter your password" required />
              <Eye class="input-eye" />
            </div>
          </div>
          <button type="submit" class="login-button">
            <Lock class="icon-svg" /> Login
          </button>
        </form>

        <!-- Divider -->
        <div class="divider">
          <span class="line"></span>
          <span class="or">OR</span>
          <span class="line"></span>
        </div>

        <!-- Google Sign-In -->
        <button @click="loginWithGoogle" class="google-button">
          <img src="../../assets/SVG/svgGoogle.svg" alt="Google" /> Sign in with Google
        </button>

        <!-- Help link -->
        <div class="help">
          <p>Need Help?</p>
          <a href="/help">Contact Support</a>
        </div>
      </div>
    </main>
  </div>
</template>
