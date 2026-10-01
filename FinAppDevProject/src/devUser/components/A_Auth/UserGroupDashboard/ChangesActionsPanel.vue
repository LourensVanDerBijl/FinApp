<script setup>
// ─────────────────────────────────────────────────────────────────────────
// "Changes & Actions" column — v3. Every item is now a compact, clickable
// row; nothing opens inline anymore. Clicking a row opens the shared
// DashboardDetailDrawer with that item's full detail component inside,
// where the actual Approve/Question/Decline/record actions live.
//
// CA3 shows only the next few payments (default 2) with a "+N more"
// toggle, per the brief — the full list still exists in the mock data,
// it's just not all rendered open at once.
//
// Mutations still happen on the mock refs directly — no backend yet, same
// pattern as before.
// ─────────────────────────────────────────────────────────────────────────
import { ref, computed } from 'vue'
import PendingChangeRow from './PendingChangeRow.vue'
import PolicyChangeRow from './PolicyChangeRow.vue'
import PaymentRow from './PaymentRow.vue'
import PendingChangeCard from './PendingChangeCard.vue'
import PolicyChangeCard from './PolicyChangeCard.vue'
import UpcomingPaymentCard from './UpcomingPaymentCard.vue'
import DashboardDetailDrawer from './DashboardDetailDrawer.vue'
import { CheckCircle2, ChevronDown } from 'lucide-vue-next'
import {
  pendingContributionChanges,
  groupPolicyChangeProposal,
  upcomingPayments
} from '../../../data/financialMockData.js'

const PAYMENTS_PREVIEW_COUNT = 2
const showAllPayments = ref(false)

const visiblePayments = computed(() =>
  showAllPayments.value ? upcomingPayments.value : upcomingPayments.value.slice(0, PAYMENTS_PREVIEW_COUNT)
)
const hiddenPaymentsCount = computed(() =>
  Math.max(0, upcomingPayments.value.length - PAYMENTS_PREVIEW_COUNT)
)

// ── Drawer state — one shared drawer, fed whichever item was clicked ──
const drawer = ref(null) // { kind: 'change' | 'policy' | 'payment', item }

function openChange(item) {
  drawer.value = { kind: 'change', item }
}
function openPolicy(item) {
  drawer.value = { kind: 'policy', item }
}
function openPayment(item) {
  drawer.value = { kind: 'payment', item }
}
function closeDrawer() {
  drawer.value = null
}

const drawerTitle = computed(() => {
  if (!drawer.value) return ''
  if (drawer.value.kind === 'change') return 'Contribution Change'
  if (drawer.value.kind === 'policy') return 'Group Policy Change'
  return 'Payment Detail'
})

// ── Mutations ──────────────────────────────────────────────────────────
function handleApproveChange(id) {
  pendingContributionChanges.value = pendingContributionChanges.value.filter((c) => c.id !== id)
  closeDrawer()
}
function handleQuestionChange({ id, comment }) {
  const change = pendingContributionChanges.value.find((c) => c.id === id)
  if (change) change.status = `Questioned`
  closeDrawer()
}

function handlePolicyApprove() {
  groupPolicyChangeProposal.value = null
  closeDrawer()
}
function handlePolicyDecline() {
  groupPolicyChangeProposal.value = null
  closeDrawer()
}
function handlePolicyQuestion() {
  if (groupPolicyChangeProposal.value) groupPolicyChangeProposal.value.status = 'Questioned'
  closeDrawer()
}

function handlePaymentRecord({ id, action }) {
  if (action === 'paid') {
    const payment = upcomingPayments.value.find((p) => p.id === id)
    if (payment) payment.status = 'Paid'
  } else {
    const payment = upcomingPayments.value.find((p) => p.id === id)
    if (payment) {
      payment.status = action === 'unpaid' ? 'Unpaid' : action === 'short' ? 'Short Payment' : 'Over Paid'
    }
  }
  closeDrawer()
}

function handleQuickPaid(payment) {
  payment.status = 'Paid'
}

const nothingOutstanding = computed(
  () => pendingContributionChanges.value.length === 0 && !groupPolicyChangeProposal.value && upcomingPayments.value.length === 0
)
</script>

<template>
  <div class="ca-panel">
    <div v-if="nothingOutstanding" class="all-caught-up">
      <CheckCircle2 :size="24" />
      <p>You're all caught up.</p>
    </div>

    <template v-else>
      <section v-if="pendingContributionChanges.length" class="ca-section">
        <h4>Pending Changes</h4>
        <PendingChangeRow
          v-for="change in pendingContributionChanges"
          :key="change.id"
          :change="change"
          @open="openChange"
        />
      </section>

      <section v-if="groupPolicyChangeProposal" class="ca-section">
        <h4>Policy Changes</h4>
        <PolicyChangeRow :proposal="groupPolicyChangeProposal" @open="openPolicy" />
      </section>

      <section v-if="upcomingPayments.length" class="ca-section">
        <h4>Upcoming Payments</h4>
        <PaymentRow
          v-for="payment in visiblePayments"
          :key="payment.id"
          :payment="payment"
          @open="openPayment"
          @quick-paid="handleQuickPaid"
        />
        <button
          v-if="hiddenPaymentsCount > 0 && !showAllPayments"
          type="button"
          class="show-more"
          @click="showAllPayments = true"
        >
          <ChevronDown :size="13" />
          {{ hiddenPaymentsCount }} more
        </button>
      </section>
    </template>

    <DashboardDetailDrawer :open="!!drawer" :title="drawerTitle" @close="closeDrawer">
      <PendingChangeCard
        v-if="drawer?.kind === 'change'"
        :change="drawer.item"
        @approve="handleApproveChange"
        @question="handleQuestionChange"
      />
      <PolicyChangeCard
        v-else-if="drawer?.kind === 'policy'"
        :proposal="drawer.item"
        @approve="handlePolicyApprove"
        @decline="handlePolicyDecline"
        @question="handlePolicyQuestion"
      />
      <UpcomingPaymentCard
        v-else-if="drawer?.kind === 'payment'"
        :payment="drawer.item"
        @record="handlePaymentRecord"
      />
    </DashboardDetailDrawer>
  </div>
</template>

<style scoped>
.ca-panel {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.ca-section {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.ca-section h4 {
  margin: 0;
  font-size: 0.62rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.03em;
  color: #94a3b8;
}

.show-more {
  align-self: flex-start;
  display: flex;
  align-items: center;
  gap: 4px;
  border: none;
  background: none;
  color: #1855b9;
  font-family: inherit;
  font-size: 0.72rem;
  font-weight: 700;
  cursor: pointer;
  padding: 3px 2px;
}

.show-more:hover {
  text-decoration: underline;
}

.all-caught-up {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  gap: 6px;
  padding: 18px 12px;
  color: #94a3b8;
}

.all-caught-up svg {
  color: #16a34a;
}

.all-caught-up p {
  margin: 0;
  font-size: 0.84rem;
  font-weight: 600;
  color: #64748b;
}
</style>
