<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  activities: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <ul class="activity-list">
    <li v-for="(activity, index) in activities" :key="index">
      <p class="activity-text">
        <strong>{{ activity.user }}</strong> {{ activity.action }}
        <span v-if="activity.group"> <strong>{{ activity.group }}</strong></span>
      </p>
      <p class="activity-time">{{ activity.time }}</p>
    </li>
  </ul>
</template>

<style scoped>
.activity-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.activity-list li {
  padding: 2px 0;
  border-bottom: 1px solid #e5e7eb;
  transition: background 0.2s ease;
}

.activity-list li:hover {
  background: #f9fafb; /* subtle hover highlight */
}

.activity-text {
  font-size: 11px; /* compact text */
  line-height: 1.3;
}

.activity-time {
  font-size: 9px;
  color: #6b7280;
}
</style>
