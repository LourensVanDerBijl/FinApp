<script setup>
// Merged into a single card (was two) — same reasoning as MyGroupPanel.
import DashboardCard from './DashboardCard.vue'
import FinancialStatusCard from './FinancialStatusCard.vue'
import PaymentHistoryTable from './PaymentHistoryTable.vue'
</script>

<template>
  <DashboardCard title="My Finances">
    <FinancialStatusCard />
    <div class="panel-divider"></div>
    <PaymentHistoryTable />
  </DashboardCard>
</template>

<style scoped>
.panel-divider {
  height: 1px;
  background: #f1f5f9;
  margin: 2px 0;
}
</style>
