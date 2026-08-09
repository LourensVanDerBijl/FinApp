<script setup>
import { defineProps } from 'vue'
import { Users, User, MessageSquare, Clock, CheckCircle } from 'lucide-vue-next'

const props = defineProps({
  icon: String,
  title: String,
  value: [String, Number],
  link: String
})

const iconMap = {
  groups: Users,
  users: User,
  tickets: MessageSquare,
  pending: Clock,
  closed: CheckCircle
}
</script>

<template>
  <div class="stat-card">
    <div class="stat-content">
      <!-- Left side: compact icon -->
      <component
        :is="iconMap[icon]"
        size="28"
        :class="['stat-icon', icon]"
      />

      <!-- Right side: title + value stacked tightly -->
      <div class="stat-info">
        <h3 class="stat-title">{{ title }}</h3>
        <p class="stat-value">{{ value }}</p>
        <a v-if="link" :href="link" class="stat-link">View more</a>
      </div>
    </div>
  </div>
</template>

<style scoped>
.stat-card {
  background: #fff;
  border-radius: 8px;
  padding: 8px;
  box-shadow: 0 1px 2px rgba(0,0,0,0.05);
}

.stat-content {
  display: flex;
  align-items: center;
  justify-content: flex-start;
  gap: 8px;
}

.stat-icon.groups { color: #065f46; }
.stat-icon.users { color: #2563eb; }
.stat-icon.tickets { color: #ea580c; }
.stat-icon.pending { color: #dc2626; }
.stat-icon.closed { color: #22c55e; }

.stat-info {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  justify-content: center;
}

.stat-title {
  font-size: 11px;   /* smaller title */
  font-weight: 500;
  margin: 0;
  line-height: 1.1;
  color: #111827;
}

.stat-value {
  font-size: 16px;   /* smaller value */
  font-weight: bold;
  margin: 0;
  line-height: 1;
  color: #111827;
}

.stat-link {
  font-size: 10px;
  color: #2563eb;
  margin-top: 2px;
}
</style>
