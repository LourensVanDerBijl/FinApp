<script setup>
import { ref } from 'vue'
import { X, Crown, Mail } from 'lucide-vue-next'

const props = defineProps({
  open: Boolean,
  group: Object
})

const emit = defineEmits(['close'])

// Brief inline confirmation after Resend Password, per member
const sentFeedback = ref({})

function groupedMembers(group) {
  return {
    main: group.members.filter((m) => m.isOwner),
    pending: group.members.filter((m) => !m.isOwner && m.memberStatus === 'pending'),
    active: group.members.filter((m) => !m.isOwner && m.memberStatus === 'active'),
    suspended: group.members.filter((m) => !m.isOwner && m.memberStatus === 'suspended'),
  }
}

function approveMember(member) {
  member.memberStatus = 'active'
}

function suspendMember(member) {
  if (confirm(`Suspend ${member.preferredName} ${member.lastName}? They will lose access immediately.`)) {
    member.memberStatus = 'suspended'
  }
}

function reinstateMember(member) {
  member.memberStatus = 'active'
}

function resendPassword(member) {
  sentFeedback.value = { ...sentFeedback.value, [member.userId]: true }
  setTimeout(() => {
    const updated = { ...sentFeedback.value }
    delete updated[member.userId]
    sentFeedback.value = updated
  }, 2000)
}

function formatDate(isoString) {
  if (!isoString) return '—'
  return new Date(isoString).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

function initials(preferredName, lastName) {
  return ((preferredName?.charAt(0) || '') + (lastName?.charAt(0) || '')).toUpperCase()
}

const avatarPalette = ['#2563EB', '#DB2777', '#059669', '#D97706', '#7C3AED', '#0891B2']

function avatarColor(userId) {
  let hash = 0
  for (let i = 0; i < userId.length; i++) {
    hash = userId.charCodeAt(i) + ((hash << 5) - hash)
  }
  return avatarPalette[Math.abs(hash) % avatarPalette.length]
}

function memberStatusClass(status) {
  switch (status) {
    case 'active': return 'badge-green'
    case 'pending': return 'badge-amber'
    case 'suspended': return 'badge-red'
    default: return 'badge-neutral'
  }
}

function signInLabel(method) {
  return method === 'Email + Password' ? 'Email' : method
}
</script>

<template>
  <Teleport to="body">
    <div v-if="open" class="drawer-overlay" @click="emit('close')"></div>

    <div class="drawer" :class="{ open: open }">
      <template v-if="group">
        <div class="drawer-header">
          <div>
            <p class="drawer-eyebrow">{{ group.groupId }}</p>
            <h2>{{ group.groupName }}</h2>
          </div>
          <button class="close-btn" @click="emit('close')">
            <X size="18" />
          </button>
        </div>

        <div class="drawer-body">
          <h3 class="members-heading">Members ({{ group.members.length }})</h3>

          <template v-for="(bucket, key) in groupedMembers(group)" :key="key">
            <div v-for="member in bucket" :key="member.userId" class="member-card" :class="key">
              <div class="member-identity">
                <span class="avatar" :style="{ backgroundColor: avatarColor(member.userId) }">
                  {{ initials(member.preferredName, member.lastName) }}
                </span>
                <div>
                  <p class="member-name">
                    {{ member.preferredName }} {{ member.lastName }}
                    <Crown v-if="member.isOwner" size="12" class="owner-crown" />
                  </p>
                  <p class="member-sub">{{ member.firstName }} {{ member.lastName }} · {{ member.email }}</p>
                </div>
              </div>

              <div class="member-meta">
                <span class="meta-chip">
                  <img v-if="member.signInMethod === 'Google'" src="../../../assets/SVG/svgGoogle.svg" alt="Google" class="signin-icon" />
                  <svg v-else-if="member.signInMethod === 'Microsoft'" class="signin-icon" viewBox="0 0 21 21">
                    <rect x="1" y="1" width="9" height="9" fill="#f25022"/>
                    <rect x="11" y="1" width="9" height="9" fill="#7fba00"/>
                    <rect x="1" y="11" width="9" height="9" fill="#00a4ef"/>
                    <rect x="11" y="11" width="9" height="9" fill="#ffb900"/>
                  </svg>
                  <span v-else-if="member.signInMethod === 'Yahoo'" class="yahoo-mark">y!</span>
                  <Mail v-else size="11" />
                  {{ signInLabel(member.signInMethod) }}
                </span>
                <span class="meta-chip">{{ member.country }}</span>
                <span class="meta-chip">{{ member.currency }}</span>
                <span class="meta-chip">{{ member.timezone }}</span>
                <span class="meta-chip">Joined {{ formatDate(member.joinedAt) }}</span>
              </div>

              <div class="member-actions">
                <span class="badge" :class="memberStatusClass(member.memberStatus)">{{ member.memberStatus }}</span>

                <button v-if="member.memberStatus === 'pending'" class="action-btn approve" @click="approveMember(member)">Approve</button>
                <button v-if="!member.isOwner && member.memberStatus === 'active'" class="action-btn suspend" @click="suspendMember(member)">Suspend</button>
                <button v-if="member.memberStatus === 'suspended'" class="action-btn reinstate" @click="reinstateMember(member)">Reinstate</button>
                <button
                  v-if="member.memberStatus !== 'suspended'"
                  class="action-btn resend"
                  @click="resendPassword(member)"
                >
                  {{ sentFeedback[member.userId] ? 'Sent ✓' : 'Resend Password' }}
                </button>
              </div>
            </div>
          </template>
        </div>
      </template>
    </div>
  </Teleport>
</template>

<style scoped>
.drawer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.35);
  z-index: 90;
}

