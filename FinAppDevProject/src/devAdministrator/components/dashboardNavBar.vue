<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { auth } from '../../firebase/firebaseManager.js'

const props = defineProps({
  activeTab: String
})
const emit = defineEmits(['update:activeTab'])

const router = useRouter()

// Dummy admin data (later from DB)
const adminInfo = ref({
  name: "Louis",
  surname: "Van Der Bijl",
  accountId: "ADM-001",
  role: "Super Admin",
  email: auth.currentUser?.email || "admin@finbine.com"
})

function handleLogout() {
  auth.signOut()
  router.push('/admin/login')
}
</script>

<template>
  <nav class="navbar">
    <!-- Left: Brand -->
    <div class="navbar-left">
      <div class="brand-block">
        <span class="brand-name">FinBine</span>
        <span class="brand-subtext">Administrator Portal</span>
      </div>
    </div>

    <!-- Middle: Navigation Links -->
    <ul class="nav-links">
      <li><button :class="{ active: activeTab==='overview' }" @click="emit('update:activeTab','overview')">Overview</button></li>
      <li><button :class="{ active: activeTab==='memberships' }" @click="emit('update:activeTab','memberships')">Memberships</button></li>
      <li><button :class="{ active: activeTab==='requests' }" @click="emit('update:activeTab','requests')">Requests</button></li>
      <li><button :class="{ active: activeTab==='monitoring' }" @click="emit('update:activeTab','monitoring')">Monitoring</button></li>
    </ul>

    <!-- Right: Admin Info + Logout side by side -->
    <div class="navbar-right">
      <div class="admin-info">
        <span class="admin-name">{{ adminInfo.name }} {{ adminInfo.surname }}</span>
        <span class="admin-details">{{ adminInfo.accountId }} · {{ adminInfo.role }}</span>
      </div>
      <button @click="handleLogout" class="logout-button">Logout</button>
    </div>
  </nav>
</template>

<style scoped>
/* Navbar Container */
.navbar {
  background-color: #1a2a40; /* FinBine Blue */
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem 1rem;
  min-height: 80px;
  border-bottom: 1px solid rgba(59, 130, 246, 0.2);
  box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.2);
  font-family: 'Inter', system-ui, sans-serif;
}

/* Branding */
.brand-block {
  display: flex;
  flex-direction: column;
  line-height: 1.2;
}
.brand-name {
  font-weight: 800;
  font-size: 1.75rem;
  color: #ffffff;
  letter-spacing: -0.05em;
}
.brand-subtext {
  color: #94a3b8;
  font-size: 8px;
  font-weight: 500;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  opacity: 0.7;
}

/* Nav Links */
.nav-links {
  display: flex;
  gap: 0.75rem;
  align-items: center;
  margin: 0;
  padding: 0;
  list-style: none;
}
.nav-links li {
  margin: 0;
  padding: 0;
}
.nav-links button {
  background: none;
  border: none;
  color: #94a3b8;
  font-size: 0.9rem;
  font-weight: 600;
  padding: 0.4rem 0.8rem;
  border-radius: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
}
.nav-links button:hover {
  color: #ffffff;
  background-color: rgba(255, 255, 255, 0.05);
}
.nav-links button.active {
  color: #60a5fa;
  background-color: rgba(59, 130, 246, 0.1);
}

/* Admin Info + Logout side by side */
.navbar-right {
  display: flex;
  align-items: center;
  gap: 1rem;
}
.admin-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  line-height: 1.2;
}
.admin-name {
  font-weight: 600;
  font-size: 0.875rem;
  color: #ffffff;
}
.admin-details {
  font-size: 0.7rem;
  color: #94a3b8;
}

/* Logout Button */
.logout-button {
  background-color: #ef4444;
  color: white;
  border: none;
  padding: 0.5rem 1rem;
  border-radius: 0.375rem;
  font-weight: 600;
  font-size: 0.875rem;
  cursor: pointer;
  transition: opacity 0.2s;
}
.logout-button:hover {
  opacity: 0.9;
}
</style>
