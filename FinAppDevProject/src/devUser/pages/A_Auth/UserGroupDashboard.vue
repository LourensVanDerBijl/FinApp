<script setup>
// ─────────────────────────────────────────────────────────────────────────
// User Group Dashboard — v3, cockpit header + click-through detail.
//
// v2 fixed the scrolling problem with a fixed-height shell and denser
// cards. v3 goes further on two fronts:
//   1. Header stripped to logo + group name + status only — no avatar,
//      no name banner, no logout button competing for attention up top.
//      Identity (the "signed in as" label) moved to a small caption under
//      the sidebar logo; logout moved to the bottom of the sidebar nav.
//   2. CA1/CA2/CA3 are now compact, clickable rows — full detail (every
//      field from the source spec) lives one click away in a shared
//      slide-over drawer, rather than being permanently open in a padded
//      card. Nothing was removed, only reorganized behind a click.
// ─────────────────────────────────────────────────────────────────────────
import { computed } from 'vue'
import { useRouter } from 'vue-router'
import { signOut } from 'firebase/auth'
import { auth } from '../../../firebase/firebaseManager.js'
import { currentUserProfile, clearUserProfile } from '../../data/userSession.js'
import { useFinancialStatus } from '../../composables/A_Auth/useFinancialStatus.js'
import { financialStatusInputs } from '../../data/financialMockData.js'

import logo from '../../../assets/SVG/logo.svg'
import DashboardSidebarNav from '../../components/A_Auth/UserGroupDashboard/DashboardSidebarNav.vue'
import DashboardKpiStrip from '../../components/A_Auth/UserGroupDashboard/DashboardKpiStrip.vue'
import ChangesActionsPanel from '../../components/A_Auth/UserGroupDashboard/ChangesActionsPanel.vue'
import MyGroupPanel from '../../components/A_Auth/UserGroupDashboard/MyGroupPanel.vue'
import MyFinancesPanel from '../../components/A_Auth/UserGroupDashboard/MyFinancesPanel.vue'

const router = useRouter()
const { tier, percentRemaining } = useFinancialStatus(financialStatusInputs)

async function handleLogout() {
  await signOut(auth)
  clearUserProfile()
  router.push('/user/login')
}
</script>

<template>
  <div class="dashboard-shell">
    <!-- ============================= SIDEBAR ============================= -->
    <aside class="sidebar">
      <div class="sidebar-brand">
        <img :src="logo" alt="FinBine" class="sidebar-logo" />
        <div class="sidebar-brand-text">
          <span class="sidebar-wordmark">FinBine</span>
          <span v-if="currentUserProfile?.displayName" class="sidebar-signed-in">
            {{ currentUserProfile.displayName }}
          </span>
        </div>
      </div>
      <DashboardSidebarNav @logout="handleLogout" />
    </aside>

    <!-- ============================== MAIN =============================== -->
    <div class="dashboard-main">
      <!-- ===================== MINIMAL COCKPIT HEADER ===================== -->
      <header class="cockpit-header">
        <div class="cockpit-brand">
          <img :src="logo" alt="FinBine" class="cockpit-logo" />
          <span>FinBine</span>
        </div>
        <span v-if="currentUserProfile?.groupName" class="cockpit-group">
          {{ currentUserProfile.groupName }}
        </span>
        <span class="cockpit-status" :class="tier.key">
          {{ tier.emoji }} {{ tier.label }} · {{ percentRemaining.toFixed(0) }}%
        </span>
      </header>

      <DashboardKpiStrip />

      <div class="dashboard-columns">
        <div class="dashboard-col"><ChangesActionsPanel /></div>
        <div class="dashboard-col"><MyGroupPanel /></div>
        <div class="dashboard-col"><MyFinancesPanel /></div>
      </div>
    </div>
  </div>
</template>

<style scoped>
* {
  box-sizing: border-box;
}

.dashboard-shell {
  display: flex;
  height: 100vh;
  background: #f8fafc;
  font-family: system-ui, 'Segoe UI', Roboto, sans-serif;
  overflow: hidden;
}

/* ============================= SIDEBAR ============================= */
.sidebar {
  width: 208px;
  flex-shrink: 0;
  background: linear-gradient(165deg, #0d1728 0%, #0b1220 60%, #0a0f1c 100%);
  padding: 16px 14px;
  display: flex;
  flex-direction: column;
  gap: 18px;
}

.sidebar-brand {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 0 6px;
}

.sidebar-logo {
  height: 26px;
  width: auto;
  flex-shrink: 0;
}

.sidebar-brand-text {
  display: flex;
  flex-direction: column;
  min-width: 0;
}

.sidebar-wordmark {
  font-size: 1.02rem;
  font-weight: 700;
  color: #fff;
  line-height: 1.2;
}

.sidebar-signed-in {
  font-size: 0.66rem;
  color: #64748b;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

/* =============================== MAIN =============================== */
.dashboard-main {
  flex: 1;
  min-width: 0;
  height: 100%;
  padding: 14px 20px 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
  min-height: 0;
}

/* ========================= COCKPIT HEADER ========================= */
.cockpit-header {
  display: flex;
  align-items: center;
  gap: 14px;
  flex-shrink: 0;
}

.cockpit-brand {
  display: flex;
  align-items: center;
  gap: 7px;
}

.cockpit-logo {
  height: 20px;
  width: auto;
}

.cockpit-brand span {
  font-size: 0.94rem;
  font-weight: 800;
  color: #0f172a;
}

.cockpit-group {
  font-size: 0.8rem;
  color: #64748b;
  font-weight: 600;
  padding-left: 14px;
  border-left: 1px solid #e2e8f0;
}

.cockpit-status {
  margin-left: auto;
  display: flex;
  align-items: center;
  font-size: 0.76rem;
  font-weight: 700;
  padding: 5px 12px;
  border-radius: 999px;
}

.cockpit-status.excellent {
  background: #eff6ff;
  color: #1855b9;
}
.cockpit-status.good {
  background: #f0fdf4;
  color: #16a34a;
}
.cockpit-status.watch {
  background: #fff7ed;
  color: #c2410c;
}
.cockpit-status.critical,
.cockpit-status.danger {
  background: #fef2f2;
  color: #dc2626;
}

.dashboard-columns {
  display: grid;
  grid-template-columns: 1fr 1fr 1fr;
  gap: 14px;
  flex: 1;
  min-height: 0;
}

.dashboard-col {
  min-height: 0;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}

.dashboard-col::-webkit-scrollbar {
  width: 6px;
}
.dashboard-col::-webkit-scrollbar-thumb {
  background: #e2e8f0;
  border-radius: 999px;
}
.dashboard-col::-webkit-scrollbar-track {
  background: transparent;
}

/* ============================================================ */
/* RESPONSIVE                                                    */
/* ============================================================ */
@media (max-width: 1180px) {
  .dashboard-shell {
    height: auto;
    min-height: 100vh;
    overflow: visible;
  }

  .dashboard-main {
    height: auto;
    min-height: 0;
  }

  .dashboard-columns {
    grid-template-columns: 1fr 1fr;
    height: auto;
  }

  .dashboard-col {
    overflow-y: visible;
  }
}

@media (max-width: 900px) {
  .sidebar {
    display: none;
  }

  .dashboard-main {
    padding: 14px 16px 24px;
  }

  .dashboard-columns {
    grid-template-columns: 1fr;
  }

  .cockpit-group {
    display: none;
  }
}
</style>
