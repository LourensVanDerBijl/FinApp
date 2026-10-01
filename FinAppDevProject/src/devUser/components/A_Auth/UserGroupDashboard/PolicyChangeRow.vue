<script setup>
// CA2 — shows only that an open approval exists and who proposed it.
// Everything else (purpose, when, impact, comments, actions) lives behind
// the click, in the detail drawer.
import { ChevronRight, FileEdit } from 'lucide-vue-next'

defineProps({
  proposal: { type: Object, required: true }
})

defineEmits(['open'])
</script>

<template>
  <button type="button" class="policy-row" @click="$emit('open', proposal)">
    <FileEdit :size="14" class="policy-icon" />
    <div class="row-main">
      <span class="policy-title">Group Policy Change</span>
      <span class="policy-sub">Proposed by {{ proposal.proposedBy }}</span>
    </div>
    <span class="status-pill pending">Open Approval</span>
    <ChevronRight :size="14" class="chevron" />
  </button>
</template>

<style scoped>
.policy-row {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 9px;
  border: 1px solid #fed7aa;
  border-radius: 8px;
  padding: 8px 9px;
  background: #fffaf5;
  cursor: pointer;
  font-family: inherit;
  text-align: left;
  transition: border-color 0.15s, background 0.15s;
}

.policy-row:hover {
  border-color: #fdba74;
  background: #fff5eb;
}

.policy-icon {
  color: #c2410c;
  flex-shrink: 0;
}

.row-main {
  flex: 1;
  min-width: 0;
  display: flex;
  flex-direction: column;
  gap: 1px;
}

.policy-title {
  font-size: 0.76rem;
  font-weight: 700;
  color: #0f172a;
}

.policy-sub {
  font-size: 0.68rem;
  color: #94a3b8;
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
  background: #fff;
  color: #c2410c;
  border: 1px solid #fed7aa;
}

.chevron {
  color: #fdba74;
  flex-shrink: 0;
}
</style>
