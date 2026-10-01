<script setup>
import { Check } from 'lucide-vue-next'
import { myPaymentObligations, paymentProgress } from '../../../data/financialMockData.js'

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}

function markPaid(item) {
  item.status = 'Paid'
}

const progressPercent = () =>
  paymentProgress.value.totalAmount > 0
    ? (paymentProgress.value.paidAmount / paymentProgress.value.totalAmount) * 100
    : 0

const unpaidCount = () => paymentProgress.value.totalCount - paymentProgress.value.paidCount
const unpaidAmount = () => paymentProgress.value.totalAmount - paymentProgress.value.paidAmount
</script>

<template>
  <div class="payment-history">
    <div class="history-table">
      <div class="history-header">
        <span>Expense</span>
        <span>Due</span>
        <span class="align-right">Amount</span>
        <span>Status</span>
        <span></span>
      </div>
      <div v-for="item in myPaymentObligations" :key="item.id" class="history-row">
        <span class="expense-name">{{ item.expense }}</span>
        <span class="due-date">{{ item.dueDate }}</span>
        <span class="align-right amount">{{ formatCurrency(item.amount) }}</span>
        <span class="status-pill" :class="item.status.toLowerCase()">{{ item.status }}</span>
        <span class="row-action">
          <button v-if="item.status !== 'Paid'" type="button" class="mark-paid-btn" @click="markPaid(item)">
            <Check :size="12" /> Paid
          </button>
          <span v-else class="dash">—</span>
        </span>
      </div>
    </div>

    <div class="progress-block">
      <div class="progress-track">
        <div class="progress-fill" :style="{ width: progressPercent() + '%' }"></div>
      </div>
      <div class="summary-tiles">
        <div class="summary-tile paid">
          <span class="tile-label">Paid</span>
          <span class="tile-value">{{ paymentProgress.paidCount }} · {{ formatCurrency(paymentProgress.paidAmount) }}</span>
        </div>
        <div class="summary-tile unpaid">
          <span class="tile-label">Unpaid</span>
          <span class="tile-value">{{ unpaidCount() }} · {{ formatCurrency(unpaidAmount()) }}</span>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.payment-history {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.history-table {
  display: flex;
  flex-direction: column;
}

.history-header {
  display: grid;
  grid-template-columns: 1.4fr 0.8fr 1fr 1fr 0.7fr;
  gap: 6px;
  padding: 0 2px 5px;
  border-bottom: 1px solid #e2e8f0;
  font-size: 0.58rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  color: #94a3b8;
}

.history-row {
  display: grid;
  grid-template-columns: 1.4fr 0.8fr 1fr 1fr 0.7fr;
  gap: 6px;
  padding: 5px 2px;
  border-bottom: 1px solid #f1f5f9;
  align-items: center;
}

.history-row:last-child {
  border-bottom: none;
}

.expense-name {
  font-size: 0.72rem;
  font-weight: 600;
  color: #0f172a;
}

.due-date {
  font-size: 0.68rem;
  color: #64748b;
}

.amount {
  font-size: 0.72rem;
  font-weight: 700;
  color: #0f172a;
}

.align-right {
  text-align: right;
}

.status-pill {
  font-size: 0.62rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  padding: 2px 8px;
  border-radius: 999px;
  width: fit-content;
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

.row-action {
  display: flex;
  justify-content: flex-end;
}

.mark-paid-btn {
  display: flex;
  align-items: center;
  gap: 4px;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #16a34a;
  font-family: inherit;
  font-size: 0.68rem;
  font-weight: 700;
  padding: 4px 8px;
  border-radius: 6px;
  cursor: pointer;
}

.mark-paid-btn:hover {
  border-color: #bbf7d0;
  background: #f0fdf4;
}

.dash {
  color: #cbd5e1;
  font-size: 0.78rem;
}

.progress-block {
  display: flex;
  flex-direction: column;
  gap: 5px;
  padding-top: 6px;
  border-top: 1px solid #e2e8f0;
}

.progress-track {
  height: 8px;
  border-radius: 999px;
  background: #f1f5f9;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  border-radius: 999px;
  background: linear-gradient(90deg, #1855b9, #2dd4bf);
  transition: width 0.3s ease;
}

.summary-tiles {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 6px;
}

.summary-tile {
  display: flex;
  flex-direction: column;
  gap: 1px;
  border-radius: 7px;
  padding: 6px 9px;
}

.summary-tile.paid {
  background: #f0fdf4;
}

.summary-tile.unpaid {
  background: #fef2f2;
}

.tile-label {
  font-size: 0.6rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.summary-tile.paid .tile-label {
  color: #16a34a;
}
.summary-tile.unpaid .tile-label {
  color: #dc2626;
}

.tile-value {
  font-size: 0.74rem;
  font-weight: 800;
  color: #0f172a;
}
</style>
