<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'


const props = defineProps({
  systems: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <SectionCard>
    <h3>System Overview</h3>
    <div class="system-list">
      <div v-for="system in systems" :key="system.name" class="system-item">
        <p class="system-name">{{ system.name }}</p>
        <ProgressBar :value="system.value" :max="system.max" />
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.system-list {
  display: flex;
  flex-direction: column;
  gap: 16px;
}
.system-item {
  display: flex;
  flex-direction: column;
  gap: 4px;
}
.system-name {
  font-size: 14px;
  font-weight: 500;
}
</style>