.drawer {
  position: fixed;
  top: 0;
  right: 0;
  height: 100vh;
  width: min(720px, 92vw);
  background: #fff;
  z-index: 100;
  transform: translateX(100%);
  transition: transform 0.25s ease;
  display: flex;
  flex-direction: column;
  box-shadow: -6px 0 24px rgba(0,0,0,0.12);
}

.drawer.open {
  transform: translateX(0);
}

.drawer-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  padding: 18px 22px;
  border-bottom: 1px solid #E5E7EB;
  flex-shrink: 0;
}

.drawer-eyebrow {
  font-size: 0.68rem;
  color: #2563EB;
  font-weight: 700;
  margin: 0 0 2px 0;
}

.drawer-header h2 {
  font-size: 1.05rem;
  margin: 0;
  color: #0F172A;
}

.close-btn {
  background: none;
  border: none;
  color: #64748B;
  cursor: pointer;
  padding: 4px;
}

.close-btn:hover {
  color: #0F172A;
}

.drawer-body {
  flex: 1;
  overflow-y: auto;
  padding: 18px 22px;
}

.members-heading {
  font-size: 0.85rem;
  color: #0F172A;
  margin: 0 0 12px 0;
}

.member-card {
  border: 1px solid #F1F5F9;
  border-radius: 8px;
  padding: 12px;
  margin-bottom: 10px;
}

.member-card.main { background: #F8FAFF; }
.member-card.pending { background: #FFFDF5; }
.member-card.active { background: #FAFFFC; }
.member-card.suspended { background: #FFF8F8; }

.member-identity {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-bottom: 8px;
}

.avatar {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  color: #fff;
  font-size: 0.7rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.member-name {
  font-size: 0.8rem;
  font-weight: 700;
  color: #0F172A;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 5px;
}

.owner-crown {
  color: #D97706;
}

.member-sub {
  font-size: 0.68rem;
  color: #64748B;
  margin: 1px 0 0 0;
}

.member-meta {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  margin-bottom: 10px;
}

.meta-chip {
  display: flex;
  align-items: center;
  gap: 4px;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 5px;
  padding: 3px 8px;
  font-size: 0.66rem;
  color: #334155;
}

.signin-icon {
  width: 11px;
  height: 11px;
}

.yahoo-mark {
  color: #6001D2;
  font-weight: 800;
  font-size: 0.66rem;
}

.member-actions {
  display: flex;
  align-items: center;
  gap: 8px;
  flex-wrap: wrap;
}

.badge {
  font-size: 0.64rem;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 10px;
}

.badge-green { background: #DCFCE7; color: #15803D; }
.badge-amber { background: #FEF3C7; color: #B45309; }
.badge-red { background: #FEE2E2; color: #B91C1C; }
.badge-neutral { background: #F1F5F9; color: #475569; }

.action-btn {
  border: none;
  border-radius: 5px;
  font-size: 0.68rem;
  font-weight: 600;
  padding: 5px 10px;
  cursor: pointer;
}

.action-btn.approve { background: #059669; color: #fff; }
.action-btn.suspend { background: #FEE2E2; color: #B91C1C; }
.action-btn.reinstate { background: #2563EB; color: #fff; }
.action-btn.resend { background: #F1F5F9; color: #334155; }
</style>