<script setup>
// CA1 — a compact, clickable row. No inline actions, no expanding
// textarea — the whole row opens the detail drawer, where the full
// context (who/when/why) and Approve/Question actions live.
import { ArrowRight, ChevronRight } from 'lucide-vue-next'

defineProps({
  change: { type: Object, required: true }
})

defineEmits(['open'])

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}
</script>

<template>
  <button type="button" class="change-row" @click="$emit('open', change)">
    <div class="row-main">
      <span class="obligation-name">{{ change.obligationName }}</span>
      <span class="figures">
        <span class="old">{{ formatCurrency(change.oldContribution) }}</span>
        <ArrowRight :size="10" />
        <span class="new">{{ formatCurrency(change.newContribution) }}</span>
      </span>
    </div>
    <span class="status-pill pending">Awaiting Approval</span>
    <ChevronRight :size="14" class="chevron" />
  </button>
</template>

<style scoped>
.change-row {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 8px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 8px 9px;
  background: #fafbfc;
  cursor: pointer;
  font-family: inherit;
  text-align: left;
  transition: border-color 0.15s, background 0.15s;
}

.change-row:hover {
  border-color: #cbd5e1;
  background: #fff;
}

.row-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 2px;
}

.obligation-name {
  font-size: 0.76rem;
  font-weight: 700;
  color: #0f172a;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.figures {
  display: flex;
  align-items: center;
  gap: 4px;
  font-size: 0.7rem;
  color: #94a3b8;
}

.figures svg {
  color: #cbd5e1;
  flex-shrink: 0;
}

.figures .old {
  text-decoration: line-through;
}

.figures .new {
  color: #0f172a;
  font-weight: 700;
}

.status-pill {
  font-size: 0.58rem;
  font-weight: 700;
  padding: 2px 7px;
  border-radius: 999px;
  flex-shrink: 0;
  white-space: nowrap;
}

.status-pill.pending {
  background: #fff7ed;
  color: #c2410c;
  border: 1px solid #fed7aa;
}

.chevron {
  color: #cbd5e1;
  flex-shrink: 0;
}
</style>
