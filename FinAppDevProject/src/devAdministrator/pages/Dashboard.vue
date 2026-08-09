<script setup>
import Stats from '../components/dashboard/Stats.vue'
import Subscription from '../components/dashboard/Subscription.vue'
import Activity from '../components/dashboard/Activity.vue'
import PlatformHealth from '../components/dashboard/PlatformHealth.vue'
import SystemResources from '../components/dashboard/SystemResources.vue'
import Logs from '../components/dashboard/Logs.vue'

import { stats, subscriptionData, activities, platformHealth, systemResources, logs } from '../data/mockData.js'
</script>

<template>
  <div class="dashboard">
    <Stats :stats="stats" />

    <!-- Top row: Subscription, Activity, Platform Health -->
    <div class="dashboard-row dashboard-row-top">
      <Subscription :data="subscriptionData" />
      <Activity :activities="activities" />
      <PlatformHealth :services="platformHealth" />
    </div>

    <!-- Second row: System Resources + Logs side by side -->
    <div class="dashboard-row dashboard-row-resources">
      <SystemResources :resources="systemResources" />
      <Logs :logs="logs" />
    </div>
  </div>
</template>

<style scoped>
.dashboard {
  background: #F8FAFC;
  padding: 0 12px 12px 12px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  height: 100vh;          /* fill viewport */
  overflow: hidden;       /* ✅ prevent page scroll */
}

.dashboard-row {
  display: flex;
  gap: 16px;
}

/* Top row: all three equal width, fixed height */
.dashboard-row-top {
  display: flex;
  flex-direction: row;
  align-items: stretch;
  height: 220px;
}

.dashboard-row-top > * {
  flex: 1;
  display: flex;
  flex-direction: column;
  height: 100%;
}

/* Second row: SystemResources ~30%, Logs ~70% */
.dashboard-row-resources {
  display: grid;
  grid-template-columns: 30% 70%;
  gap: 16px;
  align-items: stretch;
  max-height: calc(100vh - 260px); /* ✅ constrain row height */
  overflow: hidden;                /* ✅ prevent row overflow */
}
</style>
