<script setup>
// ─────────────────────────────────────────────────────────────────────────
// VISUAL / STRUCTURE ONLY.
// No API calls, no validation, no persistence. The refs below exist purely
// so the page can be clicked through (step navigation, selecting a sign-in
// method, checking the terms box) to preview the UI. Wire this up to real
// registration logic separately.
// ─────────────────────────────────────────────────────────────────────────
import { ref, computed, onMounted, onUnmounted } from 'vue'
import logo from '../../../assets/SVG/logo.svg'
import googleLogo from '../../../assets/SVG/svgGoogle.svg'
import countriesData from '../../../devAdministrator/data/finbine-countries.json'
import {
  User,
  IdCard,
  Mail,
  Calendar,
  Globe,
  Clock,
  DollarSign,
  ChevronDown,
  ChevronLeft,
  ChevronRight,
  Info,
  Check,
  ShieldCheck,
  Users,
  BarChart3,
  Lock,
  ArrowRight,
  Search
} from 'lucide-vue-next'

// Mobile step wizard state (steps 1–4 per the design; only two are built out
// visually here — Account Info + Sign-in Method — the rest can be added the
// same way).
const mobileStep = ref(1)
const totalSteps = 4
const progressPercent = computed(() => (mobileStep.value / totalSteps) * 100)

function goToStep(step) {
  mobileStep.value = step
}

// Sign-in method selection (visual state only)
const selectedMethod = ref('email')

const signInMethods = [
  {
    id: 'email',
    name: 'Email & Password',
    description: "We'll email you a link to set your password",
    selectedDescription:
      'We will create your account and send you a secure link to verify your email and set your password.'
  },
  {
    id: 'google',
    name: 'Google',
    description: 'Sign in with your Google account',
    selectedDescription: 'You will be redirected to Google to sign in and link your account.'
  },
  {
    id: 'microsoft',
    name: 'Microsoft',
    description: 'Sign in with your Microsoft account',
    selectedDescription: 'You will be redirected to Microsoft to sign in and link your account.'
  },
  {
    id: 'yahoo',
    name: 'Yahoo',
    description: 'Sign in with your Yahoo account',
    selectedDescription: 'You will be redirected to Yahoo to sign in and link your account.'
  }
]

const selectedMethodDetails = computed(() =>
  signInMethods.find((m) => m.id === selectedMethod.value)
)

const agreedToTerms = ref(false)

const benefits = [
  {
    icon: ShieldCheck,
    title: 'Bank-level security',
    description: 'Your data is protected with industry-leading encryption and security practices.'
  },
  {
    icon: Users,
    title: 'Built for groups',
    description: 'Manage shared expenses, budgets and financial goals together with your group.'
  },
  {
    icon: BarChart3,
    title: 'Full financial clarity',
    description: 'Get real-time insights and reports to make informed financial decisions.'
  },
  {
    icon: Lock,
    title: 'Your data, your control',
    description: 'You decide what to share and who has access to your financial information.'
  }
]

// ─────────────────────────────────────────────────────────────────────────
// Country / Preferred Currency / Time Zone
//
// finbine-countries.json is the single source of truth for this data (see
// src/devAdministrator/data). We never hardcode country names, currencies
// or time zones here — everything is looked up from the JSON at runtime.
// ─────────────────────────────────────────────────────────────────────────

// Only countries flagged active are eligible for registration. Inactive
// countries stay in the JSON for future expansion but must never appear in
// the dropdown, the search results, or be selectable.
const activeCountries = computed(() => countriesData.countries.filter((c) => c.active))

const selectedCountryCode = ref('')
const selectedCurrencyCode = ref('')
const selectedTimeZone = ref('')

const selectedCountry = computed(
  () => activeCountries.value.find((c) => c.code === selectedCountryCode.value) || null
)

// The currency/time-zone choices on offer are always derived from the
// selected country's own JSON entry — never a separate hardcoded list.
const availableCurrencies = computed(() => selectedCountry.value?.currencies || [])
const availableTimeZones = computed(() => selectedCountry.value?.timeZones || [])

const selectedCurrency = computed(
  () => availableCurrencies.value.find((c) => c.code === selectedCurrencyCode.value) || null
)

