<template>
  <div class="section-card">
    <slot />
  </div>
</template>

<style scoped>
.section-card {
  background: #fdfcff18;
  border-radius: 8px;
  padding: 1px;
  box-shadow: 0 1px 2px rgba(0, 0, 0, 0.057);
  margin-top: 0;
  height: 100%;          /* ✅ fill parent row height */
  display: flex;
  flex-direction: column; /* ✅ ensures scroll containers expand correctly */
  overflow: hidden;       /* ✅ prevents card from overflowing its parent */
}
</style>
