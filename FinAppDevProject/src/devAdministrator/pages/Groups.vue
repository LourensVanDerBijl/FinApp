<script setup>
import SectionCard from '../sharedComponents/SectionCard.vue'
import StatusBadge from '../sharedComponents/StatusBadge.vue'

// Example placeholder data
const groups = [
  { name: 'Finance Team', members: 12, status: 'Active' },
  { name: 'Support Staff', members: 8, status: 'Active' },
  { name: 'Developers', members: 5, status: 'Inactive' }
]
</script>

<template>
  <SectionCard>
    <h3>Groups</h3>
    <div class="group-list">
      <div v-for="group in groups" :key="group.name" class="group-item">
        <div class="group-header">
          <p class="group-name">{{ group.name }}</p>
          <StatusBadge :status="group.status" />
        </div>
        <p class="group-members">{{ group.members }} members</p>
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.group-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.group-item {
  background: #f9fafb;
  padding: 12px;
  border-radius: 8px;
}
.group-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.group-name {
  font-weight: 600;
}
.group-members {
  font-size: 14px;
  color: #4b5563;
}
</style>
