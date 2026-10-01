<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Two entry points into one page, matching how Firebase's password-reset
// flow actually works:
//
//   1. No oobCode in the URL  -> "request" step. Person enters their
//      email, we call sendPasswordResetEmail. Firebase emails them a
//      link back to THIS route with ?mode=resetPassword&oobCode=...
//   2. oobCode present in the URL -> we verify it (verifyPasswordResetCode)
//      and, if valid, show the "set a new password" step. Submitting
//      calls confirmPasswordReset, then we show a success screen with a
//      link back to FinBine login.
//
// Firebase owns the mechanics (link validity, the actual reset); this
// page only owns how it looks and reads to the person using it — no
// step here ever talks to the FinBine backend, since Firebase is the
// sole source of truth for a password.
// ─────────────────────────────────────────────────────────────────────────
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { auth } from '../../../firebase/firebaseManager.js'
import {
  sendPasswordResetEmail,
  verifyPasswordResetCode,
  confirmPasswordReset
} from 'firebase/auth'
import logo from '../../../assets/SVG/logo.svg'
import {
  ShieldCheck,
  Lock,
  Mail,
  Eye,
  EyeOff,
  AlertCircle,
  CheckCircle2,
  ArrowLeft,
  Loader2
} from 'lucide-vue-next'

const route = useRoute()
const router = useRouter()

// 'request' | 'verifying' | 'invalid' | 'reset' | 'success'
const step = ref('request')
const isSubmitting = ref(false)
const errorMessage = ref('')

// Step 1 — request a link
const requestEmail = ref('')
const requestSent = ref(false)

// Step 2 — set a new password
const oobCode = ref('')
const resetForEmail = ref('')
const newPassword = ref('')
const confirmPassword = ref('')
const passwordVisible = ref(false)
const confirmVisible = ref(false)

onMounted(async () => {
  const code = route.query.oobCode
  const mode = route.query.mode

  if (!code || mode !== 'resetPassword') {
    step.value = 'request'
    return
  }

  oobCode.value = code
  step.value = 'verifying'

  try {
    // Confirms the link is real, unexpired, and unused — and tells us
    // which account it's actually for, so the form can say so.
    resetForEmail.value = await verifyPasswordResetCode(auth, code)
    step.value = 'reset'
  } catch (err) {
    console.error('Password reset link verification failed:', err)
    step.value = 'invalid'
  }
})

function togglePasswordVisibility() {
  passwordVisible.value = !passwordVisible.value
}

function toggleConfirmVisibility() {
  confirmVisible.value = !confirmVisible.value
}

async function handleRequestLink() {
  errorMessage.value = ''

  if (!requestEmail.value.trim()) {
    errorMessage.value = 'Please enter your email address.'
    return
  }

  isSubmitting.value = true

  try {
    await sendPasswordResetEmail(auth, requestEmail.value.trim())
    requestSent.value = true
  } catch (err) {
    console.error('Error sending password reset email:', err)
    // Deliberately vague on "does this email exist" — same reasoning
    // Firebase itself uses, so this doesn't become a way to check
    // whether an email is registered with FinBine.
    if (err.code === 'auth/invalid-email') {
      errorMessage.value = 'That doesn\'t look like a valid email address.'
    } else if (err.code === 'auth/too-many-requests') {
      errorMessage.value = 'Too many attempts. Please wait a moment and try again.'
    } else {
      requestSent.value = true
    }
  } finally {
    isSubmitting.value = false
  }
}

function resetToRequestStep() {
  step.value = 'request'
  requestSent.value = false
  errorMessage.value = ''
  requestEmail.value = ''
}

async function handleSetNewPassword() {
  errorMessage.value = ''

  if (!newPassword.value || !confirmPassword.value) {
    errorMessage.value = 'Please fill in both password fields.'
    return
  }

  if (newPassword.value.length < 8) {
    errorMessage.value = 'Your password needs to be at least 8 characters.'
    return
  }

  if (newPassword.value !== confirmPassword.value) {
    errorMessage.value = 'Those passwords don\'t match.'
    return
  }

  isSubmitting.value = true

  try {
    await confirmPasswordReset(auth, oobCode.value, newPassword.value)
    step.value = 'success'
  } catch (err) {
    console.error('Error confirming password reset:', err)
    if (err.code === 'auth/expired-action-code' || err.code === 'auth/invalid-action-code') {
      step.value = 'invalid'
    } else if (err.code === 'auth/weak-password') {
      errorMessage.value = 'Please choose a stronger password.'
    } else {
      errorMessage.value = 'Something went wrong. Please try again.'
    }
  } finally {
    isSubmitting.value = false
  }
}

