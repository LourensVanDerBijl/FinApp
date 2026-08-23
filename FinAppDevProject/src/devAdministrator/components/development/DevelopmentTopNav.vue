<script setup>
import { Database, Lock } from 'lucide-vue-next'

defineProps({
  activeTab: { type: String, required: true }
})

const emit = defineEmits(['change'])

const tabs = [
  { key: 'dbusers', label: 'DbUsers', icon: Database, disabled: false },
  { key: 'dbsomething', label: 'DbSomething', icon: Lock, disabled: true }
]
</script>

<template>
  <div class="dev-top-nav">
    <button
      v-for="tab in tabs"
      :key="tab.key"
      class="dev-tab"
      :class="{ active: activeTab === tab.key, disabled: tab.disabled }"
      :disabled="tab.disabled"
      :title="tab.disabled ? 'Coming soon' : ''"
      @click="!tab.disabled && emit('change', tab.key)"
    >
      <component :is="tab.icon" size="13" />
      {{ tab.label }}
      <span v-if="tab.disabled" class="soon-badge">Soon</span>
    </button>
  </div>
</template>

<style scoped>
.dev-top-nav {
  display: flex;
  gap: 4px;
  border-bottom: 1px solid #E2E8F0;
  padding: 0 16px;
  background: #F8FAFC;
  flex-shrink: 0;
}

.dev-tab {
  display: flex;
  align-items: center;
  gap: 6px;
  padding: 9px 12px;
  font-size: 0.72rem;
  font-weight: 600;
  color: #64748B;
  background: none;
  border: none;
  border-bottom: 2px solid transparent;
  cursor: pointer;
  margin-bottom: -1px;
}

.dev-tab:hover:not(.disabled) {
  color: #0F172A;
}

.dev-tab.active {
  color: #2563EB;
  border-bottom-color: #2563EB;
}

.dev-tab.disabled {
  color: #CBD5E1;
  cursor: not-allowed;
}

.soon-badge {
  font-size: 0.58rem;
  font-weight: 700;
  text-transform: uppercase;
  background: #F1F5F9;
  color: #94A3B8;
  padding: 1px 5px;
  border-radius: 4px;
}
</style>
