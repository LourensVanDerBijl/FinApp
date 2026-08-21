<script setup>
import { currentUserProfile } from '../../data/userSession.js'
</script>

<template>
  <div class="dashboard-placeholder">
    <h1>User Group Dashboard</h1>
    <p>Welcome, {{ currentUserProfile?.displayName }}.</p>
    <p>Group: {{ currentUserProfile?.groupName }} ({{ currentUserProfile?.groupId }})</p>
    <p class="note">This is a placeholder — the real dashboard gets built later.</p>
  </div>
</template>

<style scoped>
.dashboard-placeholder {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  background: #0b1220;
  color: #fff;
  text-align: center;
  gap: 8px;
}

.note {
  color: #94a3b8;
  font-size: 0.85rem;
  margin-top: 16px;
}
</style>