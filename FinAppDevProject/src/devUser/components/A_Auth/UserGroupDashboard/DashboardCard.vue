<script setup>
// Shared card shell — tightened for a denser, enterprise-console feel:
// smaller header, tighter body padding, less air between elements.
defineProps({
  title: { type: String, default: '' }
})
</script>

<template>
  <div class="dash-card">
    <div v-if="title" class="dash-card-header">
      <h3>{{ title }}</h3>
      <slot name="header-extra" />
    </div>
    <div class="dash-card-body">
      <slot />
    </div>
  </div>
</template>

<style scoped>
.dash-card {
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  display: flex;
  flex-direction: column;
  overflow: hidden;
  min-height: 0;
}

.dash-card-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 9px 12px;
  border-bottom: 1px solid #e2e8f0;
  background: #f8fafc;
  flex-shrink: 0;
}

.dash-card-header h3 {
  margin: 0;
  font-size: 0.74rem;
  font-weight: 700;
  color: #0f172a;
  letter-spacing: 0.01em;
}

.dash-card-body {
  padding: 10px 12px;
  flex: 1;
  min-height: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}
</style>
