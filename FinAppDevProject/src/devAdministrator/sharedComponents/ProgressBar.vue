<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  value: Number,
  max: Number
})
</script>

<template>
  <div class="progress-bar">
    <div class="progress-fill" :style="{ width: (value/max*100) + '%' }"></div>
  </div>
</template>

<style scoped>
.progress-bar {
  background: #e5e7eb;
  border-radius: 8px;
  height: 12px;
  width: 100%;
}
.progress-fill {
  background: #0ea5a4;
  height: 100%;
  border-radius: 8px;
}
</style>
