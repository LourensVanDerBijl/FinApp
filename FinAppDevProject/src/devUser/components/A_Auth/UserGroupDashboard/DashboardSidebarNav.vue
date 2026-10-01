<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Left navigation. Logout now lives here, at the bottom — present, but
// not shouting from the top of the page the way a prominent top-bar
// button does. Only "Dashboard" is a real, built page right now — the
// rest of the sections named in the product wireframe are future Epic 2
// work, so they render as a visible, honest nav shell rather than links
// that would 404.
// ─────────────────────────────────────────────────────────────────────────
import {
  LayoutDashboard,
  ScrollText,
  ArrowLeftRight,
  FileEdit,
  CalendarClock,
  UserCog,
  History,
  LogOut
} from 'lucide-vue-next'

defineEmits(['logout'])

const navItems = [
  { key: 'dashboard', label: 'Dashboard', icon: LayoutDashboard, active: true },
  { key: 'group-policy', label: 'Group Policy', icon: ScrollText, active: false },
  { key: 'group-finance-change', label: 'Group Finance Change', icon: ArrowLeftRight, active: false },
  { key: 'group-policy-changes', label: 'Group Policy Changes', icon: FileEdit, active: false },
  { key: 'payment-dates', label: 'Payment Dates', icon: CalendarClock, active: false },
  { key: 'my-financial-policy', label: 'My Financial Policy', icon: UserCog, active: false },
  { key: 'my-payment-history', label: 'My Payment History', icon: History, active: false }
]
</script>

<template>
  <nav class="sidebar-nav">
    <ul class="nav-list">
      <li v-for="item in navItems" :key="item.key">
        <button
          type="button"
          class="nav-item"
          :class="{ active: item.active }"
          :disabled="!item.active"
        >
          <component :is="item.icon" :size="15" class="nav-icon" />
          <span>{{ item.label }}</span>
          <span v-if="!item.active" class="soon-badge">Soon</span>
        </button>
      </li>
    </ul>

    <div class="nav-footer">
      <button type="button" class="logout-btn" @click="$emit('logout')">
        <LogOut :size="14" />
        <span>Log out</span>
      </button>
    </div>
  </nav>
</template>

<style scoped>
.sidebar-nav {
  width: 100%;
  flex: 1;
  display: flex;
  flex-direction: column;
  min-height: 0;
}

.nav-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 2px;
  flex: 1;
}

.nav-item {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 9px;
  padding: 8px 10px;
  border: none;
  border-radius: 7px;
  background: transparent;
  color: #94a3b8;
  font-family: inherit;
  font-size: 0.78rem;
  font-weight: 600;
  text-align: left;
  cursor: not-allowed;
  transition: background 0.15s, color 0.15s;
}

.nav-icon {
  flex-shrink: 0;
}

.nav-item.active {
  background: rgba(45, 212, 191, 0.12);
  color: #fff;
  cursor: default;
}

.nav-item.active .nav-icon {
  color: #2dd4bf;
}

.soon-badge {
  margin-left: auto;
  font-size: 0.6rem;
  font-weight: 700;
  letter-spacing: 0.02em;
  text-transform: uppercase;
  color: #64748b;
  background: rgba(255, 255, 255, 0.06);
  border: 1px solid rgba(255, 255, 255, 0.08);
  border-radius: 999px;
  padding: 2px 6px;
  flex-shrink: 0;
}

.nav-footer {
  flex-shrink: 0;
  padding-top: 10px;
  border-top: 1px solid rgba(255, 255, 255, 0.08);
}

.logout-btn {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 9px;
  padding: 8px 10px;
  border: none;
  border-radius: 7px;
  background: transparent;
  color: #64748b;
  font-family: inherit;
  font-size: 0.76rem;
  font-weight: 600;
  cursor: pointer;
  transition: background 0.15s, color 0.15s;
}

.logout-btn:hover {
  background: rgba(255, 255, 255, 0.06);
  color: #cbd5e1;
}
</style>
