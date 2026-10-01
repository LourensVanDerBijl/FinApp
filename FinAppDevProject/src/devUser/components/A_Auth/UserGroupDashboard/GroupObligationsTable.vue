<script setup>
defineProps({
  obligations: { type: Array, required: true }
})

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}
</script>

<template>
  <div class="obligations-table">
    <div class="table-header">
      <span>Obligation</span>
      <span class="align-right">Total</span>
      <span class="align-right">My Share</span>
      <span class="align-right">My %</span>
    </div>
    <div v-for="ob in obligations" :key="ob.id" class="table-row">
      <span class="ob-name">{{ ob.name }}</span>
      <span class="align-right ob-total">{{ formatCurrency(ob.total) }}</span>
      <span class="align-right ob-share">{{ formatCurrency(ob.myShare) }}</span>
      <span class="align-right ob-percent">{{ ob.myPercent.toFixed(1) }}%</span>
    </div>
  </div>
</template>

<style scoped>
.obligations-table {
  display: flex;
  flex-direction: column;
}

.table-header {
  display: grid;
  grid-template-columns: 1.6fr 1fr 1fr 0.7fr;
  gap: 6px;
  padding: 0 2px 5px;
  border-bottom: 1px solid #e2e8f0;
  font-size: 0.6rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.02em;
  color: #94a3b8;
}

.table-row {
  display: grid;
  grid-template-columns: 1.6fr 1fr 1fr 0.7fr;
  gap: 6px;
  padding: 6px 2px;
  border-bottom: 1px solid #f1f5f9;
  align-items: center;
}

.table-row:last-child {
  border-bottom: none;
}

.ob-name {
  font-size: 0.74rem;
  font-weight: 600;
  color: #0f172a;
}

.ob-total {
  font-size: 0.72rem;
  color: #64748b;
}

.ob-share {
  font-size: 0.74rem;
  font-weight: 700;
  color: #1855b9;
}

.ob-percent {
  font-size: 0.68rem;
  color: #94a3b8;
}

.align-right {
  text-align: right;
}
</style>