// Dropdown open/search state for the three custom combo fields.
const countryDropdownOpen = ref(false)
const currencyDropdownOpen = ref(false)
const timeZoneDropdownOpen = ref(false)

const countrySearch = ref('')
const currencySearch = ref('')
const timeZoneSearch = ref('')

const filteredCountries = computed(() => {
  const query = countrySearch.value.trim().toLowerCase()
  if (!query) return activeCountries.value
  return activeCountries.value.filter((c) => c.name.toLowerCase().includes(query))
})

const filteredCurrencies = computed(() => {
  const query = currencySearch.value.trim().toLowerCase()
  if (!query) return availableCurrencies.value
  return availableCurrencies.value.filter(
    (c) => c.name.toLowerCase().includes(query) || c.code.toLowerCase().includes(query)
  )
})

const filteredTimeZones = computed(() => {
  const query = timeZoneSearch.value.trim().toLowerCase()
  if (!query) return availableTimeZones.value
  return availableTimeZones.value.filter((tz) => tz.toLowerCase().includes(query))
})

function closeAllDropdowns() {
  countryDropdownOpen.value = false
  currencyDropdownOpen.value = false
  timeZoneDropdownOpen.value = false
}

function toggleCountryDropdown() {
  const next = !countryDropdownOpen.value
  closeAllDropdowns()
  countryDropdownOpen.value = next
  if (next) countrySearch.value = ''
}

function toggleCurrencyDropdown() {
  const next = !currencyDropdownOpen.value
  closeAllDropdowns()
  currencyDropdownOpen.value = next
  if (next) currencySearch.value = ''
}

function toggleTimeZoneDropdown() {
  const next = !timeZoneDropdownOpen.value
  closeAllDropdowns()
  timeZoneDropdownOpen.value = next
  if (next) timeZoneSearch.value = ''
}

// Selecting a country auto-populates currency + time zone from that
// country's defaults, but never locks them — the user can still open the
// Currency/Time Zone dropdowns and pick something else afterwards.
// Re-selecting a (different) country resets both back to the new
// country's defaults, per the required behaviour.
function selectCountry(country) {
  selectedCountryCode.value = country.code
  selectedCurrencyCode.value = country.defaultCurrency
  selectedTimeZone.value = country.defaultTimeZone
  countryDropdownOpen.value = false
}

function selectCurrency(currency) {
  selectedCurrencyCode.value = currency.code
  currencyDropdownOpen.value = false
}

function selectTimeZone(timeZone) {
  selectedTimeZone.value = timeZone
  timeZoneDropdownOpen.value = false
}

// Close any open combo dropdown when the user clicks outside of it.
function handleDocumentClick(event) {
  const container = event.target.closest('.combo-wrap')
  if (!container) closeAllDropdowns()
}

onMounted(() => document.addEventListener('click', handleDocumentClick))
onUnmounted(() => document.removeEventListener('click', handleDocumentClick))

// No-op placeholder — visual only.
function handleCreateAccount() {
  // Intentionally does nothing yet.
}
</script>

