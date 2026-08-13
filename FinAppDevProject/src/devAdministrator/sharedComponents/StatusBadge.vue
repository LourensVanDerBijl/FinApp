<script setup>
import { defineProps, computed } from 'vue'

const props = defineProps({
  status: String
})

// ✅ Compute badge class based on status
const badgeClass = computed(() => {
  switch (props.status) {
    case 'Online':
      return 'online'
    case 'Warning':
      return 'warning'
    case 'Critical':
      return 'critical'
    case 'Offline':
      return 'offline'
    default:
      return 'neutral'
  }
})
</script>

<template>
  <span class="status-badge" :class="badgeClass">
    <span class="status-dot"></span>
    {{ status }}
  </span>
</template>

<style scoped>
.status-badge {
  display: inline-flex;
  align-items: center;
  gap: 3px;
  padding: 1px 6px;
  border-radius: 6px;
  background-color: #FFFFFF;
  border: 1px solid #E5E7EB;
  font-weight: 600;
  font-size: 9px;
  color: #374151;
  min-width: 60px;
  height: 12px;
}

/* ✅ Status dot */
.status-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  flex-shrink: 0;
}

.status-badge.online .status-dot {
  background-color: #10B981; /* green */
}

.status-badge.warning .status-dot {
  background-color: #e6950a; /* amber */
}

.status-badge.critical .status-dot {
  background-color: #e6950a; /* red */
}

.status-badge.offline .status-dot {
  background-color: #DC2626; /* darker red for offline */
}

.status-badge.neutral .status-dot {
  background-color: #6B7280; /* gray fallback */
}
</style>
