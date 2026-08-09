<script setup>
import { defineProps, computed } from 'vue'

const props = defineProps({
  value: {
    type: Number,
    required: true
  },
  max: {
    type: Number,
    required: true
  }
})

const percentage = computed(() => {
  if (!props.max || props.max === 0) return 0
  return Math.min((props.value / props.max) * 100, 100).toFixed(1)
})
</script>

<template>
  <div class="progress-bar">
    <div class="progress-fill" :style="{ width: percentage + '%' }"></div>
    <!-- ✅ percentage always visible -->
    <span class="progress-text">{{ percentage }}%</span>
  </div>
</template>

<style scoped>
@keyframes shimmer {
  0% { background-position: 200% 0; }
  100% { background-position: -200% 0; }
}

.progress-bar {
  position: relative;
  height: 12px;              /* slim bar */
  background-color: #e2e8f0;
  border-radius: 6px;
  overflow: hidden;
  box-shadow: inset 0 1px 2px rgba(0, 0, 0, 0.1);
  max-width: 80px;
}

.progress-fill {
  height: 100%;
  background-color: rgb(5, 150, 105);
  background-image: linear-gradient(
    90deg,
    rgba(255, 255, 255, 0) 0%,
    rgba(255, 255, 255, 0.4) 50%,
    rgba(255, 255, 255, 0) 100%
  );
  background-size: 200% 100%;
  animation: shimmer 3s infinite linear;
  transition: width 0.6s cubic-bezier(0.4, 0, 0.2, 1);
}

.progress-text {
  position: absolute;
  right: 4px;
  top: 0;                    /* ✅ ensure visible above fill */
  height: 100%;
  display: flex;
  align-items: center;
  color: #111827;            /* dark gray for contrast */
  font-size: 9px;
  font-weight: 600;
  z-index: 2;                /* ✅ layered above shimmer */
  pointer-events: none;
}
</style>
