<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  status: String
})
</script>

<template>
  <span class="status-badge">{{ status }}</span>
</template>

<style scoped>
.status-badge {
  background: #22c55e;
  color: #fff;
  padding: 4px 8px;
  border-radius: 8px;
  font-size: 12px;
}
</style>
