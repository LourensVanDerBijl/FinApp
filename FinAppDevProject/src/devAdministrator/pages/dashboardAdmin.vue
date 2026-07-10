<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { auth } from '../../firebase/firebaseManager.js'
import { startIdleTimer } from '../idleLogout.js'

// Import the NavBar and tab components
import DashboardNavBar from '../components/dashboardNavBar.vue'
import DashboardOverview from '../components/dashboardOverview.vue'
import DashboardMembership from '../components/dashboardMembership.vue'
import DashboardRequest from '../components/dashboardRequest.vue'
import DashboardMonitoring from '../components/dashboardMonitoring.vue'

const router = useRouter()
const adminName = ref(auth.currentUser?.email || "Admin User")

// Track which tab is active (default: overview)
const activeTab = ref("overview")

function handleLogout() {
  auth.signOut()
  router.push('/admin/login')
}

onMounted(() => {
  startIdleTimer(router)
})
</script>

<template>
  <div class="dashboard">
    <!-- Navbar always on top, passes activeTab state -->
    <DashboardNavBar v-model:activeTab="activeTab" />

    <main class="dashboard-content">
      <!-- Conditional rendering of tab components -->
      <DashboardOverview v-if="activeTab === 'overview'" />
      <DashboardMembership v-else-if="activeTab === 'memberships'" />
      <DashboardRequest v-else-if="activeTab === 'requests'" />
      <DashboardMonitoring v-else-if="activeTab === 'monitoring'" />
    </main>
  </div>
</template>

<style scoped>
.dashboard {
  min-height: 100vh;
  width: 100%;
  font-family: Arial, sans-serif;
  background-color: white;
  display: flex;
  flex-direction: column;
}
.dashboard-content {
  height: calc(100vh - 80px); /* full screen minus navbar height */
  margin: 0;
  padding: 0;
  overflow: hidden;           /* prevent scrollbars */
  display: flex;
  flex-direction: column;
}
</style>