<template>
  <div class="registration-page">
    <!-- ============================= HERO ============================= -->
    <header class="hero">
      <div class="hero-inner">
        <button v-if="mobileStep === 2" class="back-btn" type="button" @click="goToStep(1)" aria-label="Go back">
          <ChevronLeft :size="20" />
        </button>

        <div class="brand-row">
          <img :src="logo" alt="FinBine Logo" class="brand-logo" />
          <span class="brand-wordmark">FinBine</span>
        </div>

        <div class="hero-copy" :class="{ 'hero-copy-collapsed': mobileStep === 2 }">
          <p class="eyebrow">JOIN FINBINE TODAY</p>
          <h1>Create your FinBine account</h1>
          <p class="subtitle">
            Create your account to get started with managing your group's finances all in one secure place.
          </p>
        </div>
      </div>
    </header>

    <!-- ============================= CARD ============================= -->
    <main class="card-shell">
      <div class="card">
        <!-- Mobile-only progress bar -->
        <div class="mobile-progress">
          <div class="progress-track">
            <div class="progress-fill" :style="{ width: progressPercent + '%' }"></div>
          </div>
          <p class="step-label">Step {{ mobileStep }} of {{ totalSteps }}</p>
        </div>

        <div class="card-columns">
          <!-- ---------------------------- FORM ---------------------------- -->
          <div class="form-panel">
            <!-- STEP 1 — Account Information -->
            <section class="step-block" :class="{ 'is-hidden-mobile': mobileStep !== 1 }">
              <h2>Account Information</h2>
              <p class="panel-sub">Please provide your details to create your account.</p>

              <div class="field-row">
                <div class="field">
                  <label>First Name</label>
                  <div class="input-wrap">
                    <User :size="16" class="input-icon" />
                    <input type="text" placeholder="Enter your first name" />
                  </div>
                </div>
                <div class="field">
                  <label>Last Name</label>
                  <div class="input-wrap">
                    <User :size="16" class="input-icon" />
                    <input type="text" placeholder="Enter your last name" />
                  </div>
                </div>
              </div>

              <div class="field">
                <label>Display Name</label>
                <div class="input-wrap">
                  <IdCard :size="16" class="input-icon" />
                  <input type="text" placeholder="How should we call you?" />
                </div>
                <p class="hint">This is how your name will appear in FinBine.</p>
              </div>

              <div class="field-row">
                <div class="field">
                  <label>Email Address</label>
                  <div class="input-wrap">
                    <Mail :size="16" class="input-icon" />
                    <input type="email" placeholder="Enter your email address" />
                  </div>
                </div>
                <div class="field">
                  <label>Date of Birth</label>
                  <div class="input-wrap">
                    <Calendar :size="16" class="input-icon" />
                    <input type="text" placeholder="DD / MM / YYYY" />
                  </div>
                  <p class="hint">You must be 18 years or older to use FinBine.</p>
                </div>
              </div>

              <div class="field-row">
                <div class="field">
                  <label>Country</label>
                  <div class="input-wrap select-wrap combo-wrap" @click="toggleCountryDropdown">
                    <Globe :size="16" class="input-icon" />
                    <span class="combo-value" :class="{ placeholder: !selectedCountry }">
                      {{ selectedCountry ? selectedCountry.name : 'Select your country' }}
                    </span>
                    <ChevronDown :size="16" class="select-caret" />

                    <div v-if="countryDropdownOpen" class="combo-panel" @click.stop>
                      <div class="combo-search">
                        <Search :size="14" class="combo-search-icon" />
                        <input
                          type="text"
                          v-model="countrySearch"
                          placeholder="Search country..."
                          autofocus
                        />
                      </div>
                      <ul class="combo-options">
                        <li
                          v-for="country in filteredCountries"
                          :key="country.code"
                          class="combo-option"
                          :class="{ selected: country.code === selectedCountryCode }"
                          @click="selectCountry(country)"
                        >
                          {{ country.name }}
                        </li>
                        <li v-if="!filteredCountries.length" class="combo-empty">No countries found</li>
                      </ul>
                    </div>
                  </div>
                </div>
                <div class="field">
                  <label>Time Zone</label>
                  <div
                    class="input-wrap select-wrap combo-wrap"
                    :class="{ disabled: !selectedCountry }"
                    @click="selectedCountry && toggleTimeZoneDropdown()"
                  >
                    <Clock :size="16" class="input-icon" />
                    <span class="combo-value" :class="{ placeholder: !selectedTimeZone }">
                      {{ selectedTimeZone || 'Select your time zone' }}
                    </span>
                    <ChevronDown :size="16" class="select-caret" />

                    <div v-if="timeZoneDropdownOpen" class="combo-panel" @click.stop>
                      <div class="combo-search">
                        <Search :size="14" class="combo-search-icon" />
                        <input
                          type="text"
                          v-model="timeZoneSearch"
                          placeholder="Search time zone..."
                          autofocus
                        />
                      </div>
                      <ul class="combo-options">
                        <li
                          v-for="tz in filteredTimeZones"
                          :key="tz"
                          class="combo-option"
                          :class="{ selected: tz === selectedTimeZone }"
                          @click="selectTimeZone(tz)"
                        >
                          {{ tz }}
                        </li>
                        <li v-if="!filteredTimeZones.length" class="combo-empty">No time zones found</li>
                      </ul>
                    </div>
                  </div>
                  <p v-if="!selectedCountry" class="hint">Select a country first.</p>
                </div>
              </div>

              <div class="field">
                <label>Preferred Currency</label>
                <div
                  class="input-wrap select-wrap combo-wrap"
                  :class="{ disabled: !selectedCountry }"
                  @click="selectedCountry && toggleCurrencyDropdown()"
                >
                  <DollarSign :size="16" class="input-icon" />
                  <span class="combo-value" :class="{ placeholder: !selectedCurrency }">
                    {{ selectedCurrency ? `${selectedCurrency.code} — ${selectedCurrency.name}` : 'Select your preferred currency' }}
                  </span>
                  <ChevronDown :size="16" class="select-caret" />

                  <div v-if="currencyDropdownOpen" class="combo-panel" @click.stop>
                    <div class="combo-search">
                      <Search :size="14" class="combo-search-icon" />
                      <input
                        type="text"
                        v-model="currencySearch"
                        placeholder="Search currency..."
                        autofocus
                      />
                    </div>
                    <ul class="combo-options">
                      <li
                        v-for="currency in filteredCurrencies"
                        :key="currency.code"
                        class="combo-option"
                        :class="{ selected: currency.code === selectedCurrencyCode }"
                        @click="selectCurrency(currency)"
                      >
                        {{ currency.code }} — {{ currency.name }}
                      </li>
                      <li v-if="!filteredCurrencies.length" class="combo-empty">No currencies found</li>
                    </ul>
                  </div>
                </div>
                <p class="hint">This will be your default currency for financial data.</p>
              </div>

              <button type="button" class="continue-btn" @click="goToStep(2)">Continue</button>

              <div class="step-dots">
                <span
                  v-for="n in totalSteps"
                  :key="n"
                  class="dot"
                  :class="{ active: n === 1 }"
                ></span>
              </div>
            </section>

            <div class="section-divider"></div>

            <!-- STEP 2 — Sign-in method -->
            <section class="step-block" :class="{ 'is-hidden-mobile': mobileStep !== 2 }">
              <h2>Choose your sign-in method</h2>
              <p class="panel-sub">You can only use one sign-in method for your account.</p>

              <div class="method-grid">
                <button
                  v-for="method in signInMethods"
                  :key="method.id"
                  type="button"
                  class="method-card"
                  :class="{ selected: selectedMethod === method.id }"
                  @click="selectedMethod = method.id"
                >
                  <span class="method-icon">
                    <Mail v-if="method.id === 'email'" :size="20" />
                    <img v-else-if="method.id === 'google'" :src="googleLogo" alt="Google" />
                    <span v-else-if="method.id === 'microsoft'" class="ms-logo" aria-hidden="true">
                      <i></i><i></i><i></i><i></i>
                    </span>
                    <span v-else-if="method.id === 'yahoo'" class="yahoo-logo" aria-hidden="true">y!</span>
                  </span>

                  <span class="method-name">{{ method.name }}</span>
                  <span class="method-desc">{{ method.description }}</span>

                  <Check v-if="selectedMethod === method.id" :size="16" class="method-badge check" />
                  <ChevronRight v-else :size="16" class="method-badge chevron" />
                </button>
              </div>

              <div class="info-box">
                <Info :size="16" class="info-icon" />
                <div>
                  <strong>{{ selectedMethodDetails.name }} selected</strong>
                  <p>{{ selectedMethodDetails.selectedDescription }}</p>
                </div>
              </div>

              <label class="terms-check">
                <input type="checkbox" v-model="agreedToTerms" />
                <span>I agree to FinBine's <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a>.</span>
              </label>

              <button type="button" class="create-btn" @click="handleCreateAccount">Create Account</button>

              <p class="signin-link">Already have an account? <a href="#">Sign in</a></p>
            </section>
          </div>

          <!-- -------------------------- BENEFITS -------------------------- -->
          <aside class="benefits-panel">
            <div class="benefits-list">
              <div v-for="benefit in benefits" :key="benefit.title" class="benefit-item">
                <div class="benefit-icon">
                  <component :is="benefit.icon" :size="20" />
                </div>
                <div>
                  <h4>{{ benefit.title }}</h4>
                  <p>{{ benefit.description }}</p>
                </div>
              </div>
            </div>

            <div class="help-block">
              <h4>Need help?</h4>
              <p>Our support team is here to help you get started.</p>
              <a href="#" class="contact-link">
                Contact Support
                <ArrowRight :size="14" />
              </a>
            </div>
          </aside>
        </div>
      </div>
    </main>

    <!-- ============================ FOOTER ============================ -->
    <footer class="page-footer">
      <p class="footer-secure"><Lock :size="14" /> Your security and privacy are our top priority.</p>
      <p class="footer-copy">© {{ new Date().getFullYear() }} FinBine. All rights reserved.</p>
    </footer>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.registration-page {
  background-color: #0b1220;
  min-height: 100vh;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
}

