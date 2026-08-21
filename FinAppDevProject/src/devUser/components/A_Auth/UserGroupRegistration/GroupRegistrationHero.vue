<script setup>
import { ArrowLeft } from 'lucide-vue-next'

defineProps({
  accountType: { type: String, required: true } // 'premium' | 'free'
})

defineEmits(['back'])
</script>

<template>
  <div class="hero-copy">
    <button type="button" class="back-link" @click="$emit('back')">
      <ArrowLeft :size="13" />
      Change account type
    </button>

    <p class="eyebrow">LAST STEP</p>
    <h1>Set Up Your <span class="highlight">Group</span></h1>
    <p class="subtitle">
      Give your group a name to finish creating your
      <strong>{{ accountType === 'premium' ? 'Premium' : 'Free' }}</strong> group.
    </p>
  </div>
</template>

<style scoped>
.hero-copy {
  text-align: center;
  flex-shrink: 0;
  margin-bottom: 14px;
  position: relative;
}

.back-link {
  position: absolute;
  left: 0;
  top: 2px;
  display: flex;
  align-items: center;
  gap: 5px;
  border: none;
  background: none;
  color: #64748b;
  font-family: inherit;
  font-size: 0.72rem;
  font-weight: 600;
  cursor: pointer;
  padding: 4px 2px;
}

.back-link:hover {
  color: #1855b9;
}

.eyebrow {
  font-size: 0.62rem;
  font-weight: 700;
  letter-spacing: 0.12em;
  color: #1855b9;
  margin: 0 0 6px;
}

.hero-copy h1 {
  font-size: 1.4rem;
  font-weight: 800;
  color: #0f172a;
  margin: 0 0 6px;
}

.highlight {
  color: #1855b9;
}

.hero-copy .subtitle {
  font-size: 0.78rem;
  color: #64748b;
  margin: 0;
}

.hero-copy .subtitle strong {
  color: #0f172a;
}

@media (max-width: 960px) {
  .back-link {
    position: static;
    justify-content: center;
    margin-bottom: 10px;
  }
}
</style>
