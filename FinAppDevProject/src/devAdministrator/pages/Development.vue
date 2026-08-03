<script setup>
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'

// Example placeholder data
const devServices = [
  { name: 'Feature Flags', status: 'Enabled' },
  { name: 'Beta Testing', status: 'Active' },
  { name: 'CI/CD Pipeline', status: 'Running' },
  { name: 'Error Tracking', status: 'Connected' }
]
</script>

<template>
  <SectionCard>
    <h3>Development Tools</h3>
    <div class="dev-list">
      <div v-for="service in devServices" :key="service.name" class="dev-item">
        <p class="dev-name">{{ service.name }}</p>
        <StatusBadge :status="service.status" />
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.dev-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.dev-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: #f9fafb;
  padding: 8px 12px;
  border-radius: 6px;
}
.dev-name {
  font-weight: 500;
}
</style>