/* ============================== HERO ============================== */
.hero {
  padding: 56px 24px 120px;
  text-align: center;
  position: relative;
}

.hero-inner {
  max-width: 640px;
  margin: 0 auto;
}

.back-btn {
  display: none;
  align-items: center;
  justify-content: center;
  position: absolute;
  top: 24px;
  left: 20px;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  border: none;
  background: rgba(255, 255, 255, 0.08);
  color: #fff;
  cursor: pointer;
}

.brand-row {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
  margin-bottom: 18px;
}

.brand-logo {
  height: 40px;
  width: auto;
}

.brand-wordmark {
  font-size: 1.4rem;
  font-weight: 700;
  color: #fff;
}

.eyebrow {
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 0.06em;
  color: #4f8ef7;
  margin: 0 0 12px;
}

.hero-copy h1 {
  font-size: 2.25rem;
  font-weight: 800;
  color: #fff;
  margin: 0 0 14px;
  line-height: 1.2;
}

.hero-copy .subtitle {
  font-size: 0.95rem;
  color: #94a3b8;
  line-height: 1.6;
  margin: 0;
}

/* ============================== CARD ============================== */
.card-shell {
  max-width: 1200px;
  margin: 0 auto;
  padding: 0 24px;
  margin-top: -84px;
}

