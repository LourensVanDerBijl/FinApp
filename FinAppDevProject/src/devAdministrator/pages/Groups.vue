<script setup>
import { ref, computed } from 'vue'
import { groups } from '../data/mockData.js'
import GroupMembersDrawer from '../components/groups/GroupMembersDrawer.vue'
import {
  Search,
  SlidersHorizontal,
  Download,
  Users,
  CheckCircle2,
  Crown,
  Coins,
  Clock,
  Ban,
  MoreVertical
} from 'lucide-vue-next'

// ------------------------------------------------------------
// Search
// ------------------------------------------------------------
const searchTerm = ref('')

const filteredGroups = computed(() => {
  const term = searchTerm.value.trim().toLowerCase()
  if (!term) return groups

  return groups.filter((group) => {
    const ownerMatch =
      group.owner.email.toLowerCase().includes(term) ||
      `${group.owner.firstName} ${group.owner.lastName}`.toLowerCase().includes(term)

    const memberMatch = group.members.some(
      (m) =>
        m.email.toLowerCase().includes(term) ||
        `${m.firstName} ${m.lastName}`.toLowerCase().includes(term) ||
        m.preferredName.toLowerCase().includes(term)
    )

    return (
      group.groupId.toLowerCase().includes(term) ||
      group.groupName.toLowerCase().includes(term) ||
      ownerMatch ||
      memberMatch
    )
  })
})

// ------------------------------------------------------------
// Summary stat cards
// ------------------------------------------------------------
function pct(count) {
  if (groups.length === 0) return '0'
  return ((count / groups.length) * 100).toFixed(1)
}

const totalGroups = computed(() => groups.length)
const activeGroups = computed(() => groups.filter((g) => g.groupStatus === 'Active').length)
const premiumGroups = computed(() => groups.filter((g) => g.accountType === 'Premium').length)
const freeGroups = computed(() => groups.filter((g) => g.accountType === 'Free').length)
const pendingApprovalGroups = computed(() => groups.filter((g) => g.groupStatus === 'Pending Approval').length)
const terminatedGroups = computed(
  () => groups.filter((g) => g.groupStatus === 'Terminated' || g.groupStatus === 'Pending Termination').length
)

const summaryCards = computed(() => [
  { key: 'total', label: 'Total Groups', value: `${totalGroups.value}`, icon: Users, color: 'blue' },
  { key: 'active', label: 'Active Groups', value: `${activeGroups.value}`, icon: CheckCircle2, color: 'green' },
  { key: 'premium', label: 'Premium Groups', value: `${premiumGroups.value} (${pct(premiumGroups.value)}%)`, icon: Crown, color: 'purple' },
  { key: 'free', label: 'Free Groups', value: `${freeGroups.value} (${pct(freeGroups.value)}%)`, icon: Coins, color: 'teal' },
  { key: 'pending', label: 'Pending Approval', value: `${pendingApprovalGroups.value}`, icon: Clock, color: 'amber' },
  { key: 'terminated', label: 'Terminated Groups', value: `${terminatedGroups.value}`, icon: Ban, color: 'red' },
])

// ------------------------------------------------------------
// Row menu (Suspend Group / Terminate Group / View Audit Log)
// ------------------------------------------------------------
const openMenuId = ref(null)

function toggleMenu(groupId) {
  openMenuId.value = openMenuId.value === groupId ? null : groupId
}

function closeMenu() {
  openMenuId.value = null
}

function suspendGroup(group) {
  if (confirm(`Suspend "${group.groupName}"? Members will lose access until it's reinstated.`)) {
    group.groupStatus = 'Suspended'
  }
  closeMenu()
}

function terminateGroup(group) {
  if (confirm(`Terminate "${group.groupName}"? This cannot be undone from here.`)) {
    group.groupStatus = 'Terminated'
  }
  closeMenu()
}

function viewAuditLog() {
  alert('Audit log page not built yet — coming in a future update.')
  closeMenu()
}

// ------------------------------------------------------------
// Drawer
// ------------------------------------------------------------
const drawerOpen = ref(false)
const selectedGroup = ref(null)

