<script setup>
// Merged into a single card (was two): one header instead of two, a
// divider between the table and the chart instead of a second card's
// worth of border/padding/background around each.
import DashboardCard from './DashboardCard.vue'
import GroupObligationsTable from './GroupObligationsTable.vue'
import ContributionDistributionChart from './ContributionDistributionChart.vue'
import { groupObligations, contributionDistribution } from '../../../data/financialMockData.js'
</script>

<template>
  <DashboardCard title="My Group">
    <GroupObligationsTable :obligations="groupObligations" />
    <div class="panel-divider"></div>
    <ContributionDistributionChart :distribution="contributionDistribution" />
  </DashboardCard>
</template>

<style scoped>
.panel-divider {
  height: 1px;
  background: #f1f5f9;
  margin: 2px 0;
}
</style>