.card {
  background: #fff;
  border-radius: 20px;
  box-shadow: 0 20px 50px rgba(0, 0, 0, 0.25);
  overflow: hidden;
}

.mobile-progress {
  display: none;
}

.card-columns {
  display: grid;
  grid-template-columns: 1fr 340px;
}

/* ---------------------------- FORM PANEL ---------------------------- */
.form-panel {
  padding: 40px 44px;
}

.step-block h2 {
  font-size: 1.15rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 4px;
}

.panel-sub {
  font-size: 0.85rem;
  color: #64748b;
  margin: 0 0 24px;
}

.field-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
  margin-bottom: 20px;
}

.field {
  margin-bottom: 20px;
}

.field-row .field {
  margin-bottom: 0;
}

.field label {
  display: block;
  font-size: 0.8rem;
  font-weight: 600;
  color: #0f172a;
  margin-bottom: 6px;
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

.input-wrap input,
.input-wrap select {
  width: 100%;
  padding: 10px 12px 10px 38px;
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

.input-wrap input:focus,
.input-wrap select:focus {
  border-color: #1855b9;
}

.select-wrap select {
  appearance: none;
  padding-right: 34px;
  color: #94a3b8;
}

.select-caret {
  position: absolute;
  right: 12px;
  color: #94a3b8;
  pointer-events: none;
}

.hint {
  font-size: 0.75rem;
  color: #94a3b8;
  margin: 6px 0 0;
}

/* ------------------------- SEARCHABLE COMBO FIELDS ------------------------- */
.combo-wrap {
  cursor: pointer;
  user-select: none;
}

.combo-wrap.disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.combo-value {
  width: 100%;
  padding: 10px 34px 10px 38px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.85rem;
  color: #0f172a;
  background: #fff;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.combo-value.placeholder {
  color: #94a3b8;
}

.combo-wrap:not(.disabled) .combo-value:hover {
  border-color: #cbd5e1;
}

.combo-panel {
  position: absolute;
  top: calc(100% + 6px);
  left: 0;
  right: 0;
  z-index: 20;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  box-shadow: 0 12px 28px rgba(15, 23, 42, 0.14);
  overflow: hidden;
  cursor: default;
}

.combo-search {
  position: relative;
  display: flex;
  align-items: center;
  border-bottom: 1px solid #e2e8f0;
  padding: 8px 10px;
}

.combo-search-icon {
  position: absolute;
  left: 18px;
  color: #94a3b8;
  pointer-events: none;
}

.combo-search input {
  width: 100%;
  padding: 6px 8px 6px 24px;
  border: none;
  outline: none;
  font-size: 0.83rem;
  color: #0f172a;
  font-family: inherit;
  background: transparent;
}

.combo-options {
  list-style: none;
  margin: 0;
  padding: 4px 0;
  max-height: 200px;
  overflow-y: auto;
}

.combo-option {
  padding: 9px 14px;
  font-size: 0.83rem;
  color: #0f172a;
  cursor: pointer;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.combo-option:hover {
  background: #f1f5f9;
}

.combo-option.selected {
  background: #dbeafe;
  color: #1855b9;
  font-weight: 600;
}

.combo-empty {
  padding: 10px 14px;
  font-size: 0.8rem;
  color: #94a3b8;
}

.section-divider {
  border-top: 1px solid #e2e8f0;
  margin: 8px 0 28px;
}

/* -------------------------- SIGN-IN METHOD -------------------------- */
.method-grid {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 12px;
  margin-bottom: 16px;
}

.method-card {
  position: relative;
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 6px;
  text-align: left;
  padding: 14px 14px 16px;
  border: 1.5px solid #e2e8f0;
  border-radius: 10px;
  background: #fff;
  cursor: pointer;
  font-family: inherit;
}

.method-card.selected {
  border-color: #1855b9;
  background: #eff6ff;
}

.method-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 22px;
  color: #1855b9;
}

.method-icon img {
  height: 20px;
  width: 20px;
}

.method-name {
  font-size: 0.82rem;
  font-weight: 700;
  color: #0f172a;
}

.method-desc {
  font-size: 0.72rem;
  color: #64748b;
  line-height: 1.4;
}

.method-badge {
  position: absolute;
  top: 12px;
  right: 12px;
}

.method-badge.check {
  color: #fff;
  background: #1855b9;
  border-radius: 50%;
  padding: 2px;
  width: 16px;
  height: 16px;
  box-sizing: content-box;
}

.method-badge.chevron {
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

.info-box {
  display: flex;
  align-items: flex-start;
  gap: 10px;
  background: #eff6ff;
  border: 1px solid #dbeafe;
  border-radius: 8px;
  padding: 12px 14px;
  margin-bottom: 18px;
}

.info-icon {
  color: #1855b9;
  margin-top: 2px;
  flex-shrink: 0;
}

.info-box strong {
  display: block;
  font-size: 0.82rem;
  color: #0f172a;
  margin-bottom: 2px;
}

.info-box p {
  font-size: 0.78rem;
  color: #475569;
  line-height: 1.5;
  margin: 0;
}

.terms-check {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  font-size: 0.8rem;
  color: #334155;
  margin-bottom: 18px;
  cursor: pointer;
}

.terms-check input[type='checkbox'] {
  appearance: auto;
  width: 16px;
  height: 16px;
  min-width: 16px;
  max-width: 16px;
  padding: 0;
  margin: 2px 0 0;
  flex-shrink: 0;
  accent-color: #1855b9;
}

.terms-check a {
  color: #1855b9;
  text-decoration: none;
  font-weight: 600;
}

.terms-check a:hover {
  text-decoration: underline;
}

.create-btn {
  width: 100%;
  padding: 12px 16px;
  border: none;
  border-radius: 8px;
  background: #1855b9;
  color: #fff;
  font-size: 0.9rem;
  font-weight: 700;
  cursor: pointer;
  font-family: inherit;
  transition: background 0.15s;
}

.create-btn:hover {
  background: #123f8a;
}

.signin-link {
  text-align: center;
  font-size: 0.82rem;
  color: #64748b;
  margin: 14px 0 0;
}

.signin-link a {
  color: #1855b9;
  font-weight: 600;
  text-decoration: none;
}

.signin-link a:hover {
  text-decoration: underline;
}

.continue-btn,
.step-dots {
  display: none;
}

/* -------------------------- BENEFITS PANEL -------------------------- */
.benefits-panel {
  background: #f8fafc;
  border-left: 1px solid #e2e8f0;
  padding: 40px 32px;
}

.benefits-list {
  display: flex;
  flex-direction: column;
  gap: 26px;
}

.benefit-item {
  display: flex;
  gap: 14px;
}

.benefit-icon {
  flex-shrink: 0;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background: #dbeafe;
  color: #1855b9;
  display: flex;
  align-items: center;
  justify-content: center;
}

.benefit-item h4 {
  font-size: 0.88rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 4px;
}

.benefit-item p {
  font-size: 0.78rem;
  color: #64748b;
  line-height: 1.5;
  margin: 0;
}

.help-block {
  margin-top: 36px;
  padding-top: 24px;
  border-top: 1px solid #e2e8f0;
}

.help-block h4 {
  font-size: 0.85rem;
  font-weight: 700;
  color: #0f172a;
  margin: 0 0 6px;
}

.help-block p {
  font-size: 0.78rem;
  color: #64748b;
  line-height: 1.5;
  margin: 0 0 8px;
}

.contact-link {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  color: #1855b9;
  font-size: 0.8rem;
  font-weight: 600;
  text-decoration: none;
}

.contact-link:hover {
  text-decoration: underline;
}

/* ============================== FOOTER ============================== */
.page-footer {
  text-align: center;
  padding: 32px 24px 40px;
}

.footer-secure {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 8px;
  font-size: 0.8rem;
  color: #94a3b8;
  margin: 0 0 8px;
}

.footer-copy {
  font-size: 0.75rem;
  color: #64748b;
  margin: 0;
}

/* ============================================================ */
/* MOBILE                                                         */
/* ============================================================ */
@media (max-width: 900px) {
  .hero {
    padding: 20px 20px 24px;
    text-align: center;
  }

  .back-btn {
    display: flex;
  }

  .brand-row {
    margin-bottom: 14px;
  }

  .brand-logo {
    height: 32px;
  }

  .brand-wordmark {
    font-size: 1.15rem;
  }

  .hero-copy h1 {
    font-size: 1.5rem;
  }

  .hero-copy.hero-copy-collapsed {
    display: none;
  }

  .card-shell {
    margin-top: 0;
    padding: 0 14px;
  }

  .card {
    border-radius: 18px;
  }

  .mobile-progress {
    display: block;
    padding: 18px 20px 0;
  }

  .progress-track {
    height: 4px;
    border-radius: 2px;
    background: #e2e8f0;
    overflow: hidden;
    margin-bottom: 10px;
  }

  .progress-fill {
    height: 100%;
    background: #1855b9;
    border-radius: 2px;
    transition: width 0.2s;
  }

  .step-label {
    font-size: 0.78rem;
    font-weight: 600;
    color: #1855b9;
    margin: 0 0 4px;
  }

  .card-columns {
    display: block;
  }

  .form-panel {
    padding: 20px 20px 28px;
  }

  .field-row {
    grid-template-columns: 1fr;
    gap: 0;
  }

  .field-row .field {
    margin-bottom: 20px;
  }

  .step-block.is-hidden-mobile {
    display: none;
  }

  .section-divider {
    display: none;
  }

  .method-grid {
    grid-template-columns: 1fr;
  }

  .method-card {
    flex-direction: row;
    align-items: center;
    flex-wrap: wrap;
    padding: 14px 40px 14px 14px;
  }

  .method-icon {
    width: 22px;
  }

  .method-name {
    flex: 1;
  }

  .method-desc {
    width: 100%;
    padding-left: 34px;
    margin-top: -4px;
  }

  .continue-btn {
    display: block;
    width: 100%;
    padding: 12px 16px;
    border: none;
    border-radius: 8px;
    background: #1855b9;
    color: #fff;
    font-size: 0.9rem;
    font-weight: 700;
    cursor: pointer;
    font-family: inherit;
    margin-top: 4px;
  }

  .step-dots {
    display: flex;
    justify-content: center;
    gap: 6px;
    margin-top: 20px;
  }

  .dot {
    width: 7px;
    height: 7px;
    border-radius: 50%;
    background: #cbd5e1;
  }

  .dot.active {
    background: #1855b9;
    width: 18px;
    border-radius: 4px;
  }

  .benefits-panel {
    border-left: none;
    border-top: 1px solid #e2e8f0;
    border-radius: 18px 18px 0 0;
    margin-top: 8px;
    padding: 28px 20px;
  }
}

@media (max-width: 520px) {
  .method-card {
    padding-right: 36px;
  }
}
</style>
