<script setup>
import SectionCard from '../sharedComponents/SectionCard.vue'
import StatusBadge from '../sharedComponents/StatusBadge.vue'

// Example placeholder data
const tickets = [
  { id: '#101', title: 'Payment gateway error', status: 'Open', assignee: 'John Doe' },
  { id: '#102', title: 'UI bug on dashboard', status: 'In Progress', assignee: 'Jane Smith' },
  { id: '#103', title: 'Feature request: Dark mode', status: 'Closed', assignee: 'Admin' }
]
</script>

<template>
  <SectionCard>
    <h3>Support Tickets</h3>
    <div class="ticket-list">
      <div v-for="ticket in tickets" :key="ticket.id" class="ticket-item">
        <div class="ticket-header">
          <p class="ticket-id">{{ ticket.id }}</p>
          <StatusBadge :status="ticket.status" />
        </div>
        <p class="ticket-title">{{ ticket.title }}</p>
        <p class="ticket-assignee">Assigned to: {{ ticket.assignee }}</p>
      </div>
    </div>
  </SectionCard>
</template>

<style scoped>
.ticket-list {
  display: flex;
  flex-direction: column;
  gap: 12px;
}
.ticket-item {
  background: #f9fafb;
  padding: 12px;
  border-radius: 8px;
}
.ticket-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
.ticket-id {
  font-weight: 600;
}
.ticket-title {
  font-size: 14px;
  margin-top: 4px;
}
.ticket-assignee {
  font-size: 12px;
  color: #6b7280;
}
</style>
