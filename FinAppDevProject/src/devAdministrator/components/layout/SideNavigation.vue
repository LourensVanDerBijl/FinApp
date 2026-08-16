<script setup>
import logo from '../../../assets/SVG/logo.svg'
import { LayoutDashboard, Users, Activity, ServerCog, Ticket, Code, Settings, LogOut, UserCircle } from 'lucide-vue-next'
import { RouterLink, useRouter } from 'vue-router'
import { auth } from '../../../firebase/firebaseManager.js'
import { signOut } from 'firebase/auth'
import { adminInfo } from '../../data/mockData.js'   // ✅ import admin info

const router = useRouter()

async function handleLogout() {
  try {
    await signOut(auth) // ✅ Firebase logout
    router.push('/admin/login')
  } catch (error) {
    console.error("Logout error:", error)
    alert("Failed to logout. Please try again.")
  }
}
</script>

<template>
  <aside class="sidebar">
    <!-- Header with logo -->
    <div class="sidebar-header">
      <img :src="logo" alt="FinBine Logo" class="logo" />
      <div class="brand">
        <h2>FinBine</h2>
        <p>Enterprise Administration</p>
      </div>
    </div>

    <!-- Navigation -->
    <nav class="sidebar-nav">
      <RouterLink to="/admin/dashboard"><LayoutDashboard size="14" /> Dashboard</RouterLink>
      <RouterLink to="/admin/groups"><Users size="14" /> Groups</RouterLink>
      <RouterLink to="/admin/activity"><Activity size="14" /> Activity</RouterLink>
      <RouterLink to="/admin/api-control"><ServerCog size="14" /> API Control</RouterLink>
      <RouterLink to="/admin/tickets"><Ticket size="14" /> Tickets</RouterLink>
      <hr />
      <RouterLink to="/admin/development"><Code size="14" /> Development</RouterLink>
      <RouterLink to="/admin/settings"><Settings size="14" /> Settings</RouterLink>
    </nav>

    <!-- Footer -->
    <footer class="sidebar-footer">
      <div class="admin-info">
        <!-- Red profile icon instead of image -->
        <UserCircle size="28" class="profile-icon" />
        <div>
          <!-- ✅ dynamic values from mockData -->
          <p class="admin-name">{{ adminInfo.name }}</p>
          <p class="admin-role">{{ adminInfo.role }}</p>
        </div>
      </div>
      <button class="logout" @click="handleLogout">
        <LogOut size="14" /> Logout
      </button>
    </footer>
  </aside>
</template>

<style scoped>
/* Sidebar container */
.sidebar {
  background-color: #04111f;
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 240px;
  border-right: 1px solid rgba(255, 255, 255, 0.1);
  color: #fff;
}

/* Branding section */
.sidebar-header {
  display: flex;
  align-items: flex-start;
  justify-content: flex-start;
  padding: 14px;
  gap: 8px;
}

.logo {
  height: 36px;
  width: auto;
  filter: brightness(0) saturate(100%) invert(29%) sepia(98%) saturate(748%) hue-rotate(180deg) brightness(95%) contrast(90%);
}

.brand {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
}

.brand h2 {
  font-size: 1rem;
  font-weight: 600;
  color: #ffffff;
  margin: 0;
  line-height: 1.1;
  text-align: left;
}

.brand p {
  font-size: 0.55rem;
  color: #02b0a1;
  margin-top: 0;
  font-weight: 500;
  line-height: 1;
  text-align: left;
}

/* Navigation links */
.sidebar-nav {
  padding: 0 10px;
  flex: 1;
}

.sidebar-nav a {
  display: flex;
  align-items: center;
  gap: 8px;
  padding: 6px 12px;
  text-decoration: none;
  color: #f8fafc;
  border-radius: 6px;
  margin-bottom: 2px;
  transition: all 0.2s;
  font-size: 0.7rem;
}

.sidebar-nav a.router-link-active {
  background-color: #052b7f49;
  color: #ffffff;
}

.sidebar-nav hr {
  border: none;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  margin: 10px 0;
}

/* Footer */
.sidebar-footer {
  padding: 0;
}

.admin-info {
  padding: 14px;
  display: flex;
  align-items: center;
  gap: 8px;
  border-top: 1px solid rgba(255, 255, 255, 0.1);
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.profile-icon {
  color: #dc2626; /* red icon */
}

.admin-name {
  font-size: 0.7rem;
  font-weight: 500;
  color: #ffffff;
  margin: 0;
  line-height: 1;
}

.admin-role {
  font-size: 0.6rem;
  color: #94a3b8;
  margin: 0;
  line-height: 1;
}

.logout {
  width: 100%;
  padding: 10px 14px;
  background: none;
  border: none;
  color: #f8fafc;
  display: flex;
  align-items: center;
  gap: 8px;
  cursor: pointer;
  font-size: 0.7rem;
}
</style>
