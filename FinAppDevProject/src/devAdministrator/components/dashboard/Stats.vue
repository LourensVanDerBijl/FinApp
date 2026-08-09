<script setup>
import StatisticCard from '../../sharedComponents/StatisticCard.vue'

// Accept stats as a prop (coming from Dashboard.vue)
const props = defineProps({
  stats: {
    type: Array,
    required: true
  }
})
</script>

<template>
  <div class="stats-grid">
    <StatisticCard
      v-for="(stat, index) in stats"
      :key="index"
      :icon="stat.icon"
      :title="stat.title"
      :value="stat.value"
      :link="stat.link"
    />
  </div>
</template>

<style scoped>
.stats-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 8px; /* tighter spacing */
  margin-top: 0; /* flush to the top */
}

/* Responsive adjustments */
@media (max-width: 1200px) {
  .stats-grid { grid-template-columns: repeat(3, 1fr); }
}
@media (max-width: 768px) {
  .stats-grid { grid-template-columns: repeat(2, 1fr); }
}
@media (max-width: 480px) {
  .stats-grid { grid-template-columns: 1fr; }
}
</style>
