<script setup>
// Status badge now reads as a button (solid fill, shadow, hover, click
// target) rather than a plain pill — clicking it reveals the Rule 1
// formula breakdown (Income − Group − Personal = Available), so the
// "why" behind the number is one click away instead of hidden.
import { ref } from 'vue'
import { Clock, TrendingUp, Users, ChevronDown } from 'lucide-vue-next'
import { useFinancialStatus } from '../../../composables/A_Auth/useFinancialStatus.js'
import { financialStatusInputs } from '../../../data/financialMockData.js'

const { availableSpendingMoney, remainingSpendingMoney, percentRemaining, tier, nextIncomeInDays } =
  useFinancialStatus(financialStatusInputs)

const showBreakdown = ref(false)

function formatCurrency(value) {
  return `R${Math.round(value).toLocaleString('en-ZA')}`
}
</script>

<template>
  <div class="status-widget">
    <button type="button" class="status-badge-btn" :class="tier.key" @click="showBreakdown = !showBreakdown">
      <span class="status-emoji">{{ tier.emoji }}</span>
      <span class="status-label">{{ tier.label }}</span>
      <span class="status-percent">{{ percentRemaining.toFixed(0) }}%</span>
      <ChevronDown :size="13" class="chevron" :class="{ open: showBreakdown }" />
    </button>

    <div class="remaining-bar-track">
      <div
        class="remaining-bar-fill"
        :class="tier.key"
        :style="{ width: Math.max(0, Math.min(100, percentRemaining)) + '%' }"
      ></div>
    </div>

    <div v-if="showBreakdown" class="breakdown">
      <div class="breakdown-row">
        <span>Expected income</span>
        <span>{{ formatCurrency(financialStatusInputs.expectedIncome) }}</span>
      </div>
      <div class="breakdown-row minus">
        <span>Group expenses</span>
        <span>− {{ formatCurrency(financialStatusInputs.groupExpenses) }}</span>
      </div>
      <div class="breakdown-row minus">
        <span>Personal expenses</span>
        <span>− {{ formatCurrency(financialStatusInputs.personalExpenses) }}</span>
      </div>
      <div class="breakdown-row total">
        <span>Available spending money</span>
        <span>{{ formatCurrency(availableSpendingMoney) }}</span>
      </div>
      <div class="breakdown-row minus">
        <span>Spent so far</span>
        <span>− {{ formatCurrency(financialStatusInputs.actualSpending) }}</span>
      </div>
      <div class="breakdown-row total final">
        <span>Remaining</span>
        <span>{{ formatCurrency(remainingSpendingMoney) }}</span>
      </div>
    </div>

    <div class="status-stats">
      <div class="stat">
        <Clock :size="12" class="stat-icon" />
        <span class="stat-value">{{ nextIncomeInDays }}d</span>
        <span class="stat-label">to income</span>
      </div>
      <div class="stat">
        <TrendingUp :size="12" class="stat-icon" />
        <span class="stat-value">{{ formatCurrency(financialStatusInputs.expectedIncome) }}</span>
        <span class="stat-label">income</span>
      </div>
      <div class="stat">
        <Users :size="12" class="stat-icon" />
        <span class="stat-value">{{ formatCurrency(financialStatusInputs.groupExpenses) }}</span>
        <span class="stat-label">group</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.status-widget {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.status-badge-btn {
  display: flex;
  align-items: center;
  gap: 7px;
  width: fit-content;
  padding: 6px 12px;
  border-radius: 9px;
  border: none;
  font-family: inherit;
  font-size: 0.78rem;
  font-weight: 700;
  cursor: pointer;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.06);
  transition: filter 0.15s;
}

.status-badge-btn:hover {
  filter: brightness(0.97);
}

.status-badge-btn.excellent {
  background: #1855b9;
  color: #fff;
}
.status-badge-btn.good {
  background: #16a34a;
  color: #fff;
}
.status-badge-btn.watch {
  background: #f97316;
  color: #fff;
}
.status-badge-btn.critical,
.status-badge-btn.danger {
  background: #dc2626;
  color: #fff;
}

.status-percent {
  opacity: 0.85;
  font-weight: 600;
}

.chevron {
  transition: transform 0.15s;
  opacity: 0.85;
}
.chevron.open {
  transform: rotate(180deg);
}

.remaining-bar-track {
  height: 5px;
  border-radius: 999px;
  background: #f1f5f9;
  overflow: hidden;
}

.remaining-bar-fill {
  height: 100%;
  border-radius: 999px;
  transition: width 0.3s ease;
}

.remaining-bar-fill.excellent {
  background: #1855b9;
}
.remaining-bar-fill.good {
  background: #16a34a;
}
.remaining-bar-fill.watch {
  background: #f97316;
}
.remaining-bar-fill.critical,
.remaining-bar-fill.danger {
  background: #dc2626;
}

.breakdown {
  display: flex;
  flex-direction: column;
  gap: 4px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 8px 10px;
}

.breakdown-row {
  display: flex;
  justify-content: space-between;
  font-size: 0.72rem;
  color: #64748b;
}

.breakdown-row.minus span:last-child {
  color: #dc2626;
}

.breakdown-row.total {
  font-weight: 700;
  color: #0f172a;
  border-top: 1px dashed #e2e8f0;
  padding-top: 4px;
  margin-top: 2px;
}

.breakdown-row.total.final {
  color: #1855b9;
  border-top: 1px solid #e2e8f0;
}

.status-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 6px;
  padding-top: 4px;
}

.stat {
  display: flex;
  align-items: baseline;
  gap: 4px;
  min-width: 0;
}

.stat-icon {
  color: #94a3b8;
  flex-shrink: 0;
  align-self: center;
}

.stat-value {
  font-size: 0.74rem;
  color: #0f172a;
  font-weight: 800;
  white-space: nowrap;
}

.stat-label {
  font-size: 0.64rem;
  color: #94a3b8;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
</style>
