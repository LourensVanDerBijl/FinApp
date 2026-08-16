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
      <tr 
        v-for="(log, index) in logs" 
        :key="index"
        :class="['log-row', log.level.toLowerCase()]"
      >
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
  font-size: 11px;
}

.log-table thead {
  position: sticky;
  top: 0;
  z-index: 1;
  background-color: rgba(255, 255, 255, 0.93);
  color: #0e0e0e;
}

.log-table th {
  line-height: 1.2;
  padding: 4px 6px;
  font-size: 11px;
  font-weight: 600;
  text-align: left;
}

.log-table td {
  padding: 4px 6px;
  text-align: left;
  font-size: 9px;
  line-height: 1.2;
}

/* ✅ Row colors + text colors */
.log-row.error {
  background-color: #fee2e2; /* light red background */
  color: #b91c1c;           /* red text */
}
.log-row.warning {
  background-color: #fef3c7; /* light yellow/orange background */
  color: #b45309;            /* orange text */
}
.log-row.success {
  background-color: #dcfce7; /* light green background */
  color: #15803d;            /* green text */
}
.log-row.info {
  background-color: #dbeafe; /* light blue background */
  color: #1d4ed8;            /* blue text */
}

.log-row.critical {
  background-color: #fecaca; /* stronger red background, distinct from error */
  color: #7f1d1d;            /* dark red text */
  font-weight: 700;
}

/* ✅ Badge inherits row text color */
.badge {
  padding: 2px 6px;
  border-radius: 4px;
  font-size: 10px;
  font-weight: 800;
  color: inherit; /* ✅ matches row text color */
}
</style>
