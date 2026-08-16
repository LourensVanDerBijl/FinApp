<script setup>
import { defineProps, computed, ref, onMounted, onUnmounted } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import StatusBadge from '../../sharedComponents/StatusBadge.vue'
import { platformHealthLastChecked, refreshPlatformHealth } from '../../data/mockData.js'

import { Database, Server, Cloud, Cpu, RefreshCw } from 'lucide-vue-next'

const props = defineProps({
  services: Array
})

// A "clock" that ticks every second, purely so lastChecked below has a
// reason to recalculate that often — it doesn't drive anything else.
const now = ref(new Date())
let clockInterval = null

onMounted(() => {
  clockInterval = setInterval(() => {
    now.value = new Date()
  }, 1000)
})

onUnmounted(() => {
  clearInterval(clockInterval)
})

// Turns the real last-checked Date into a friendly "12s ago" style label.
const lastChecked = computed(() => {
  if (!platformHealthLastChecked.value) return 'never'

  const seconds = Math.floor((now.value - platformHealthLastChecked.value) / 1000)

  if (seconds < 60) return `${seconds}s ago`
  const minutes = Math.floor(seconds / 60)
  return `${minutes}m ago`
})

function getIcon(name) {
  switch (name) {
    case 'Firebase':
      return Cloud
    case 'PostgreSQL':
      return Database
    case 'C#':
      return Server
    case 'Netlify':
      return Cpu
    default:
      return Server
  }
}

// Maps the backend's status text to the existing CSS class names,
// without needing to rename anything in the stylesheet.
function getStatusClass(status) {
  switch (status) {
    case 'Online':
      return 'healthy'
    case 'Critical':
      return 'critical'
    case 'Offline':
      return 'offline'
    default:
      return ''
  }
}
</script>

<template>
  <SectionCard class="platform-card">
    <div class="section-header">
      <h3 class="section-title">Platform Health</h3>
      <div class="last-check">
        <span>Last checked: {{ lastChecked }}</span>
        <RefreshCw size="10" class="refresh-icon" @click="refreshPlatformHealth" />
      </div>
    </div>

    <div class="platform-scroll">
      <ul>
        <!-- ✅ Header row -->
        <li class="service-header">
          <div class="service-name">Service</div>
          <div class="service-status">Status</div>
          <div class="service-latency">Latency</div>
          <div class="service-errors">Uptime</div>
        </li>

        <!-- ✅ Data rows -->
        <li
          v-for="(service, index) in services"
          :key="index"
          class="service-row"
          :class="getStatusClass(service.status)"
        >
          <div class="service-name">
            <component
              :is="getIcon(service.name)"
              size="10"
              class="service-icon"
            />
            {{ service.name }}
          </div>
          <div class="service-status">
            <StatusBadge :status="service.status" />
          </div>
          <div class="service-latency">
            {{ service.responseTime !== null ? service.responseTime + ' ms' : '--' }}
          </div>
          <div class="service-errors">
            {{ service.uptime }} min
          </div>
        </li>
      </ul>
    </div>
  </SectionCard>
</template>

<style scoped>
/* unchanged — exactly as you had it */
.platform-card {
  background-color: #FFFFFF;
  border: 1px solid #E5E7EB;
  border-radius: 6px;
  overflow: hidden;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  padding: 0;
  display: flex;
  flex-direction: column;
  height: 100%;
}

.section-header {
  background-color: #112135;
  height: 26px;
  padding: 0 10px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 6px 6px 0 0;
  box-sizing: border-box;
}

.section-title {
  color: #ffffff;
  font-size: 11px;
  font-weight: 600;
  line-height: 1;
  margin: 0;
}

.last-check {
  color: rgba(255, 255, 255, 0.8);
  font-size: 9px;
  font-weight: 400;
  line-height: 1;
  margin: 0;
  display: flex;
  align-items: center;
  gap: 3px;
}

.refresh-icon {
  cursor: pointer;
  transition: transform 0.2s ease;
}

.refresh-icon:hover {
  transform: rotate(90deg);
}

.platform-scroll {
  flex: 1;
  overflow-y: auto;
}

ul {
  font-size: 9px;
  margin: 0;
  padding: 0;
}

.service-header {
  display: grid;
  grid-template-columns: 2fr 1.5fr 1fr 1fr;
  font-weight: 600;
  color: #314155;
  padding: 3px 10px;
  border-bottom: 1px solid #E5E7EB;
  font-size: 9px;
}

.service-row {
  display: grid;
  grid-template-columns: 2fr 1.5fr 1fr 1fr;
  align-items: center;
  padding: 3px 10px;
  border-bottom: 1px solid rgba(0,0,0,0.05);
  color: #1F2937;
  font-size: 9px;
}

.service-row:last-child {
  border-bottom: none;
}

.service-row.healthy {
  background-color: #02a13c20;
}

.service-row.warning {
  background-color: #d5960315;
}

.service-row.critical {
  background-color: #b57f031c;
}

.service-row.offline {
  background-color: #c2030325;
}

.service-name {
  display: flex;
  align-items: center;
  gap: 3px;
  font-weight: 500;
  color: #1E293B;
}

.service-icon {
  color: #475569;
}

.service-status {
  display: flex;
  justify-content: flex-start;
}

.service-latency {
  font-family: monospace;
  font-size: 9px;
  color: #4B5563;
}

.service-errors {
  font-weight: 600;
  font-size: 9px;
  color: #6B7280;
  text-align: left;
}

.service-errors.has-errors {
  color: #c41616;
}
</style>