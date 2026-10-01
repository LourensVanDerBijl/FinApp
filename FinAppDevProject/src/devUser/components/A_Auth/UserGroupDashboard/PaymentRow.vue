<script setup>
// CA3 — date, amount, status, and one button. For Unpaid rows that button
// is a quick "Paid" — click it and the status updates immediately, no
// drawer needed for the common case. The row itself (not the button)
// opens the full detail drawer, which also holds Unpaid/Short/Over Paid
// recording for the less common cases.
import { Check } from 'lucide-vue-next'
import ActionButton from './ActionButton.vue'

defineProps({
  payment: { type: Object, required: true }
})

const emit = defineEmits(['open', 'quick-paid'])

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}
</script>

<template>
  <div class="payment-row" @click="$emit('open', payment)">
    <div class="row-main">
      <span class="payment-name">{{ payment.name }}</span>
      <span class="payment-sub">{{ payment.dueDate }} · {{ formatCurrency(payment.amount) }}</span>
    </div>
    <span class="status-pill" :class="payment.status.toLowerCase().replace(' ', '-')">{{ payment.status }}</span>
    <ActionButton
      v-if="payment.status === 'Unpaid'"
      variant="success"
      size="xs"
      @click.stop="emit('quick-paid', payment)"
    >
      <template #icon><Check :size="11" /></template>
      Paid
    </ActionButton>
  </div>
</template>

<style scoped>
.payment-row {
  width: 100%;
  display: flex;
  align-items: center;
  gap: 8px;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 8px 9px;
  background: #fafbfc;
  cursor: pointer;
  transition: border-color 0.15s, background 0.15s;
}

.payment-row:hover {
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

.payment-name {
  font-size: 0.76rem;
  font-weight: 700;
  color: #0f172a;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.payment-sub {
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

.status-pill.unpaid {
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
}

.status-pill.paid {
  background: #f0fdf4;
  color: #16a34a;
  border: 1px solid #bbf7d0;
}

.status-pill.short-payment {
  background: #fff7ed;
  color: #c2410c;
  border: 1px solid #fed7aa;
}

.status-pill.over-paid {
  background: #eff6ff;
  color: #1855b9;
  border: 1px solid #bfdbfe;
}
</style>
