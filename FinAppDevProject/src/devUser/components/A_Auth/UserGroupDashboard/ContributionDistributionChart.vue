<script setup>
// Compact horizontal layout — small donut + inline legend, rather than a
// large chart stacked above a separate legend list. Same ApexCharts
// approach as the Admin dashboard, just sized for a dense panel.
import { computed } from 'vue'
import ApexChart from 'vue3-apexcharts'

const props = defineProps({
  distribution: { type: Object, required: true } // { labels: string[], series: number[] }
})

const COLORS = ['#1855b9', '#2dd4bf']

const chartOptions = computed(() => ({
  chart: { type: 'donut', toolbar: { show: false } },
  labels: props.distribution.labels,
  colors: COLORS,
  stroke: { width: 2, colors: ['#fff'] },
  dataLabels: { enabled: false },
  legend: { show: false },
  plotOptions: {
    pie: {
      donut: {
        size: '68%',
        labels: {
          show: true,
          name: { show: false },
          value: {
            show: true,
            fontSize: '13px',
            color: '#0f172a',
            fontWeight: 800,
            offsetY: 4,
            formatter: (val) => `${Number(val).toFixed(0)}%`
          },
          total: { show: false }
        }
      }
    }
  }
}))
</script>

<template>
  <div class="distribution-chart">
    <div class="chart-wrapper">
      <ApexChart type="donut" height="92" width="92" :options="chartOptions" :series="distribution.series" />
    </div>
    <ul class="legend">
      <li v-for="(label, i) in distribution.labels" :key="label">
        <span class="dot" :style="{ background: COLORS[i] }"></span>
        <span class="legend-label">{{ label }}</span>
        <span class="legend-value">{{ distribution.series[i].toFixed(1) }}%</span>
      </li>
    </ul>
  </div>
</template>

<style scoped>
.distribution-chart {
  display: flex;
  align-items: center;
  gap: 14px;
}

.chart-wrapper {
  flex-shrink: 0;
  width: 92px;
}

.legend {
  list-style: none;
  margin: 0;
  padding: 0;
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.legend li {
  display: flex;
  align-items: center;
  gap: 6px;
  font-size: 0.74rem;
}

.dot {
  width: 7px;
  height: 7px;
  border-radius: 50%;
  flex-shrink: 0;
}

.legend-label {
  flex: 1;
  color: #334155;
  font-weight: 600;
}

.legend-value {
  color: #0f172a;
  font-weight: 700;
}
</style>
