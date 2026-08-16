<script setup>
// ─────────────────────────────────────────────────────────────────────────
// VISUAL / STRUCTURE ONLY.
// No API calls, no validation, no auth logic. The refs below exist purely
// so the page can be clicked through (password visibility, remember me) to
// preview the UI. Wire this up to real sign-in logic separately.
// ─────────────────────────────────────────────────────────────────────────
import { ref } from 'vue'
import logo from '../../../assets/SVG/logo.svg'
import googleLogo from '../../../assets/SVG/svgGoogle.svg'
import {
  ShieldCheck,
  Users,
  BarChart3,
  Lock,
  Mail,
  Eye,
  EyeOff,
  Info,
  ChevronRight
} from 'lucide-vue-next'

const passwordVisible = ref(false)
const rememberMe = ref(false)

const features = [
  {
    icon: ShieldCheck,
    title: 'Secure',
    description: 'Your data is protected with industry-leading security.'
  },
  {
    icon: Users,
    title: 'Collaborative',
    description: 'Work together with your group seamlessly.'
  },
  {
    icon: BarChart3,
    title: 'Insightful',
    description: 'Get real-time insights and make smarter decisions.'
  }
]

const socialMethods = [
  { id: 'google', name: 'Google' },
  { id: 'microsoft', name: 'Microsoft' },
  { id: 'yahoo', name: 'Yahoo' }
]

// No-op placeholders — visual only.
function togglePasswordVisibility() {
  passwordVisible.value = !passwordVisible.value
}

function handleSignIn() {
  // Intentionally does nothing yet.
}
</script>

<template>
  <div class="login-page">
    <div class="login-card">
      <!-- ============================ BRAND PANEL ============================ -->
      <aside class="brand-panel">
        <div class="brand-decor" aria-hidden="true"></div>

        <div class="brand-content">
          <div class="brand-row">
            <img :src="logo" alt="FinBine Logo" class="brand-logo" />
            <span class="brand-wordmark">FinBine</span>
          </div>

          <div class="brand-copy">
            <h1>Welcome back!</h1>
            <p>Sign in to access your account and manage your group's finances.</p>
          </div>

          <div class="brand-divider"></div>

          <ul class="feature-list">
            <li v-for="feature in features" :key="feature.title" class="feature-item">
              <span class="feature-icon">
                <component :is="feature.icon" :size="18" />
              </span>
              <div>
                <h4>{{ feature.title }}</h4>
                <p>{{ feature.description }}</p>
              </div>
            </li>
          </ul>

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

          <h2>Sign in to FinBine</h2>
          <p class="panel-sub">Choose the sign-in method you used when you created your account.</p>

          <div class="info-box">
            <Info :size="16" class="info-icon" />
            <p>
              Use the same method you used to create your account. You can change your sign-in
              method in your account settings.
            </p>
          </div>

          <p class="section-label">Sign in with</p>

          <div class="social-list">
            <button
              v-for="method in socialMethods"
              :key="method.id"
              type="button"
              class="social-btn"
            >
              <span class="social-icon">
                <img v-if="method.id === 'google'" :src="googleLogo" alt="Google" />
                <span v-else-if="method.id === 'microsoft'" class="ms-logo" aria-hidden="true">
                  <i></i><i></i><i></i><i></i>
                </span>
                <span v-else-if="method.id === 'yahoo'" class="yahoo-logo" aria-hidden="true">y!</span>
              </span>
              <span class="social-name">Continue with {{ method.name }}</span>
              <ChevronRight :size="16" class="social-chevron" />
            </button>
          </div>

          <div class="or-divider"><span>OR</span></div>

          <p class="section-label">Sign in with email</p>

          <div class="field">
            <div class="input-wrap">
              <Mail :size="16" class="input-icon" />
              <input type="email" placeholder="Email address" />
            </div>
          </div>

          <div class="field">
            <div class="input-wrap">
              <Lock :size="16" class="input-icon" />
              <input :type="passwordVisible ? 'text' : 'password'" placeholder="Password" />
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

          <div class="field-meta">
            <a href="#" class="forgot-link">Forgot password?</a>
          </div>

          <button type="button" class="signin-btn" @click="handleSignIn">
            <span class="signin-icon"><Lock :size="16" /></span>
            Sign In
          </button>

          <div class="form-footer-row">
            <label class="remember-check">
              <input type="checkbox" v-model="rememberMe" />
              <span>Remember me</span>
            </label>
            <a href="#" class="need-help-link">Need help?</a>
          </div>

          <div class="signup-divider"></div>

          <p class="signup-link">Don't have an account? <a href="#">Get Started</a></p>

          <p class="terms-row">
            <ShieldCheck :size="14" />
            <span>
              By continuing, you agree to FinBine's <a href="#">Terms of Service</a> and
              <a href="#">Privacy Policy</a>.
            </span>
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

