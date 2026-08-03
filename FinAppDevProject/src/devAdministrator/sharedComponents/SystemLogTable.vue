<script setup>
import { defineProps } from 'vue'

const props = defineProps({
  logs: Array
})
</script>

<template>
  <table class="log-table">
    <thead>
      <tr>
        <th>Time</th>
        <th>Type</th>
        <th>Source</th>
        <th>Message</th>
        <th>Level</th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(log, index) in logs" :key="index">
        <td>{{ log.time }}</td>
        <td>{{ log.type }}</td>
        <td>{{ log.source }}</td>
        <td>{{ log.message }}</td>
        <td><span class="badge">{{ log.level }}</span></td>
      </tr>
    </tbody>
  </table>
</template>

<style scoped>
.log-table {
  width: 100%;
  border-collapse: collapse;
}
.log-table th, .log-table td {
  padding: 8px;
  text-align: left;
}
.badge {
  padding: 4px 8px;
  border-radius: 6px;
  font-size: 12px;
}
</style>
