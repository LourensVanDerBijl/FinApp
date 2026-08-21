<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Shown after the user picks "Create Premium Group" or "Create Free Group"
// on UserGroupAssign.vue. Both buttons route here — the only difference
// is the ?type= query param, which drives which copy/CTA shows.
//
// Route is guarded with meta.requiresUserAuth (see router/index.js), same
// mechanism as UserGroupAssign/UserGroupDashboard — the global guard
// re-verifies the Firebase token via loadCurrentUserProfile() before
// letting anyone reach this page, so a stale/expired login can't land
// here. This is the USER guard, separate from the Admin one.
//
// All API calls live in data/userMockData.js (createGroup) — this file
// only orchestrates state and hands data to it, never calls fetch()
// directly. See userMockData.js's file header for that convention.
// ─────────────────────────────────────────────────────────────────────────
import { ref, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { signOut } from 'firebase/auth'
import { auth } from '../../../firebase/firebaseManager.js'
import { currentUserProfile, clearUserProfile, loadCurrentUserProfile } from '../../data/userSession.js'
import { createGroup } from '../../data/userMockData.js'
import { HelpCircle } from 'lucide-vue-next'

import GroupRegistrationSidebar from '../../components/A_Auth/UserGroupRegistration/GroupRegistrationSidebar.vue'
import GroupRegistrationTopBar from '../../components/A_Auth/UserGroupRegistration/GroupRegistrationTopBar.vue'
import GroupRegistrationHero from '../../components/A_Auth/UserGroupRegistration/GroupRegistrationHero.vue'
import GroupNameField from '../../components/A_Auth/UserGroupRegistration/GroupNameField.vue'
import GroupTypeInfoPanel from '../../components/A_Auth/UserGroupRegistration/GroupTypeInfoPanel.vue'
import GroupRegistrationActions from '../../components/A_Auth/UserGroupRegistration/GroupRegistrationActions.vue'

const route = useRoute()
const router = useRouter()

const userInitials = computed(() => {
  const name = currentUserProfile.value?.displayName?.trim() || ''
  if (!name) return '?'
  const parts = name.split(' ').filter(Boolean)
  if (parts.length === 1) return parts[0].charAt(0).toUpperCase()
  return (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase()
})

async function handleLogout() {
  await signOut(auth)
  clearUserProfile()
  router.push('/user/login')
}

// ─────────────────────────────────────────────────────────────────────────
// Account type — comes in via ?type=premium|free from UserGroupAssign.vue.
// Anything else defaults to 'free'. Kept in sync with the URL so a page
// refresh or a shared link doesn't lose the selection.
// ─────────────────────────────────────────────────────────────────────────
const accountType = computed(() => (route.query.type === 'premium' ? 'premium' : 'free'))

function setAccountType(type) {
  router.replace({ query: { ...route.query, type } })
}

function goBackToGroupAssign() {
  router.push('/user/group-assign')
}

// ─────────────────────────────────────────────────────────────────────────
// Group Name — with a one-click suggestion built from the user's surname.
// NOTE: the login response doesn't expose a discrete Surname field today
// (see UserLoginResponse.cs) — using the last word of displayName as a
// stand-in until the backend adds one. Rule: surname + " Group", capped
// at 20 chars, and any trimming comes off the surname only (never the
// " Group" suffix, leading space included).
// ─────────────────────────────────────────────────────────────────────────
const MAX_GROUP_NAME_LENGTH = 20
const GROUP_SUFFIX = ' Group'

const suggestedSurname = computed(() => {
  const name = currentUserProfile.value?.displayName?.trim() || ''
  const parts = name.split(' ').filter(Boolean)
  return parts.length ? parts[parts.length - 1] : 'My'
})

const suggestedGroupName = computed(() => {
  let surname = suggestedSurname.value
  let combined = surname + GROUP_SUFFIX
  if (combined.length > MAX_GROUP_NAME_LENGTH) {
    const allowedSurnameLength = Math.max(MAX_GROUP_NAME_LENGTH - GROUP_SUFFIX.length, 0)
    surname = surname.slice(0, allowedSurnameLength)
    combined = surname + GROUP_SUFFIX
  }
  return combined
})

const groupName = ref('')
const groupNameError = ref('')
const actionMessage = ref('')
const isSubmitting = ref(false)

async function handleSubmit() {
  groupNameError.value = ''
  actionMessage.value = ''

  const trimmedName = groupName.value.trim()
  if (!trimmedName) {
    groupNameError.value = 'Enter a name for your group.'
    return
  }

  isSubmitting.value = true
  try {
    const groupType = accountType.value === 'premium' ? 'Premium' : 'Free'
    const result = await createGroup(trimmedName, groupType)

    if (!result.success) {
      actionMessage.value = result.message || 'Group creation failed. Please try again.'
      return
    }

    // The backend just updated fb_users (group_id/group_name) for this
    // owner — refresh the cached profile so the dashboard sees the new
    // group immediately instead of stale "no group" data.
    await loadCurrentUserProfile()
    router.push('/user/dashboard')
  } finally {
    isSubmitting.value = false
  }
}
</script>

<template>
  <div class="registration-page">
    <div class="page-shell">
      <GroupRegistrationSidebar />

      <!-- ============================== MAIN ============================== -->
      <main class="main-panel">
        <GroupRegistrationTopBar
          :user-name="currentUserProfile?.displayName || 'Account'"
          :user-initials="userInitials"
          @logout="handleLogout"
        />

        <GroupRegistrationHero :account-type="accountType" @back="goBackToGroupAssign" />

        <div class="content-columns">
          <section class="form-column">
            <GroupNameField
              v-model="groupName"
              :suggested-name="suggestedGroupName"
              :error-text="groupNameError"
            />

            <GroupRegistrationActions
              :account-type="accountType"
              :is-submitting="isSubmitting"
              :action-message="actionMessage"
              @submit="handleSubmit"
            />
          </section>

          <GroupTypeInfoPanel :account-type="accountType" @switch-type="setAccountType" />
        </div>

        <p class="support-row">
          <HelpCircle :size="13" />
          Need help? <a href="#">Contact FinBine Support</a>
        </p>
      </main>
    </div>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.registration-page {
  min-height: 100vh;
  background: #fff;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
}

.page-shell {
  min-height: 100vh;
  display: flex;
}

/* ============================== MAIN PANEL ============================== */
.main-panel {
  flex: 1;
  min-width: 0;
  padding: 18px 30px 24px;
  background: #fff;
  display: flex;
  flex-direction: column;
}

/* ----------------------------- CONTENT COLUMNS ----------------------------- */
.content-columns {
  display: grid;
  grid-template-columns: 1fr 300px;
  align-items: start;
  gap: 20px;
  max-width: 760px;
  width: 100%;
  margin: 0 auto;
  flex: 1;
}

.form-column {
  background: rgba(37, 99, 235, 0.05);
  border: 1px solid rgba(37, 99, 235, 0.16);
  border-radius: 13px;
  padding: 16px 18px;
}

/* ------------------------------ SUPPORT ROW ------------------------------ */
.support-row {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  font-size: 0.7rem;
  color: #64748b;
  margin: 16px 0 0;
  flex-shrink: 0;
}

.support-row a {
  color: #1855b9;
  font-weight: 600;
  text-decoration: none;
}

.support-row a:hover {
  text-decoration: underline;
}

/* ================================================================ */
/* MOBILE                                                             */
/* ================================================================ */
@media (max-width: 960px) {
  .main-panel {
    padding: 20px 18px 36px;
  }

  .content-columns {
    display: block;
  }

  .form-column {
    padding: 20px;
  }
}
</style>
