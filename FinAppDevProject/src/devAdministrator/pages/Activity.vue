<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'

const props = defineProps({
  activities: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <SectionCard>
    <h3>Recent Activity</h3>
    <ActivityList :activities="activities" />
  </SectionCard>
</template>
