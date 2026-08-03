<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  activities: Array
})
</script>

<template>
  <ul class="activity-list">
    <li v-for="(activity, index) in activities" :key="index">
      {{ activity }}
    </li>
  </ul>
</template>

<style scoped>
.activity-list {
  list-style: none;
  padding: 0;
}
.activity-list li {
  padding: 8px 0;
  border-bottom: 1px solid #e5e7eb;
  font-size: 16px;
}
</style>
