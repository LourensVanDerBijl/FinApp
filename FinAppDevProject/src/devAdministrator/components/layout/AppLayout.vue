<script setup>
import SideNavigation from './SideNavigation.vue'
</script>

<template>
  <div class="app-layout">
    <!-- Sidebar -->
    <SideNavigation />

    <!-- Main content area -->
    <main class="main-content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-layout {
  display: flex;
  height: 100vh; /* Full height layout */
  width: 100%;   /* Ensure it spans full width */
  overflow: hidden; /* Prevent scrollbars from layout container */
}

.main-content {
  flex-grow: 1;
  background: #f3f4f6;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
}
</style>