function goToLogin() {
  router.push('/user/login')
}
</script>

<template>
  <div class="reset-page">
    <div class="reset-card">
      <!-- ============================ BRAND PANEL ============================ -->
      <aside class="brand-panel">
        <div class="brand-decor" aria-hidden="true"></div>

        <div class="brand-content">
          <div class="brand-row">
            <img :src="logo" alt="FinBine Logo" class="brand-logo" />
            <span class="brand-wordmark">FinBine</span>
          </div>

          <div class="brand-copy">
            <h1>Reset your password</h1>
            <p>
              It happens to the best of us. Enter your email and we'll send you a link to get
              back into your account.
            </p>
          </div>

          <div class="brand-divider"></div>

          <p class="brand-tagline">
            <Lock :size="13" />
            <span>Your finances. A brighter together.</span>
          </p>
        </div>

        <p class="brand-copyright">© {{ new Date().getFullYear() }} FinBine. All rights reserved.</p>
      </aside>

      <!-- ============================= FORM PANEL ============================= -->
      <section class="form-panel">
        <div class="form-inner">
          <div class="mobile-brand-row">
            <img :src="logo" alt="FinBine Logo" class="mobile-brand-logo" />
            <span class="mobile-brand-wordmark">FinBine</span>
          </div>

          <!-- ── STEP: request a reset link ── -->
          <template v-if="step === 'request' && !requestSent">
            <h2>Forgot your password?</h2>
            <p class="panel-sub">
              Enter the email address on your FinBine account and we'll send you a link to reset
              it.
            </p>

            <p v-if="errorMessage" class="reset-error">
              <AlertCircle :size="15" />
              <span>{{ errorMessage }}</span>
            </p>

            <div class="field">
              <div class="input-wrap">
                <Mail :size="16" class="input-icon" />
                <input
                  type="email"
                  v-model="requestEmail"
                  placeholder="Email address"
                  @keyup.enter="handleRequestLink"
                />
              </div>
            </div>

            <button type="button" class="primary-btn" :disabled="isSubmitting" @click="handleRequestLink">
              <span v-if="isSubmitting" class="btn-icon spin"><Loader2 :size="16" /></span>
              <span v-else class="btn-icon"><Mail :size="16" /></span>
              {{ isSubmitting ? 'Sending...' : 'Send reset link' }}
            </button>

            <a href="#" class="back-to-login" @click.prevent="goToLogin">
              <ArrowLeft :size="14" />
              <span>Back to sign in</span>
            </a>
          </template>

          <!-- ── STEP: link sent confirmation ── -->
          <template v-else-if="step === 'request' && requestSent">
            <div class="status-icon success-icon">
              <Mail :size="26" />
            </div>
            <h2>Check your email</h2>
            <p class="panel-sub">
              If an account exists for <strong>{{ requestEmail }}</strong>, we've sent a link to
              reset your password. It should arrive within a few minutes.
            </p>

            <button type="button" class="secondary-btn" @click="resetToRequestStep">
              Use a different email
            </button>

            <a href="#" class="back-to-login" @click.prevent="goToLogin">
              <ArrowLeft :size="14" />
              <span>Back to sign in</span>
            </a>
          </template>

          <!-- ── STEP: verifying the link ── -->
          <template v-else-if="step === 'verifying'">
            <div class="status-icon">
              <Loader2 :size="26" class="spin" />
            </div>
            <h2>Verifying your link...</h2>
            <p class="panel-sub">This will only take a moment.</p>
          </template>

          <!-- ── STEP: invalid/expired link ── -->
          <template v-else-if="step === 'invalid'">
            <div class="status-icon error-icon">
              <AlertCircle :size="26" />
            </div>
            <h2>This link has expired</h2>
            <p class="panel-sub">
              Password reset links are only valid for a limited time, and can only be used once.
              Request a new one below.
            </p>

            <button type="button" class="primary-btn" @click="resetToRequestStep">
              <span class="btn-icon"><Mail :size="16" /></span>
              Request a new link
            </button>

            <a href="#" class="back-to-login" @click.prevent="goToLogin">
              <ArrowLeft :size="14" />
              <span>Back to sign in</span>
            </a>
          </template>

          <!-- ── STEP: set a new password ── -->
          <template v-else-if="step === 'reset'">
            <h2>Set a new password</h2>
            <p class="panel-sub">
              Choose a new password for
              <strong v-if="resetForEmail">{{ resetForEmail }}</strong>
              <span v-else>your account</span>.
            </p>

            <p v-if="errorMessage" class="reset-error">
              <AlertCircle :size="15" />
              <span>{{ errorMessage }}</span>
            </p>

            <div class="field">
              <div class="input-wrap">
                <Lock :size="16" class="input-icon" />
                <input
                  :type="passwordVisible ? 'text' : 'password'"
                  v-model="newPassword"
                  placeholder="New password"
                />
                <button
                  type="button"
                  class="visibility-toggle"
                  @click="togglePasswordVisibility"
                  aria-label="Toggle password visibility"
                >
                  <EyeOff v-if="passwordVisible" :size="16" />
                  <Eye v-else :size="16" />
                </button>
              </div>
            </div>

            <div class="field">
              <div class="input-wrap">
                <Lock :size="16" class="input-icon" />
                <input
                  :type="confirmVisible ? 'text' : 'password'"
                  v-model="confirmPassword"
                  placeholder="Confirm new password"
                  @keyup.enter="handleSetNewPassword"
                />
                <button
                  type="button"
                  class="visibility-toggle"
                  @click="toggleConfirmVisibility"
                  aria-label="Toggle password visibility"
                >
                  <EyeOff v-if="confirmVisible" :size="16" />
                  <Eye v-else :size="16" />
                </button>
              </div>
            </div>

            <p class="field-hint">Use at least 8 characters.</p>

            <button type="button" class="primary-btn" :disabled="isSubmitting" @click="handleSetNewPassword">
              <span v-if="isSubmitting" class="btn-icon spin"><Loader2 :size="16" /></span>
              <span v-else class="btn-icon"><Lock :size="16" /></span>
              {{ isSubmitting ? 'Saving...' : 'Save new password' }}
            </button>
          </template>

          <!-- ── STEP: success ── -->
          <template v-else-if="step === 'success'">
            <div class="status-icon success-icon">
              <CheckCircle2 :size="26" />
            </div>
            <h2>Password updated</h2>
            <p class="panel-sub">
              Your password has been reset. You can now sign in to FinBine with your new
              password.
            </p>

            <button type="button" class="primary-btn" @click="goToLogin">
              <span class="btn-icon"><ArrowLeft :size="16" /></span>
              Return to FinBine login
            </button>
          </template>

          <p class="terms-row">
            <ShieldCheck :size="14" />
            <span>Your password is managed securely by Firebase and is never seen by FinBine.</span>
          </p>
        </div>
      </section>
    </div>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.reset-page {
  height: 100vh;
  background-color: #0b1220;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
  display: flex;
  overflow: hidden;
}

