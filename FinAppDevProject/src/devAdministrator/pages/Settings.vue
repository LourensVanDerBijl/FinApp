<script setup>
import SectionCard from '../sharedComponents/SectionCard.vue'
import StatusBadge from '../sharedComponents/StatusBadge.vue'

// Example placeholder settings
const settings = [
  { name: 'Dark Mode', status: 'Enabled' },
  { name: 'Email Notifications', status: 'Disabled' },
  { name: 'Two-Factor Authentication', status: 'Enabled' },
  { name: 'Auto-Updates', status: 'Enabled' }
]
</script>

<template>
  <SectionCard>
    <h3>System Settings</h3>
    <div class="settings-list">
      <div v-for="setting in settings" :key="setting.name" class="settings-item">
        <p class="setting-name">{{ setting.name }}</p>
        <StatusBadge :status="setting.status" />
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.settings-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.settings-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f9fafb;
  padding: 8px 12px;
  border-radius: 6px;
}
.setting-name {
  font-weight: 500;
}
</style>
