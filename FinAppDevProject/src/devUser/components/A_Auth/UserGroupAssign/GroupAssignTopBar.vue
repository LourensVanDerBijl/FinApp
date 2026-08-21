<script setup>
import logo from '../../../../assets/SVG/logo.svg'
import { LogOut } from 'lucide-vue-next'

defineProps({
  userName: { type: String, default: 'Account' },
  userInitials: { type: String, default: '?' }
})

const emit = defineEmits(['logout'])
</script>

<template>
  <div class="top-bar">
    <div class="mobile-brand-row">
      <img :src="logo" alt="FinBine" class="mobile-brand-logo" />
      <span class="mobile-brand-wordmark">FinBine</span>
    </div>

    <div class="user-menu">
      <div class="user-avatar">{{ userInitials }}</div>
      <span class="user-name">{{ userName }}</span>
      <button type="button" class="logout-btn" @click="emit('logout')" aria-label="Logout">
        <LogOut :size="14" />
      </button>
    </div>
  </div>
</template>

<style scoped>
.top-bar {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  flex-shrink: 0;
  margin-bottom: 6px;
}

.mobile-brand-row {
  display: none;
  align-items: center;
  gap: 8px;
}

.mobile-brand-logo {
  height: 24px;
}

.mobile-brand-wordmark {
  font-size: 1rem;
  font-weight: 700;
  color: #0f172a;
}

.user-menu {
  display: flex;
  align-items: center;
  gap: 8px;
}

.user-avatar {
  width: 26px;
  height: 26px;
  border-radius: 50%;
  background: rgba(37, 99, 235, 0.1);
  border: 1px solid rgba(37, 99, 235, 0.3);
  color: #1855b9;
  font-size: 0.62rem;
  font-weight: 700;
  display: flex;
  align-items: center;
  justify-content: center;
}

.user-name {
  font-size: 0.76rem;
  font-weight: 600;
  color: #334155;
}

.logout-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 26px;
  height: 26px;
  border-radius: 50%;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #64748b;
  cursor: pointer;
  transition: border-color 0.15s, background 0.15s;
}

.logout-btn:hover {
  border-color: #cbd5e1;
  background: #f8fafc;
}

@media (max-width: 960px) {
  .top-bar {
    justify-content: space-between;
  }

  .mobile-brand-row {
    display: flex;
  }

  .user-name {
    display: none;
  }
}
</style>
