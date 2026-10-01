<script setup>
import { ref } from 'vue'
import DevelopmentTopNav from '../components/development/DevelopmentTopNav.vue'
import DbUsersView from '../components/development/DbUsersView.vue'
import TestDataView from '../components/development/TestDataView.vue'

const activeTab = ref('dbusers')

function handleChange(tab) {
  activeTab.value = tab
}
</script>

<template>
  <div class="development-page">
    <div class="page-title-row">
      <div>
        <h1>Development</h1>
        <p class="page-subtitle">A live, front-end view of the underlying databases — inspect, add, and delete records without switching tools.</p>
      </div>
    </div>

    <DevelopmentTopNav :active-tab="activeTab" @change="handleChange" />

    <DbUsersView v-if="activeTab === 'dbusers'" />
    <TestDataView v-else-if="activeTab === 'testdata'" />
  </div>
</template>

<style scoped>
.development-page {
  display: flex;
  flex-direction: column;
  flex: 1;
  min-height: 0;
  height: 100%;
  background: #F8FAFC;
}

.page-title-row {
  padding: 10px 16px 8px;
}

.page-title-row h1 {
  font-size: 1.1rem;
  font-weight: 700;
  color: #0F172A;
  margin: 0;
}

.page-subtitle {
  font-size: 0.68rem;
  color: #64748B;
  margin: 2px 0 0 0;
}

</style>
