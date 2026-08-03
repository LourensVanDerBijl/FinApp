<template>
  <div class="app-layout">
    <SideNavigation />
    <div class="main-content">
      <TopNavigation />
      <router-view />
    </div>
  </div>
</template>

<script setup>
import SideNavigation from './SideNavigation.vue'
import TopNavigation from './TopNavigation.vue'
</script>

<style scoped>
.app-layout {
  display: flex;
  height: 100vh;
  background: #F8FAFC;
}
.main-content {
  flex: 1;
  display: flex;
  flex-direction: column;
}
</style>
