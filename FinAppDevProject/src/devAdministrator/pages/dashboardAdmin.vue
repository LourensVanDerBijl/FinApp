<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { auth } from '../../firebase/firebaseManager.js'
import { startIdleTimer } from '../idleLogout.js'   // ✅ helper for auto-logout

const router = useRouter()

// Display admin email if available
const adminName = ref(auth.currentUser?.email || "Admin User")

// Manual logout
function handleLogout() {
  auth.signOut()
  router.push('/admin/login')
}

// Start idle timer when dashboard mounts
onMounted(() => {
  startIdleTimer(router)
})
</script>

<template>
  <div class="dashboard">
    <header class="dashboard-header">
      <h1>FinBine Admin Dashboard</h1>
      <button @click="handleLogout" class="logout-button">Logout</button>
    </header>

    <main class="dashboard-content">
      <h2>Welcome, {{ adminName }}</h2>

      <div class="stats-grid">
        <div class="stat-card">
          <h3>Total Users</h3>
          <p>42</p>
        </div>
        <div class="stat-card">
          <h3>Transactions</h3>
          <p>128</p>
        </div>
        <div class="stat-card">
          <h3>Revenue</h3>
          <p>$12,340</p>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.dashboard {
  padding: 2rem;
  font-family: Arial, sans-serif;
}
.dashboard-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.logout-button {
  background: #c0392b;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  cursor: pointer;
}
.stats-grid {
  display: flex;
  gap: 1rem;
  margin-top: 2rem;
}
.stat-card {
  flex: 1;
  background: #f4f4f4;
  padding: 1rem;
  border-radius: 6px;
  text-align: center;
}
</style>
