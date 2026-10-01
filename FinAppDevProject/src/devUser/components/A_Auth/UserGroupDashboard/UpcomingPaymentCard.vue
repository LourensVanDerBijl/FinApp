<script setup>
// CA3 detail content — full payment context plus the complete recording
// flow (Paid/Unpaid/Short Payment/Over Paid, each with the fields the
// spec calls for). The row itself only offers a quick "Paid" shortcut;
// everything else lives here.
import { ref } from 'vue'
import { CircleDollarSign, Ban, SplitSquareHorizontal, TrendingUp } from 'lucide-vue-next'
import ActionButton from './ActionButton.vue'

const props = defineProps({
  payment: { type: Object, required: true }
})

const emit = defineEmits(['record'])

const activeAction = ref(null) // 'paid' | 'unpaid' | 'short' | 'overpaid' | null
const reason = ref('')
const rescheduledDate = ref('')
const actualAmountPaid = ref('')

function openAction(action) {
  if (action === 'paid') {
    emit('record', { id: props.payment.id, action: 'paid' })
    return
  }
  activeAction.value = action
  reason.value = ''
  rescheduledDate.value = ''
  actualAmountPaid.value = ''
}

function submit() {
  if (!reason.value.trim()) return
  const payload = { id: props.payment.id, action: activeAction.value, reason: reason.value.trim() }

  if (activeAction.value === 'unpaid') payload.rescheduledDate = rescheduledDate.value
  if (activeAction.value === 'short') {
    payload.actualAmountPaid = Number(actualAmountPaid.value) || 0
    payload.outstanding = props.payment.amount - (Number(actualAmountPaid.value) || 0)
    payload.rescheduledDate = rescheduledDate.value
  }
  if (activeAction.value === 'overpaid') {
    payload.actualAmountPaid = Number(actualAmountPaid.value) || 0
    payload.overpaidBy = (Number(actualAmountPaid.value) || 0) - props.payment.amount
  }

  emit('record', payload)
  activeAction.value = null
}

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}
</script>

<template>
  <div class="detail-block">
    <span class="status-pill" :class="payment.status.toLowerCase().replace(' ', '-')">{{ payment.status }}</span>

    <h3 class="detail-title">{{ payment.name }}</h3>

    <p v-if="payment.description" class="description">{{ payment.description }}</p>

    <dl class="meta-list">
      <div>
        <dt>Due</dt>
        <dd>{{ payment.dueDate }}</dd>
      </div>
      <div>
        <dt>Type</dt>
        <dd>{{ payment.type }}</dd>
      </div>
      <div>
        <dt>Amount</dt>
        <dd class="amount">{{ formatCurrency(payment.amount) }}</dd>
      </div>
      <div>
        <dt>Responsible</dt>
        <dd>{{ payment.responsiblePerson }}</dd>
      </div>
    </dl>

    <div v-if="!activeAction" class="record-options">
      <span class="reason-label">Record payment status</span>
      <div class="options-grid">
        <ActionButton variant="success" @click="openAction('paid')">
          <template #icon><CircleDollarSign :size="13" /></template>
          Paid
        </ActionButton>
        <ActionButton variant="danger" @click="openAction('unpaid')">
          <template #icon><Ban :size="13" /></template>
          Unpaid
        </ActionButton>
        <ActionButton variant="warning" @click="openAction('short')">
          <template #icon><SplitSquareHorizontal :size="13" /></template>
          Short Payment
        </ActionButton>
        <ActionButton variant="primary" @click="openAction('overpaid')">
          <template #icon><TrendingUp :size="13" /></template>
          Over Paid
        </ActionButton>
      </div>
    </div>

    <div v-else class="action-form">
      <textarea
        v-model="reason"
        rows="2"
        :placeholder="
          activeAction === 'unpaid'
            ? 'Why wasn\'t this paid? (required)'
            : activeAction === 'short'
            ? 'Why was this paid short? (required)'
            : 'Why did you pay more than expected? (required)'
        "
      ></textarea>

      <div v-if="activeAction === 'short' || activeAction === 'overpaid'" class="field-row">
        <label>Actual amount paid</label>
        <input type="number" v-model="actualAmountPaid" placeholder="0" />
      </div>

      <div v-if="activeAction === 'unpaid' || activeAction === 'short'" class="field-row">
        <label>Rescheduled date</label>
        <input type="date" v-model="rescheduledDate" />
      </div>

      <div class="form-actions">
        <ActionButton variant="neutral" @click="activeAction = null">Cancel</ActionButton>
        <ActionButton variant="primary" :disabled="!reason.trim()" @click="submit">Confirm</ActionButton>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detail-block {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.status-pill {
  align-self: flex-start;
  font-size: 0.66rem;
  font-weight: 700;
  padding: 3px 9px;
  border-radius: 999px;
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

.detail-title {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 800;
  color: #0f172a;
}

.description {
  margin: 0;
  font-size: 0.8rem;
  color: #64748b;
  line-height: 1.5;
}

.meta-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin: 0;
}

.meta-list > div {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid #f1f5f9;
  padding-bottom: 8px;
}

.meta-list dt {
  font-size: 0.78rem;
  color: #64748b;
}

.meta-list dd {
  margin: 0;
  font-size: 0.78rem;
  font-weight: 700;
  color: #0f172a;
}

.meta-list dd.amount {
  color: #1855b9;
  font-weight: 800;
}

.reason-label {
  font-size: 0.66rem;
  color: #94a3b8;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.record-options {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.options-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 8px;
}

.options-grid :deep(.action-btn) {
  justify-content: center;
}

.action-form {
  display: flex;
  flex-direction: column;
  gap: 9px;
}

.action-form textarea {
  width: 100%;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 9px 11px;
  font-family: inherit;
  font-size: 0.82rem;
  color: #0f172a;
  resize: vertical;
  outline: none;
}

.action-form textarea:focus,
.field-row input:focus {
  border-color: #1855b9;
}

.field-row {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
}

.field-row label {
  font-size: 0.78rem;
  color: #64748b;
  font-weight: 600;
  flex-shrink: 0;
}

.field-row input {
  border: 1px solid #e2e8f0;
  border-radius: 7px;
  padding: 7px 10px;
  font-family: inherit;
  font-size: 0.8rem;
  color: #0f172a;
  outline: none;
  width: 150px;
}

.form-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
