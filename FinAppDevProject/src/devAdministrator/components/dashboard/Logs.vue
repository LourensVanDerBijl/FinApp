<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'


const props = defineProps({
  logs: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <SectionCard>
    <h3>Recent System Logs</h3>
    <SystemLogTable :logs="logs" />
  </SectionCard>
</template>
