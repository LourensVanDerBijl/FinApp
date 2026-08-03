<script setup>
import { defineProps } from 'vue'
import { LayoutDashboard } from 'lucide-vue-next'

const props = defineProps({
  icon: String,
  title: String,
  value: [String, Number],
  link: String
})
</script>

<template>
  <div class="stat-card">
    <div class="stat-icon">
      <component :is="icon" size="24" />
    </div>
    <div class="stat-info">
      <h3>{{ title }}</h3>
      <p class="stat-value">{{ value }}</p>
      <a v-if="link" :href="link" class="stat-link">View more</a>
    </div>
  </div>
</template>

<style scoped>
.stat-card {
  background: #fff;
  border-radius: 16px;
  padding: 24px;
  box-shadow: 0 2px 6px rgba(0,0,0,0.05);
}
.stat-icon {
  margin-bottom: 8px;
}
.stat-info h3 {
  font-size: 18px;
  margin: 0;
}
.stat-value {
  font-size: 36px;
  font-weight: bold;
}
.stat-link {
  font-size: 14px;
  color: #2563eb;
}
</style>
