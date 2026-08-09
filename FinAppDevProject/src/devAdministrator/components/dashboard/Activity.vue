<script setup>
import { defineProps } from 'vue'
import SectionCard from '../../sharedComponents/SectionCard.vue'
import ActivityList from '../../sharedComponents/ActivityList.vue'

const props = defineProps({
  activities: Array
})
</script>

<template>
  <SectionCard class="activity-card">
    <h3 class="section-title">Recent Activity</h3>
    <div class="activity-scroll">
      <ActivityList :activities="activities" />
    </div>
  </SectionCard>
</template>

<style scoped>
.section-title {
  font-size: 13px;
  font-weight: 600;
  color: #eee9e9;
  margin: 0;
  padding-left: 4px;
  border-radius: 4px;
  text-align: left;
  background: rgba(6, 30, 57, 0.934);
}

.activity-card {
  display: flex;
  flex-direction: column;
  height: 100%;
}

.activity-scroll {
  flex: 1;
  overflow-y: auto; /* scroll only if content exceeds */
}
</style>
