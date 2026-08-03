<script setup>
import Stats from '../components/dashboard/Stats.vue'
import Subscription from '../components/dashboard/Subscription.vue'
import Activity from '../components/dashboard/Activity.vue'
import PlatformHealth from '../components/dashboard/PlatformHealth.vue'
import SystemOverview from '../components/dashboard/SystemOverview.vue'
import Logs from '../components/dashboard/Logs.vue'

import { stats, subscriptionData, activities, platformHealth, systemOverview, logs } from '../data/mockData.js'
</script>

<template>
  <div class="dashboard">
    <!-- Section 1: Statistics -->
    <DashboardStats :stats="stats" />

    <!-- Section 2: Platform Overview -->
    <div class="dashboard-row">
      <DashboardSubscription :data="subscriptionData" />
      <DashboardActivity :activities="activities" />
    </div>

    <!-- Section 3: Platform Health -->
    <div class="dashboard-row">
      <DashboardPlatformHealth :services="platformHealth" />
      <DashboardSystemOverview :systems="systemOverview" />
    </div>

    <!-- Section 4: Logs -->
    <DashboardLogs :logs="logs" />
  </div>
</template>

<style scoped>
.dashboard {
  background: #F8FAFC;
  padding: 24px;
  display: flex;
  flex-direction: column;
  gap: 24px;
}
.dashboard-row {
  display: flex;
  gap: 24px;
}
</style>
