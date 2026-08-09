<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import SystemLogTable from '../../sharedComponents/SystemLogTable.vue'

const props = defineProps({
  logs: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <SectionCard>
    <!-- ✅ Heading styled same as System Resources -->
    <h3 class="section-title">Recent System Logs</h3>
    <div class="logs-scroll">
      <SystemLogTable :logs="logs" />
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
  margin-bottom: 0px !important; /* ✅ remove extra gap before table headers */
  display: block;
}

.logs-scroll {
  flex: 1;
  overflow-y: auto;
  overflow-x: hidden;
  max-height: 400px;
}
</style>
