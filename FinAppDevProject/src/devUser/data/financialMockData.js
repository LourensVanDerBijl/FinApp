// ─────────────────────────────────────────────────────────────────────────
// devUser/data/financialMockData.js
//
// Mock data for the User Group Dashboard. Separate on purpose from
// userMockData.js: that file holds real, backend-connected identity/group
// calls (registration, login, group create/join). Nothing here is backend
// data yet — FinBine's financial functionality (budgets, contributions,
// obligations, payment tracking) is still Phase-2/Epic-2 work with no API
// behind it, so this file is the "shape the real data will eventually take"
// stand-in, using deliberately fictional figures — not any real user's data.
//
// Field set is reconciled directly against the source dashboard spec
// (Shared_Fin_Plan.xlsx, Sheet1) — every field named there (who/when/
// purpose/impact for a policy change, initiator/reason for a contribution
// change, etc.) is represented here, even where the dashboard UI only
// surfaces it one click deep in a detail view rather than inline.
//
// When the financial APIs exist, each export below becomes a fetch call
// (mirroring how userMockData.js wraps the identity/group endpoints) and
// the components consuming these exports shouldn't need to change shape.
// ─────────────────────────────────────────────────────────────────────────
import { ref } from 'vue'

// ── My Financial Status (Rule 1/2/3/7 inputs) ──────────────────────────
// Available spending money = Income − Group Expenses − Personal Expenses.
// actualSpending is then subtracted from that to get what's left (Rule 7).
export const financialStatusInputs = ref({
  expectedIncome: 24800,
  groupExpenses: 9150,
  personalExpenses: 6300,
  actualSpending: 2050,
  nextIncomeInDays: 12
})

// ── My Group — group obligations this user contributes toward (Block MG1) ──
export const groupObligations = ref([
  { id: 'ob-1', name: 'Home Loan', total: 18500, myShare: 7607, myPercent: 41.1 },
  { id: 'ob-2', name: 'Kids School Fees', total: 5400, myShare: 2257, myPercent: 41.8 },
  { id: 'ob-3', name: 'Groceries', total: 4200, myShare: 1756, myPercent: 41.8 },
  { id: 'ob-4', name: 'Electricity & Water', total: 2150, myShare: 899, myPercent: 41.8 }
])

// ── Group contribution distribution (Block MH2 — pie chart) ────────────
export const contributionDistribution = ref({
  labels: ['You', 'Jordan'],
  series: [41.8, 58.2]
})

// ── Pending contribution changes awaiting this user's approval (CA Block 1) ──
// An array because the spec calls for navigating multiple pending changes,
// not just a single hardcoded one. Fields beyond old/new/status/responsible
// (initiatedBy, requestedAt, reason) support the detail view a row opens
// into — the compact row only shows a summary, not these directly.
export const pendingContributionChanges = ref([
  {
    id: 'ch-1',
    obligationName: 'Home Loan',
    oldContribution: 4600,
    newContribution: 7607,
    status: 'Awaiting Approval',
    responsiblePerson: 'Jordan',
    initiatedBy: 'Jordan',
    requestedAt: '2026-09-15T09:20:00',
    reason: 'Home loan repayment increased at renewal — contribution split needs to follow the updated total.'
  }
])

// ── Group policy change proposal (CA Block 2) ───────────────────────────
// At most one pending proposal at a time, per the rules — null means
// "nothing outstanding." purpose/proposedAt/myOldShare/myNewShare/comments
// exist for the detail view ("who requested what, when, the impact,
// actions, and comments") — the compact summary row only shows that one
// exists and who proposed it.
export const groupPolicyChangeProposal = ref({
  id: 'policy-1',
  purpose: 'Increase total monthly shared contribution',
  proposedBy: 'Jordan',
  proposedAt: '2026-09-16T18:05:00',
  ownerComment: 'Rent increased on renewal, so total shared contribution needs to go up to cover it.',
  oldTotalContribution: 32480,
  newTotalContribution: 35100,
  myOldShare: 4600,
  myNewShare: 4970,
  status: 'Pending Approval',
  comments: [
    { author: 'Jordan', at: '2026-09-16T18:06:00', text: 'Landlord confirmed the increase in writing yesterday.' }
  ]
})

// ── Upcoming payments this user is responsible for (CA3) ───────────────
// description exists for the detail view; the compact row only shows
// date, amount, status, and one action.
export const upcomingPayments = ref([
  {
    id: 'pay-1',
    name: 'Home Loan',
    dueDate: 'Friday',
    type: 'Debit Order',
    amount: 7607,
    responsiblePerson: 'You',
    status: 'Unpaid',
    description: 'Monthly bond repayment, debited automatically from the group\u2019s nominated account.'
  },
  {
    id: 'pay-2',
    name: 'Kids School Fees',
    dueDate: 'Mon 28th',
    type: 'EFT',
    amount: 2257,
    responsiblePerson: 'You',
    status: 'Unpaid',
    description: 'Term fees, paid manually via EFT to the school\u2019s account.'
  },
  {
    id: 'pay-3',
    name: 'Electricity & Water',
    dueDate: 'Wed 30th',
    type: 'Debit Order',
    amount: 899,
    responsiblePerson: 'You',
    status: 'Unpaid',
    description: 'Municipal account, split by the group\u2019s agreed allocation.'
  }
])

// ── My payment obligations this period (Block MF2) ──────────────────────
// Per the rules: group obligations are net-settled and shown as a single
// "Group Contribution" line — individual group expenses aren't broken out
// here. Personal expenses are listed individually.
export const myPaymentObligations = ref([
  { id: 'mf2-1', expense: 'Group Contribution', dueDate: 'Friday', amount: 4600, status: 'Unpaid' },
  { id: 'mf2-2', expense: 'Car Finance', dueDate: 'Monday', amount: 1450, status: 'Paid' },
  { id: 'mf2-3', expense: 'Phone', dueDate: '25th', amount: 399, status: 'Unpaid' },
  { id: 'mf2-4', expense: 'Investment', dueDate: '28th', amount: 1800, status: 'Paid' }
])

export const paymentProgress = ref({
  paidCount: 11,
  totalCount: 20,
  paidAmount: 16200,
  totalAmount: 26800
})