.reset-card {
  width: 100%;
  height: 100%;
  display: flex;
  background: #fff;
}

/* ============================ BRAND PANEL ============================ */
.brand-panel {
  flex: 0 0 42%;
  position: relative;
  overflow-x: hidden;
  overflow-y: auto;
  background: linear-gradient(165deg, #0d1728 0%, #0b1220 60%, #0a0f1c 100%);
  padding: 48px 44px 36px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  gap: 32px;
}

.brand-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  background:
    radial-gradient(circle at 8% 100%, rgba(79, 142, 247, 0.18) 0%, rgba(79, 142, 247, 0) 55%),
    radial-gradient(circle at 30% 92%, rgba(45, 212, 191, 0.14) 0%, rgba(45, 212, 191, 0) 50%),
    radial-gradient(circle at -5% 70%, rgba(45, 212, 191, 0.1) 0%, rgba(45, 212, 191, 0) 45%);
}

.brand-content {
  position: relative;
  z-index: 1;
}

.brand-row {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 40px;
}

.brand-logo {
  height: 44px;
  width: auto;
}

.brand-wordmark {
  font-size: 1.5rem;
  font-weight: 700;
  color: #fff;
}

.brand-copy h1 {
  font-size: 2.1rem;
  font-weight: 800;
  color: #fff;
  margin: 0 0 14px;
  line-height: 1.2;
}

.brand-copy p {
  font-size: 0.92rem;
  color: #94a3b8;
  line-height: 1.6;
  margin: 0;
}

.brand-divider {
  width: 40px;
  height: 3px;
  border-radius: 2px;
  background: #2dd4bf;
  margin: 28px 0 30px;
}

.brand-tagline {
  display: flex;
  align-items: center;
  gap: 8px;
  width: fit-content;
  margin: 0;
  padding: 10px 16px;
  border-radius: 999px;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.08);
  color: #e2e8f0;
  font-size: 0.78rem;
  font-weight: 600;
}

.brand-tagline svg {
  color: #2dd4bf;
  flex-shrink: 0;
}

