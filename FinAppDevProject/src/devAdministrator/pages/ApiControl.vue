<script setup>
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'

// Example placeholder data until you wire in real mockData.js
const apiKeys = [
  { name: 'Primary API Key', status: 'Active', value: 'sk_live_1234567890' },
  { name: 'Secondary API Key', status: 'Revoked', value: 'sk_test_0987654321' }
]
</script>

<template>
  <SectionCard>
    <h3>API Control</h3>
    <div class="api-list">
      <div v-for="key in apiKeys" :key="key.value" class="api-item">
        <div class="api-header">
          <p class="api-name">{{ key.name }}</p>
          <StatusBadge :status="key.status" />
        </div>
        <p class="api-value">{{ key.value }}</p>
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.api-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.api-item {
  background: #f9fafb;
  padding: 12px;
  border-radius: 8px;
}
.api-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.api-name {
  font-weight: 600;
}
.api-value {
  font-family: monospace;
  font-size: 14px;
  margin-top: 4px;
}
</style>
