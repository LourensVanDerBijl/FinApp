<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Shown after a user logs in with no group assigned yet (see router guard
// + LoginUser.vue's redirect logic). Lets them choose Create or Join.
//
// Visually aligned with the Admin/User login panels (dark brand sidebar +
// white content panel). Split into focused sub-components under
// components/A_Auth/UserGroupAssign/ so this file stays orchestration-only.
//
// Real/working right now:
//   - Tab switching (mobile), option selection, Group ID input, Logout.
//   - Both "Create Premium/Free Group" tiers route to
//     UserGroupRegistration.vue (?type=premium|free) to finish setup.
//   - "Request to Join" submits to POST /api/user/group-registration/join.
//     Success shows the PendingApprovalModal — also shown immediately on
//     mount if the user arrived here already Pending from a previous
//     session (see currentUserProfile.groupStatus).
// ─────────────────────────────────────────────────────────────────────────
import { ref, computed } from 'vue'
import { useRouter } from 'vue-router'
import { signOut } from 'firebase/auth'
import { auth } from '../../../firebase/firebaseManager.js'
import { currentUserProfile, clearUserProfile } from '../../data/userSession.js'
import { joinGroup } from '../../data/userMockData.js'
import { HelpCircle } from 'lucide-vue-next'

import GroupAssignSidebar from '../../components/A_Auth/UserGroupAssign/GroupAssignSidebar.vue'
import GroupAssignTopBar from '../../components/A_Auth/UserGroupAssign/GroupAssignTopBar.vue'
import GroupAssignHero from '../../components/A_Auth/UserGroupAssign/GroupAssignHero.vue'
import GroupAssignTabs from '../../components/A_Auth/UserGroupAssign/GroupAssignTabs.vue'
import CreateGroupCard from '../../components/A_Auth/UserGroupAssign/CreateGroupCard.vue'
import JoinGroupCard from '../../components/A_Auth/UserGroupAssign/JoinGroupCard.vue'
import OrDivider from '../../components/A_Auth/UserGroupAssign/OrDivider.vue'
import PendingApprovalModal from '../../components/A_Auth/UserGroupAssign/PendingApprovalModal.vue'

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

const activeTab = ref('create') // 'create' | 'join'
const groupIdInput = ref('')
const actionMessage = ref('')
const isSubmittingJoin = ref(false)

// Shown whenever the profile's groupStatus is Pending — true on first
// render if the user arrived here already pending (e.g. logged out and
// back in while waiting), and set true again after a fresh submit.
const showPendingModal = ref(currentUserProfile.value?.groupStatus === 'Pending')
const pendingGroupName = computed(() => currentUserProfile.value?.groupName || '')

// Called directly by CreateGroupCard's tier buttons — picking a tier IS
// the action, so this routes straight to the Group Registration page,
// carrying the chosen tier along as ?type=.
function handleCreateGroup(tier) {
  router.push({ name: 'UserGroupRegistration', query: { type: tier } })
}

async function handleRequestJoin() {
  if (!groupIdInput.value.trim() || isSubmittingJoin.value) return

  isSubmittingJoin.value = true
  actionMessage.value = ''

  const result = await joinGroup(groupIdInput.value.trim())

  isSubmittingJoin.value = false

  if (!result.success) {
    actionMessage.value = result.message
    return
  }

  // Reflect the new Pending state locally so the modal's group name is
  // right and a page refresh (or next login) already sees it too —
  // login itself is what re-fetches the authoritative copy.
  currentUserProfile.value = {
    ...currentUserProfile.value,
    groupId: result.groupId,
    groupName: result.groupName,
    groupStatus: result.groupStatus
  }

  groupIdInput.value = ''
  showPendingModal.value = true
}

// "Let's wait for the owner" — just closes the modal. The user stays on
// this page; nothing else to do until they're approved.
function handleWaitForOwner() {
  showPendingModal.value = false
}

// "Join a different group" — closes the modal and drops them on the
// Join tab with the input ready. Submitting from here is what actually
// abandons the current pending request (see JoinableFromStatuses on the
// backend) — closing the modal alone does not.
function handleJoinDifferentGroup() {
  showPendingModal.value = false
  activeTab.value = 'join'
}
</script>

<template>
  <div class="assign-page">
    <PendingApprovalModal
      v-if="showPendingModal"
      :group-name="pendingGroupName"
      @wait="handleWaitForOwner"
      @join-different="handleJoinDifferentGroup"
    />

    <div class="page-shell">
      <GroupAssignSidebar />

      <!-- ============================== MAIN ============================== -->
      <main class="main-panel">
        <GroupAssignTopBar
          :user-name="currentUserProfile?.displayName || 'Account'"
          :user-initials="userInitials"
          @logout="handleLogout"
        />

        <GroupAssignHero />

        <GroupAssignTabs :active-tab="activeTab" @select="(tab) => (activeTab = tab)" />

        <div class="cards-row">
          <CreateGroupCard
            :is-hidden-mobile="activeTab !== 'create'"
            @create="handleCreateGroup"
          />

          <OrDivider />

          <JoinGroupCard
            v-model:group-id="groupIdInput"
            :is-hidden-mobile="activeTab !== 'join'"
            :is-submitting="isSubmittingJoin"
            @submit="handleRequestJoin"
          />
        </div>

        <p v-if="actionMessage" class="action-message">{{ actionMessage }}</p>

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

.assign-page {
  height: 100vh;
  overflow: hidden;
  background: #fff;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
}

.page-shell {
  height: 100vh;
  display: flex;
}

/* ============================== MAIN PANEL ============================== */
.main-panel {
  flex: 1;
  height: 100vh;
  padding: 14px 30px 12px;
  min-width: 0;
  background: #fff;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

/* ------------------------------- CARDS ROW ------------------------------- */
.cards-row {
  flex: 1;
  min-height: 0;
  display: grid;
  grid-template-columns: 1fr auto 1fr;
  align-items: stretch;
  gap: 16px;
  max-width: 1040px;
  width: 100%;
  margin: 0 auto;
}

/* ------------------------------ MISC / FOOTER ------------------------------ */
.action-message {
  text-align: center;
  font-size: 0.72rem;
  color: #b45309;
  margin: 6px 0 0;
  flex-shrink: 0;
}

.support-row {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  font-size: 0.7rem;
  color: #64748b;
  margin: 8px 0 0;
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
/* MOBILE — allowed to scroll normally, unlike desktop                */
/* ================================================================ */
@media (max-width: 960px) {
  .assign-page {
    height: auto;
    min-height: 100vh;
    overflow: visible;
  }

  .page-shell {
    height: auto;
  }

  .main-panel {
    height: auto;
    overflow: visible;
    padding: 20px 18px 36px;
  }

  .cards-row {
    display: block;
    flex: none;
  }
}
</style>