.brand-copyright {
  position: relative;
  z-index: 1;
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

/* ============================= FORM PANEL ============================= */
.form-panel {
  flex: 1;
  display: flex;
  flex-direction: column;
  align-items: center;
  overflow-y: auto;
  padding: 48px 56px;
}

.form-inner {
  width: 100%;
  max-width: 420px;
  margin: auto 0;
}

.mobile-brand-row {
  display: none;
  align-items: center;
  justify-content: center;
  gap: 8px;
  margin-bottom: 20px;
}

.mobile-brand-logo {
  height: 26px;
  width: auto;
}

.mobile-brand-wordmark {
  font-size: 1.1rem;
  font-weight: 700;
  color: #0f172a;
}

.form-inner h2 {
  font-size: 1.7rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 10px;
}

.panel-sub {
  font-size: 0.88rem;
  color: #64748b;
  line-height: 1.55;
  margin: 0 0 22px;
}

.panel-sub strong {
  color: #0f172a;
}

.status-icon {
  width: 52px;
  height: 52px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background: #eff6ff;
  color: #1855b9;
  margin-bottom: 18px;
}

.status-icon.success-icon {
  background: #f0fdf4;
  color: #16a34a;
}

.status-icon.error-icon {
  background: #fef2f2;
  color: #dc2626;
}

.reset-error {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  background: #fef2f2;
  border: 1px solid #fecaca;
  border-radius: 8px;
  padding: 10px 13px;
  margin: -8px 0 18px;
  font-size: 0.8rem;
  color: #b91c1c;
  line-height: 1.5;
}

.reset-error svg {
  flex-shrink: 0;
  margin-top: 1px;
}

.field {
  margin-bottom: 16px;
}

.input-wrap {
  position: relative;
  display: flex;
  align-items: center;
}

.input-icon {
  position: absolute;
  left: 12px;
  color: #94a3b8;
  pointer-events: none;
}

.input-wrap input {
  width: 100%;
  padding: 11px 40px 11px 38px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.85rem;
  color: #0f172a;
  background: #fff;
  font-family: inherit;
  outline: none;
  transition: border-color 0.15s;
}

.input-wrap input::placeholder {
  color: #94a3b8;
}

.input-wrap input:focus {
  border-color: #1855b9;
}

.visibility-toggle {
  position: absolute;
  right: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  background: none;
  color: #94a3b8;
  cursor: pointer;
  padding: 4px;
}

.field-hint {
  font-size: 0.78rem;
  color: #94a3b8;
  margin: -8px 0 20px;
}

.primary-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 9px;
  padding: 13px 16px;
  border: none;
  border-radius: 10px;
  background: #0b1220;
  color: #fff;
  font-family: inherit;
  font-size: 0.92rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.15s;
}

.primary-btn:hover {
  background: #14213d;
}

.primary-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.secondary-btn {
  width: 100%;
  padding: 13px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  background: #fff;
  color: #0f172a;
  font-family: inherit;
  font-size: 0.88rem;
  font-weight: 700;
  cursor: pointer;
  transition: border-color 0.15s, background 0.15s;
}

.secondary-btn:hover {
  border-color: #cbd5e1;
  background: #f8fafc;
}

.btn-icon {
  display: flex;
  color: #2dd4bf;
}

.btn-icon.spin,
.status-icon .spin {
  animation: spin 0.9s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.back-to-login {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 6px;
  margin-top: 20px;
  font-size: 0.82rem;
  font-weight: 600;
  color: #1855b9;
  text-decoration: none;
}

.back-to-login:hover {
  text-decoration: underline;
}

.secondary-btn + .back-to-login {
  margin-top: 14px;
}

.terms-row {
  display: flex;
  align-items: flex-start;
  justify-content: center;
  gap: 8px;
  text-align: center;
  font-size: 0.75rem;
  color: #94a3b8;
  line-height: 1.6;
  margin: 28px 0 0;
}

.terms-row svg {
  flex-shrink: 0;
  margin-top: 2px;
  color: #94a3b8;
}

/* ============================================================ */
/* MOBILE                                                        */
/* ============================================================ */
@media (max-width: 900px) {
  .reset-page {
    height: auto;
    min-height: 100vh;
    overflow: visible;
  }

  .reset-card {
    flex-direction: column;
    height: auto;
  }

  .brand-panel {
    flex: none;
    overflow: visible;
    padding: 44px 28px 32px;
  }

  .brand-row {
    margin-bottom: 32px;
  }

  .brand-copy h1 {
    font-size: 1.9rem;
  }

  .brand-tagline {
    display: flex;
  }

  .form-panel {
    overflow: visible;
    padding: 40px 22px 48px;
  }

  .form-inner {
    margin: 0;
  }

  .mobile-brand-row {
    display: flex;
  }

  .form-inner h2 {
    text-align: center;
  }

  .panel-sub {
    text-align: center;
  }

  .status-icon {
    margin-left: auto;
    margin-right: auto;
  }
}
</style>