function openDrawer(group) {
  selectedGroup.value = group
  drawerOpen.value = true
}

function closeDrawer() {
  drawerOpen.value = false
}

// ------------------------------------------------------------
// Formatting + badge helpers
// ------------------------------------------------------------
function formatDateOnly(dateString) {
  if (!dateString) return '—'
  return new Date(dateString).toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

function timeAgo(isoString) {
  if (!isoString) return '—'
  const then = new Date(isoString)
  const diffMins = Math.floor((new Date() - then) / 60000)
  if (diffMins < 60) return `${diffMins}m ago`
  const diffHours = Math.floor(diffMins / 60)
  if (diffHours < 24) return `${diffHours}h ago`
  const diffDays = Math.floor(diffHours / 24)
  if (diffDays === 1) return 'Yesterday'
  if (diffDays < 7) return `${diffDays} days ago`
  return formatDateOnly(isoString)
}

function memberCounts(group) {
  return {
    active: group.members.filter((m) => m.memberStatus === 'active').length,
    pending: group.members.filter((m) => m.memberStatus === 'pending').length,
    suspended: group.members.filter((m) => m.memberStatus === 'suspended').length,
  }
}

function groupStatusClass(status) {
  switch (status) {
    case 'Active': return 'badge-green'
    case 'Pending Approval': return 'badge-amber'
    case 'Pending Termination': return 'badge-amber'
    case 'Suspended': return 'badge-red'
    case 'Terminated': return 'badge-red'
    default: return 'badge-neutral'
  }
}

function accountTypeClass(type) {
  return type === 'Premium' ? 'badge-purple' : 'badge-blue'
}

function paymentStatusClass(status) {
  switch (status) {
    case 'Paid': return 'badge-green'
    case 'Not Required': return 'badge-neutral'
    case 'Overdue': return 'badge-red'
    default: return 'badge-neutral'
  }
}
</script>

<template>
  <div class="groups-page">
    <!-- ============================================================ -->
    <!-- FIXED TOP SECTION                                             -->
    <!-- ============================================================ -->
    <div class="groups-fixed-top">
      <div class="page-title-row">
        <div>
          <h1>Groups</h1>
          <p class="page-subtitle">Manage and monitor all groups on the FinBine platform.</p>
        </div>

        <div class="title-actions">
          <div class="search-box">
            <Search size="12" class="search-icon" />
            <input
              v-model="searchTerm"
              type="text"
              placeholder="Search by Group ID, Group Name, Owner, Email or Member..."
            />
          </div>
          <button class="btn-outline">
            <SlidersHorizontal size="12" /> Filters
          </button>
          <button class="btn-primary">
            <Download size="12" /> Export
          </button>
        </div>
      </div>

      <div class="summary-grid">
        <div v-for="card in summaryCards" :key="card.key" class="summary-card">
          <div class="summary-icon" :class="card.color">
            <component :is="card.icon" size="14" />
          </div>
          <div class="summary-text">
            <p class="summary-label">{{ card.label }}</p>
            <p class="summary-value">{{ card.value }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Click-outside layer to close an open row menu -->
    <div v-if="openMenuId" class="menu-overlay" @click="closeMenu"></div>

    <!-- ============================================================ -->
    <!-- SCROLLABLE GROUP CARD LIST                                    -->
    <!-- ============================================================ -->
    <div class="groups-scroll-body">
      <div
        v-for="group in filteredGroups"
        :key="group.groupId"
        class="group-card"
        @click="openDrawer(group)"
      >
        <div class="card-top">
          <div class="card-title">
            <span class="group-id">{{ group.groupId }}</span>
            <span class="group-name">{{ group.groupName }}</span>
          </div>

          <div class="card-badges">
            <span class="badge" :class="accountTypeClass(group.accountType)">{{ group.accountType }}</span>
            <span class="badge" :class="paymentStatusClass(group.subscription.paymentStatus)">{{ group.subscription.paymentStatus }}</span>
            <span class="badge" :class="groupStatusClass(group.groupStatus)">{{ group.groupStatus }}</span>

            <div class="menu-wrapper" @click.stop>
              <button class="menu-btn" @click="toggleMenu(group.groupId)">
                <MoreVertical size="15" />
              </button>
              <div v-if="openMenuId === group.groupId" class="dropdown-menu">
                <button @click="suspendGroup(group)">Suspend Group</button>
                <button @click="terminateGroup(group)">Terminate Group</button>
                <button @click="viewAuditLog()">View Audit Log</button>
              </div>
            </div>
          </div>
        </div>

        <div class="card-details">
          <div class="detail-chip">
            <span class="chip-label">Group Owner</span>
            <span class="chip-value">{{ group.owner.firstName }} {{ group.owner.lastName }}<br /><span class="chip-sub">{{ group.owner.email }}</span></span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Members</span>
            <span class="chip-value member-dots">
              <span class="dot green"></span>{{ memberCounts(group).active }}
              <span class="dot amber"></span>{{ memberCounts(group).pending }}
              <span class="dot red"></span>{{ memberCounts(group).suspended }}
            </span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Country</span>
            <span class="chip-value">{{ group.groupCountry }}</span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Currency</span>
            <span class="chip-value">{{ group.groupCurrency }}</span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Time Zone</span>
            <span class="chip-value">{{ group.groupTimezone }}</span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Subscription</span>
            <span class="chip-value">{{ formatDateOnly(group.subscription.startDate) }} → {{ group.subscription.endDate ? formatDateOnly(group.subscription.endDate) : '—' }}</span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Created</span>
            <span class="chip-value">{{ formatDateOnly(group.createdAt) }}</span>
          </div>
          <div class="detail-chip">
            <span class="chip-label">Last Activity</span>
            <span class="chip-value">{{ timeAgo(group.lastActivity) }}</span>
          </div>
        </div>
      </div>

      <p v-if="filteredGroups.length === 0" class="no-results">No groups match your search.</p>
    </div>

    <GroupMembersDrawer :open="drawerOpen" :group="selectedGroup" @close="closeDrawer" />
  </div>
</template>

<style scoped>
.groups-page {
  height: 100vh;
  display: flex;
  flex-direction: column;
  background: #F8FAFC;
  overflow: hidden;
  font-size: 13px;
  position: relative;
}

/* ---------- Fixed top section ---------- */
.groups-fixed-top {
  flex-shrink: 0;
  padding: 10px 16px 8px 16px;
  background: #F8FAFC;
}

.page-title-row {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  margin-bottom: 8px;
  gap: 16px;
  flex-wrap: wrap;
}

.page-title-row h1 {
  font-size: 1.1rem;
  font-weight: 700;
  color: #0F172A;
  margin: 0;
}

.page-subtitle {
  font-size: 0.68rem;
  color: #64748B;
  margin: 2px 0 0 0;
}

.title-actions {
  display: flex;
  gap: 8px;
  align-items: center;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 5px 10px;
  min-width: 260px;
}

.search-box input {
  border: none;
  outline: none;
  font-size: 0.7rem;
  flex: 1;
  color: #0F172A;
}

.search-icon {
  color: #94A3B8;
}

.btn-outline,
.btn-primary {
  display: flex;
  align-items: center;
  gap: 5px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 600;
  padding: 6px 10px;
  cursor: pointer;
  white-space: nowrap;
}

.btn-outline {
  background: #fff;
  border: 1px solid #E2E8F0;
  color: #334155;
}

.btn-primary {
  background: #2563EB;
  border: none;
  color: #fff;
}

/* ---------- Summary cards ---------- */
.summary-grid {
  display: grid;
  grid-template-columns: repeat(6, 1fr);
  gap: 8px;
}

.summary-card {
  background: #fff;
  border-radius: 7px;
  border: 1px solid #E5E7EB;
  padding: 7px 9px;
  display: flex;
  align-items: center;
  gap: 8px;
}

.summary-icon {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
  color: #fff;
}

.summary-icon.blue { background: #2563EB; }
.summary-icon.green { background: #059669; }
.summary-icon.purple { background: #7C3AED; }
.summary-icon.teal { background: #0D9488; }
.summary-icon.amber { background: #D97706; }
.summary-icon.red { background: #DC2626; }

.summary-label {
  font-size: 0.62rem;
  color: #64748B;
  margin: 0;
  font-weight: 600;
  white-space: nowrap;
}

.summary-value {
  font-size: 0.92rem;
  font-weight: 700;
  color: #0F172A;
  margin: 1px 0 0 0;
}

/* ---------- Menu click-outside overlay ---------- */
.menu-overlay {
  position: fixed;
  inset: 0;
  z-index: 40;
}

/* ---------- Scrollable body ---------- */
.groups-scroll-body {
  flex: 1;
  overflow-y: auto;
  padding: 0 16px 16px 16px;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

/* ---------- Group card ---------- */
.group-card {
  background: #fff;
  border: 1px solid #E5E7EB;
  border-radius: 8px;
  padding: 10px 14px;
  cursor: pointer;
  transition: border-color 0.15s, box-shadow 0.15s;
}

.group-card:hover {
  border-color: #CBD5E1;
  box-shadow: 0 1px 4px rgba(0,0,0,0.06);
}

.card-top {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 8px;
}

.card-title {
  display: flex;
  align-items: baseline;
  gap: 10px;
}

.group-id {
  color: #2563EB;
  font-weight: 700;
  font-size: 0.78rem;
}

.group-name {
  font-weight: 700;
  font-size: 0.85rem;
  color: #0F172A;
}

.card-badges {
  display: flex;
  align-items: center;
  gap: 6px;
  position: relative;
}

/* ---------- Row 3-dot menu ---------- */
.menu-wrapper {
  position: relative;
}

.menu-btn {
  background: none;
  border: none;
  color: #64748B;
  cursor: pointer;
  padding: 3px;
  display: flex;
  border-radius: 4px;
}

.menu-btn:hover {
  background: #F1F5F9;
}

.dropdown-menu {
  position: absolute;
  top: 26px;
  right: 0;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  box-shadow: 0 4px 14px rgba(0,0,0,0.1);
  min-width: 160px;
  z-index: 50;
  overflow: hidden;
}

.dropdown-menu button {
  display: block;
  width: 100%;
  text-align: left;
  padding: 8px 12px;
  background: none;
  border: none;
  font-size: 0.72rem;
  color: #1E293B;
  cursor: pointer;
}

.dropdown-menu button:hover {
  background: #F8FAFC;
}

/* ---------- Detail chips (wrap freely, never squeezed) ---------- */
.card-details {
  display: flex;
  flex-wrap: wrap;
  gap: 14px 22px;
  border-top: 1px solid #F1F5F9;
  padding-top: 8px;
}

.detail-chip {
  display: flex;
  flex-direction: column;
  gap: 2px;
  min-width: 110px;
}

.chip-label {
  font-size: 0.6rem;
  font-weight: 700;
  color: #94A3B8;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.chip-value {
  font-size: 0.74rem;
  color: #1E293B;
  font-weight: 600;
  line-height: 1.4;
}

.chip-sub {
  font-size: 0.66rem;
  color: #64748B;
  font-weight: 500;
}

.member-dots {
  display: flex;
  align-items: center;
  gap: 3px;
}

.dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  margin-left: 6px;
}

.dot:first-child { margin-left: 0; }
.dot.green { background: #10B981; }
.dot.amber { background: #F59E0B; }
.dot.red { background: #EF4444; }

/* ---------- Badges ---------- */
.badge {
  display: inline-block;
  font-size: 0.64rem;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 10px;
  white-space: nowrap;
}

.badge-green { background: #DCFCE7; color: #15803D; }
.badge-amber { background: #FEF3C7; color: #B45309; }
.badge-red { background: #FEE2E2; color: #B91C1C; }
.badge-blue { background: #DBEAFE; color: #1D4ED8; }
.badge-purple { background: #EDE9FE; color: #6D28D9; }
.badge-neutral { background: #F1F5F9; color: #475569; }

.no-results {
  text-align: center;
  color: #64748B;
  font-size: 0.75rem;
  padding: 30px 0;
}
</style>