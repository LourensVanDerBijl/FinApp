<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'

const props = defineProps({
  services: Array
})
</script>

<template>
  <SectionCard>
    <h3>Platform Health</h3>
    <ul>
      <li v-for="(service, index) in services" :key="index">
        {{ service.name }} — <StatusBadge :status="service.status" />
      </li>
    </ul>
  </SectionCard>
</template>
