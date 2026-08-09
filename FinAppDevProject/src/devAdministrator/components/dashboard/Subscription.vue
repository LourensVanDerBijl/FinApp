<script setup>
import { defineProps, computed } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import ApexChart from 'vue3-apexcharts'
import ApexCharts from 'apexcharts'

const props = defineProps({
  data: { type: Object, required: true }
})

const seriesPercent = computed(() => {
  const total = props.data.series.reduce((a, b) => a + b, 0)
  return props.data.series.map(v => (v / total) * 100)
})

const chartOptions = computed(() => ({
  chart: {
    id: 'subscription-chart',
    type: 'donut',
    height: 180,
    toolbar: { show: false },
    offsetX: -30,
    dropShadow: { // ✅ subtle shadow
      enabled: true,
      top: 1,
      left: 0,
      blur: 3,
      opacity: 0.15
    }
  },
  grid: { padding: { left: 0, right: 0, top: 0, bottom: 0 } },
  labels: props.data.labels,
  colors: ['#2F5DA9', '#157347'], // ✅ muted enterprise sapphire + emerald
  stroke: {
    width: 3,
    colors: ['#FFFFFF'] // ✅ clean white separator
  },
  dataLabels: { enabled: false },
  legend: { show: false },
  plotOptions: {
    pie: {
      customScale: 0.85,
      donut: {
        size: '76%', // ✅ thinner ring
        labels: {
          show: true,
          name: { show: true, fontSize: '12px', color: '#000' },
          value: { show: true, fontSize: '21px', color: '#000', fontWeight: 800 },
          total: {
            show: true,
            label: 'Total Groups',
            fontSize: '10px',
            color: '#000',
            formatter: () => props.data.total
          }
        }
      }
    }
  }
}))

// ✅ Hover: dim other slice
function highlightSeries(index) {
  const colors = ['#2F5DA9', '#157347']
  const faded = ['rgba(47,93,169,.25)', 'rgba(21,115,71,.25)']
  ApexCharts.exec('subscription-chart', 'updateOptions', {
    colors: colors.map((c, i) => (i === index ? c : faded[i]))
  })
}
function resetSeries() {
  ApexCharts.exec('subscription-chart', 'updateOptions', {
    colors: ['#2F5DA9', '#157347']
  })
}
</script>

<template>
  <SectionCard>
    <h3 class="section-title">Group Subscription Overview</h3>

    <div class="chart-container">
      <div class="chart-wrapper">
        <ApexChart
          type="donut"
          :options="chartOptions"
          :series="props.data.series"
          height="180"
        />
      </div>

      <div class="custom-legend">
        <div
          v-for="(label, i) in props.data.labels"
          :key="label"
          :class="['legend-item', i === 0 ? 'premium' : 'free']"
          @mouseenter="highlightSeries(i)"
          @mouseleave="resetSeries()"
        >
          <div class="legend-bar"></div>
          <div class="legend-content">
            <div class="legend-top">
              <span class="legend-title">{{ label }}</span>
              <span class="legend-badge">{{ seriesPercent[i].toFixed(0) }}%</span>
            </div>
            <div class="legend-count">{{ props.data.series[i] }} Groups</div>
          </div>
        </div>
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.section-title {
  font-size: 13px;
  font-weight: 600;
  color: #eee9e9;
  margin: 0;
  padding-left: 4px;
  border-radius: 4px;
  text-align: left;
  background: #0F2745; /* ✅ executive header */
}

.chart-container {
  display: flex;
  align-items: center;
  gap: 4px;
  width: 210px;
}

.chart-wrapper {
  width: 210px;
  flex-shrink: 0;
  margin-right: -60px;
}

.custom-legend {
  display: flex;
  flex-direction: column;
  gap: 15px;
  width: 200px;
  flex-shrink: 0;
}

.legend-item {
  display: flex;
  align-items: center;
  border-radius: 8px;
  overflow: hidden;
  transition: all .2s ease;
  cursor: pointer;
  background: #a092dc1e;
  border: 1px solid #e5e7eb72;
}

.legend-item:hover {
  transform: translateX(3px);
  box-shadow: 0 6px 14px rgba(15,23,42,.08);
}

.legend-bar {
  width: 5px;
  align-self: stretch;
}

.premium .legend-bar {
  background: #244B8A; /* ✅ sapphire border */
}
.free .legend-bar {
  background: #105C39; /* ✅ emerald border */
}

.legend-content {
  flex: 1;
  line-height: 1.0;
  padding: 8px 10px;
}

.legend-top {
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.legend-title {
  font-size: 10px;
  font-weight: 600;
  color: #314155;
}

.legend-count {
  margin-top: 2px;
  font-size: 18px;
  font-weight: 700;
  color: #1E293B;
}

.legend-badge {
  font-size: 12px;
  font-weight: 700;
  padding: 2px 8px;
  border-radius: 999px;
  color: #fff;
}

.premium .legend-badge {
  background: #fdfeff02;
  color: #244B8A; /* ✅ sapphire highlight */
}
.free .legend-badge {
  background: #24985c00; /* ✅ emerald highlight */
  color: #24985D;
}
</style>
