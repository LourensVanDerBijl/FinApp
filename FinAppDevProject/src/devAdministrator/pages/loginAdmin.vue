<script setup>
import { ref } from 'vue'
import './loginAdmin.css'   // <-- import external stylesheet

const email = ref('')
const password = ref('')

function handleLogin() {
  console.log(`Admin login attempt: ${email.value} / ${password.value}`)
  alert(`Logging in as admin: ${email.value}`)
}

function loginWithSSO() {
  alert("Single Sign-On login coming soon...")
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

      <!-- Footer now lives in the sidebar -->
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
        <!-- Security notice -->
        <div class="security-notice">
          <p>
            <strong>Restricted Access:</strong> Authorized administrators only.
            Unauthorized login attempts will be logged and may lead to disciplinary or legal action.
          </p>
        </div>

        <!-- Form -->
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

        <!-- Single SSO button -->
        <div class="sso">
          <button @click="loginWithSSO" class="sso-button">
            Sign in with Company Account (SSO)
          </button>
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

</style>