.login-page {
  height: 100vh;
  background-color: #0b1220;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
  display: flex;
  overflow: hidden;
}

.login-card {
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

.feature-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 26px;
}

.feature-item {
  display: flex;
  gap: 14px;
}

.feature-icon {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: rgba(45, 212, 191, 0.14);
  color: #2dd4bf;
  display: flex;
  align-items: center;
  justify-content: center;
}

.feature-item h4 {
  font-size: 0.92rem;
  font-weight: 700;
  color: #fff;
  margin: 0 0 4px;
}

.feature-item p {
  font-size: 0.8rem;
  color: #94a3b8;
  line-height: 1.5;
  margin: 0;
}

.brand-tagline {
  display: none;
  align-items: center;
  gap: 8px;
  width: fit-content;
  margin: 30px 0 0;
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
  margin: 0 0 20px;
}

.info-box {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  background: #eff6ff;
  border: 1px solid #dbeafe;
  border-radius: 10px;
  padding: 13px 15px;
  margin-bottom: 22px;
}

.info-box .info-icon {
  color: #1855b9;
  margin-top: 2px;
  flex-shrink: 0;
}

.info-box p {
  font-size: 0.8rem;
  color: #334155;
  line-height: 1.55;
  margin: 0;
}

.section-label {
  font-size: 0.82rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 10px;
}

.social-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-bottom: 20px;
}

.social-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 12px;
  width: 100%;
  padding: 11px 16px;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  background: #fff;
  font-family: inherit;
  font-size: 0.87rem;
  font-weight: 700;
  color: #0f172a;
  cursor: pointer;
  transition: border-color 0.15s, background 0.15s;
}

.social-btn:hover {
  border-color: #cbd5e1;
  background: #f8fafc;
}

.social-icon {
  flex-shrink: 0;
  width: 20px;
  height: 20px;
  display: flex;
  align-items: center;
  justify-content: center;
}

.social-icon img {
  width: 20px;
  height: 20px;
}

.social-chevron {
  display: none;
  color: #cbd5e1;
}

.ms-logo {
  display: grid;
  grid-template-columns: repeat(2, 9px);
  grid-template-rows: repeat(2, 9px);
  gap: 2px;
}

.ms-logo i {
  display: block;
  font-style: normal;
}

.ms-logo i:nth-child(1) {
  background: #f25022;
}
.ms-logo i:nth-child(2) {
  background: #7fba00;
}
.ms-logo i:nth-child(3) {
  background: #00a4ef;
}
.ms-logo i:nth-child(4) {
  background: #ffb900;
}

.yahoo-logo {
  font-size: 1.15rem;
  font-weight: 800;
  font-style: italic;
  color: #6001d2;
}

.or-divider {
  display: flex;
  align-items: center;
  gap: 14px;
  margin: 4px 0 20px;
  font-size: 0.72rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  color: #94a3b8;
}

.or-divider::before,
.or-divider::after {
  content: '';
  flex: 1;
  height: 1px;
  background: #e2e8f0;
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

.field-meta {
  text-align: right;
  margin: -6px 0 20px;
}

.forgot-link {
  font-size: 0.8rem;
  font-weight: 600;
  color: #1855b9;
  text-decoration: none;
}

.forgot-link:hover {
  text-decoration: underline;
}

.signin-btn {
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

.signin-btn:hover {
  background: #14213d;
}

.signin-icon {
  display: flex;
  color: #2dd4bf;
}

.form-footer-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  margin: 16px 0 22px;
  font-size: 0.82rem;
  color: #475569;
}

.remember-check {
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
}

.remember-check input {
  width: 15px;
  height: 15px;
  accent-color: #1855b9;
  cursor: pointer;
}

.need-help-link {
  color: #1855b9;
  font-weight: 600;
  text-decoration: none;
}

.need-help-link:hover {
  text-decoration: underline;
}

.signup-divider {
  border-top: 1px solid #e2e8f0;
  margin: 2px 0 20px;
}

.signup-link {
  text-align: center;
  font-size: 0.85rem;
  color: #64748b;
  margin: 0 0 20px;
}

.signup-link a {
  color: #1855b9;
  font-weight: 700;
  text-decoration: none;
}

.signup-link a:hover {
  text-decoration: underline;
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
  margin: 0;
}

.terms-row svg {
  flex-shrink: 0;
  margin-top: 2px;
  color: #94a3b8;
}

.terms-row a {
  color: #1855b9;
  font-weight: 600;
  text-decoration: none;
}

.terms-row a:hover {
  text-decoration: underline;
}

/* ============================================================ */
/* MOBILE                                                        */
/* ============================================================ */
@media (max-width: 900px) {
  .login-page {
    height: auto;
    min-height: 100vh;
    overflow: visible;
  }

  .login-card {
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

  .social-btn {
    justify-content: flex-start;
  }

  .social-chevron {
    display: block;
    margin-left: auto;
  }
}
</style>
