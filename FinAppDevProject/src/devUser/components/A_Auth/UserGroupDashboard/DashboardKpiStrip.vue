<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Four glance tiles. Financial Status itself moved into the header (it's
// the cockpit's headline number now, not one tile among several) — this
// strip covers the remaining at-a-glance numbers. "Outstanding" became
// "Available Spending" (Rule 1/7's actual output), since Outstanding
// duplicated what MF2's own summary already shows.
// ─────────────────────────────────────────────────────────────────────────
import { computed } from 'vue'
import { Clock3, AlertCircle, Wallet, CalendarClock } from 'lucide-vue-next'
import { useFinancialStatus } from '../../../composables/A_Auth/useFinancialStatus.js'
import {
  financialStatusInputs,
  pendingContributionChanges,
  groupPolicyChangeProposal,
  upcomingPayments
} from '../../../data/financialMockData.js'

const { remainingSpendingMoney, nextIncomeInDays } = useFinancialStatus(financialStatusInputs)

const openApprovalsCount = computed(
  () => pendingContributionChanges.value.length + (groupPolicyChangeProposal.value ? 1 : 0)
)

function formatCurrency(value) {
  return `R${Math.round(value).toLocaleString('en-ZA')}`
}
</script>

<template>
  <div class="kpi-strip">
    <div class="kpi-tile">
      <Wallet :size="15" class="kpi-icon-svg spending" />
      <div class="kpi-text">
        <span class="kpi-label">Available Spending</span>
        <span class="kpi-value">{{ formatCurrency(remainingSpendingMoney) }}</span>
      </div>
    </div>

    <div class="kpi-tile">
      <Clock3 :size="15" class="kpi-icon-svg" />
      <div class="kpi-text">
        <span class="kpi-label">Next Income</span>
        <span class="kpi-value">In {{ nextIncomeInDays }} days</span>
      </div>
    </div>

    <div class="kpi-tile">
      <CalendarClock :size="15" class="kpi-icon-svg" />
      <div class="kpi-text">
        <span class="kpi-label">Upcoming Payments</span>
        <span class="kpi-value">{{ upcomingPayments.length }}</span>
      </div>
    </div>

    <div class="kpi-tile" :class="{ 'has-approvals': openApprovalsCount > 0 }">
      <AlertCircle :size="15" class="kpi-icon-svg" :class="{ alert: openApprovalsCount > 0 }" />
      <div class="kpi-text">
        <span class="kpi-label">Open Approvals</span>
        <span class="kpi-value">{{ openApprovalsCount }}</span>
      </div>
    </div>
  </div>
</template>

<style scoped>
.kpi-strip {
  display: grid;
  grid-template-columns: repeat(4, 1fr);
  gap: 10px;
}

.kpi-tile {
  display: flex;
  align-items: center;
  gap: 9px;
  background: #fff;
  border: 1px solid #e2e8f0;
  border-radius: 9px;
  padding: 9px 12px;
  min-width: 0;
}

.kpi-tile.has-approvals {
  border-color: #fed7aa;
  background: #fffaf5;
}

.kpi-icon-svg {
  color: #94a3b8;
  flex-shrink: 0;
}

.kpi-icon-svg.spending {
  color: #1855b9;
}

.kpi-icon-svg.alert {
  color: #c2410c;
}

.kpi-text {
  display: flex;
  flex-direction: column;
  gap: 1px;
  min-width: 0;
}

.kpi-label {
  font-size: 0.62rem;
  color: #94a3b8;
  font-weight: 600;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}

.kpi-value {
  font-size: 0.82rem;
  color: #0f172a;
  font-weight: 800;
  white-space: nowrap;
}

@media (max-width: 900px) {
  .kpi-strip {
    grid-template-columns: repeat(2, 1fr);
  }
}
</style>
