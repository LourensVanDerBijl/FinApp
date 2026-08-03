<script setup>
import projectLogo from '../../../assets/projectLogo.png'
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-header">
      <h2>FinBine</h2>
      <p>Enterprise Administration</p>
    </div>

    <nav class="sidebar-nav">
      <RouterLink to="/admin/dashboard">Dashboard</RouterLink>
      <RouterLink to="/admin/groups">Groups</RouterLink>
      <RouterLink to="/admin/activity">Activity</RouterLink>
      <RouterLink to="/admin/api-control">API Control</RouterLink>
      <RouterLink to="/admin/tickets">Tickets</RouterLink>
      <hr />
      <RouterLink to="/admin/development">Development</RouterLink>
      <RouterLink to="/admin/settings">Settings</RouterLink>
    </nav>

    <footer class="sidebar-footer">
      <div class="admin-info">
        <img :src="projectLogo" alt="Admin Avatar" class="avatar" />
        <div>
          <p class="admin-name">Administrator Name</p>
          <p class="admin-role">Administrator Role</p>
        </div>
      </div>
      <button class="logout">Logout</button>
    </footer>
  </aside>
</template>

<style scoped>
.sidebar {
  width: 260px;
  background: #04111F;
  color: #fff;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}
.sidebar-header {
  padding: 24px;
}
.sidebar-nav {
  display: flex;
  flex-direction: column;
  gap: 12px;
  padding: 24px;
}
.sidebar-footer {
  padding: 24px;
  border-top: 1px solid #1f2937;
}
.admin-info {
  display: flex;
  align-items: center;
  gap: 12px;
}
.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
}
.logout {
  margin-top: 12px;
  background: #dc2626;
  color: #fff;
  border: none;
  padding: 8px 12px;
  border-radius: 6px;
}
</style>
