<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import ProgressBar from '../../sharedComponents/ProgressBar.vue'

const props = defineProps({
  resources: {
    type: Array,
    required: true
  }
})

function formatValue(val) {
  if (typeof val === 'number') {
    return val.toLocaleString()
  }
  return val
}
</script>

<template>
  <SectionCard>
    <h3 class="section-title">System Resources</h3>
    <div class="system-scroll">
      <div class="system-list">
        <div v-for="provider in resources" :key="provider.name" class="provider-group">
          <h4 class="provider-name">{{ provider.name }}</h4>
          <div v-for="res in provider.resources" :key="res.name" class="resource-row">
            <span class="resource-name">{{ res.name }}</span>
            <span class="resource-usage">
              {{ formatValue(res.value) }} / {{ formatValue(res.max) }} {{ res.unit }}
            </span>
            <ProgressBar :value="res.value" :max="res.max" />
          </div>
        </div>
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.section-title {
  font-size: 13px;
  color: #eee9e9;
  background-color: rgba(6, 30, 57, 0.93);
  padding: 0 0 0 4px;
  border-radius: 4px;
  font-weight: 600;
  margin-bottom: 6px;
  display: block;
}

.system-scroll {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 320px; /* slightly reduced height */
}

.system-list {
  display: flex;
  flex-direction: column;
  gap: 8px; /* ✅ tighter spacing between provider groups */
}

.provider-group {
  display: flex;
  flex-direction: column;
  gap: 4px; /* ✅ reduced spacing inside group */
  padding-bottom: 6px;
  border-bottom: 1px solid #E5E7EB;
}

.provider-group:last-child {
  border-bottom: none;
}

.provider-name {
  font-size: 12px;
  font-weight: 600;
  color: #334155;
  margin: 0 0 2px 0;
}

.resource-row {
  display: grid;
  grid-template-columns: 1fr auto 90px; /* ✅ narrower bar column */
  align-items: center;
  gap: 4px;
  font-size: 10px;
  color: #374151;
}

.resource-name {
  font-weight: 500;
}

.resource-usage {
  font-size: 10px;
  color: #475569;
  text-align: right;
}
</style>
