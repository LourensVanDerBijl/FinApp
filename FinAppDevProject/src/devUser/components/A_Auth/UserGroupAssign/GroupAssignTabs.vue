<script setup>
defineProps({
  activeTab: { type: String, required: true } // 'create' | 'join'
})

const emit = defineEmits(['select'])
</script>

<template>
  <div class="tab-switcher">
    <button
      type="button"
      class="tab-btn"
      :class="{ active: activeTab === 'create' }"
      @click="emit('select', 'create')"
    >
      Create Group
    </button>
    <button
      type="button"
      class="tab-btn"
      :class="{ active: activeTab === 'join' }"
      @click="emit('select', 'join')"
    >
      Join Group
    </button>
  </div>
</template>

<style scoped>
.tab-switcher {
  display: none;
}

@media (max-width: 960px) {
  .tab-switcher {
    display: flex;
    max-width: 340px;
    margin: 0 auto 20px;
    background: rgba(37, 99, 235, 0.05);
    border: 1px solid #e2e8f0;
    border-radius: 10px;
    padding: 4px;
  }

  .tab-btn {
    flex: 1;
    padding: 9px 12px;
    border: none;
    background: none;
    color: #64748b;
    font-family: inherit;
    font-size: 0.76rem;
    font-weight: 700;
    letter-spacing: 0.03em;
    border-radius: 7px;
    cursor: pointer;
  }

  .tab-btn.active {
    background: rgba(37, 99, 235, 0.12);
    color: #1855b9;
  }
}
</style>
