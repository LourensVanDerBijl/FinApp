<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'
import ApexChart from 'vue3-apexcharts'

const props = defineProps({
  data: Object
})

const chartOptions = {
  chart: { type: 'donut' },
  labels: props.data.labels,
  legend: { position: 'bottom' }
}
</script>

<template>
  <SectionCard>
    <h3>Group Subscription Overview</h3>
    <apexchart type="donut" :options="chartOptions" :series="props.data.series" height="300" />
  </SectionCard>
</template>
