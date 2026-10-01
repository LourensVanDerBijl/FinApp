// src/devAdministrator/data/mockData.js

import { ref, computed } from 'vue'
import { onAuthStateChanged } from 'firebase/auth'
import { auth } from '../../firebase/firebaseManager.js'

const API_BASE = 'https://localhost:5001'

// ---------------------------------------------------------
// Side Navigation mock data
// ---------------------------------------------------------
export const adminInfo = {
  name: "Louis Van Der Bijl",
  role: "Super Admin"
}

// ---------------------------------------------------------
// Admin org chart — dummy data for the Admin Users page.
// Field names (preferName/surname) intentionally match
// AdminAccount.cs so this drops in cleanly once there's a
// real "reports_to" column to build the tree from.
//
// addOrgChartMember() only mutates this local tree — it's a
// UI proof of concept, not wired to a backend yet. Creating an
// admin for real will also need to provision a user-table
// entry (free Premium account) — that's future work.
// ---------------------------------------------------------
export const adminOrgChart = ref({
  id: 'admin-001',
  preferName: 'Lourens',
  surname: 'Van Der Bijl',
  role: 'CEO',
  status: 'active',
  reports: [
    {
      id: 'admin-002',
      preferName: 'Claude',
      surname: '',
      role: 'CTO',
      status: 'active',
      reports: [
        {
          id: 'admin-005',
          preferName: 'CoPilot',
          surname: '',
          role: 'Lead Developer',
          status: 'active',
          reports: [
            {
              id: 'admin-006',
              preferName: 'Ane',
              surname: 'Van Der Bijl',
              role: 'Senior Developer',
              status: 'active',
              reports: []
            }
          ]
        }
      ]
    },
    {
      id: 'admin-003',
      preferName: 'ChatGPT',
      surname: '',
      role: 'COO',
      status: 'active',
      reports: []
    },
    {
      id: 'admin-004',
      preferName: 'Jeandre',
      surname: 'Van Der Bijl',
      role: 'CFO',
      status: 'active',
      reports: []
    }
  ]
})

function findOrgNode(node, id) {
  if (node.id === id) return node
  for (const child of node.reports) {
    const found = findOrgNode(child, id)
    if (found) return found
  }
  return null
}

// Needed for removal — a node can't remove itself from its parent's
// reports array without knowing who the parent is.
function findOrgParent(node, id) {
  for (const child of node.reports) {
    if (child.id === id) return node
    const found = findOrgParent(child, id)
    if (found) return found
  }
  return null
}

let nextOrgId = 7
export function addOrgChartMember(parentId, member) {
  const parent = findOrgNode(adminOrgChart.value, parentId)
  if (!parent) return

  parent.reports.push({
    id: `admin-${String(nextOrgId++).padStart(3, '0')}`,
    preferName: member.preferName,
    surname: member.surname,
    role: member.role,
    status: 'active',
    reports: []
  })
}

// Toggles a local "terminated" flag — same local-only, no-backend
// spirit as Groups.vue's Suspend/Terminate menu items.
export function terminateOrgMember(id) {
  const node = findOrgNode(adminOrgChart.value, id)
  if (!node) return
  node.status = node.status === 'terminated' ? 'active' : 'terminated'
}

// Only removes leaf nodes — a node with its own direct reports needs
// those reassigned first, so the UI hides the remove button for it.
export function removeOrgMember(id) {
  const parent = findOrgParent(adminOrgChart.value, id)
  if (!parent) return
  parent.reports = parent.reports.filter((child) => child.id !== id)
}

// ---------------------------------------------------------
// Stats + Subscription donut — live data from the backend
// (AdminGroupView's /summary endpoint), except the ticket
// numbers below, which stay mock — there's no ticketing
// feature built on the backend yet.
// ---------------------------------------------------------
const groupSummary = ref(null)
const groupSummaryLoading = ref(false)
const groupSummaryError = ref(null)

// No backend for this yet — kept as mock until a Tickets feature exists.
const ticketStats = {
  openTickets: 4,
  pendingResponses: 2,
  closedTickets: 7
}

export const stats = computed(() => [
  { title: 'Total Groups', value: groupSummary.value?.totalGroups ?? '—', icon: 'groups' },
  { title: 'Total Users', value: groupSummary.value?.totalUsers ?? '—', icon: 'users' },
  { title: 'Open Tickets', value: ticketStats.openTickets, icon: 'tickets' },
  { title: 'Pending Responses', value: ticketStats.pendingResponses, icon: 'pending' },
  { title: 'Closed Tickets', value: ticketStats.closedTickets, icon: 'closed' }
])

export const subscriptionData = computed(() => {
  const premium = groupSummary.value?.premiumGroups ?? 0
  const free = groupSummary.value?.freeGroups ?? 0
  const total = groupSummary.value?.totalGroups ?? 0

  return {
    series: [premium, free],
    labels: ['Premium Groups', 'Free Groups'],
    total,
    colors: ['#0d045d', '#0a7633'],
    seriesPercent: total === 0
      ? [0, 0]
      : [(premium / total) * 100, (free / total) * 100]
  }
})

export async function loadGroupSummary() {
  groupSummaryLoading.value = true
  groupSummaryError.value = null

  try {
    const token = await getAdminToken()
    if (!token) {
      throw new Error('Not signed in.')
    }

    const response = await fetch(`${API_BASE}/api/admin/groups/summary`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const result = await response.json()

    if (!response.ok || !result.success) {
      throw new Error(result.message || 'Failed to fetch group summary')
    }

    groupSummary.value = result
  } catch (err) {
    console.error('Error fetching group summary:', err)
    groupSummaryError.value = err.message
  } finally {
    groupSummaryLoading.value = false
  }
}

// Initial load
loadGroupSummary()

export const activities = [
  { user: 'John Smith', action: 'created a new group', group: 'Smith Household', time: '2m ago' },
  { user: 'Emily Johnson', action: 'upgraded subscription', group: 'Johnson Family', time: '10m ago' },
  { user: 'Michael Brown', action: 'added a new user', group: 'Brown Enterprises', time: '25m ago' },
  { user: 'Sarah Lee', action: 'cancelled subscription', group: 'Lee Household', time: '1h ago' },
  { user: 'Admin', action: 'logged in', group: null, time: '2h ago' }
]

// ---------------------------------------------------------
// System Resources
// ---------------------------------------------------------
export const systemResources = [
  {
    name: 'NETLIFY (Hosting)',
    resources: [
      { name: 'Bandwidth', value: 2.4, max: 15, unit: 'GB' },
      { name: 'Web Requests', value: 128420, max: 10000000, unit: 'requests' },
      { name: 'Function Executions', value: 6, max: 500000, unit: 'executions' },
      { name: 'Build Minutes', value: 18, max: 1000, unit: 'minutes' }
    ]
  },
  {
    name: 'FIREBASE (Authentication)',
    resources: [
      { name: 'Daily Active Users', value: 428, max: 3000, unit: 'users' },
      { name: 'Monthly Active Users', value: 4821, max: 50000, unit: 'users' }
    ]
  },
  {
    name: 'FIREBASE (Firestore)',
    resources: [
      { name: 'Storage', value: 286, max: 1024, unit: 'MB' },
      { name: 'Reads (Today)', value: 12482, max: 50000, unit: 'reads' },
      { name: 'Writes (Today)', value: 4821, max: 20000, unit: 'writes' },
      { name: 'Deletes (Today)', value: 126, max: 20000, unit: 'deletes' },
      { name: 'Network Egress', value: 1.8, max: 10, unit: 'GB' }
    ]
  },
  {
    name: 'NEON (PostgreSQL)',
    resources: [
      { name: 'Storage', value: 400, max: 512, unit: 'MB' },
      { name: 'Compute (CU-Hours)', value: 17.4, max: 100, unit: 'CU' },
      { name: 'Peak Compute', value: 1.2, max: 2, unit: 'CU' },
      { name: 'Network Egress', value: 1.2, max: 5, unit: 'GB' }
    ]
  },
  {
    name: 'RAILWAY (C# Backend)',
    resources: [
      { name: 'CPU', value: 12, max: 100, unit: '%' },
      { name: 'Memory', value: 184, max: 512, unit: 'MB' },
      { name: 'Disk / Storage', value: 312, max: 1024, unit: 'MB' },
      { name: 'Network Transfer', value: 420, max: 5120, unit: 'MB' }
    ]
  }
]

// ---------------------------------------------------------
// Groups — live data from the backend (AdminGroupView)
// ---------------------------------------------------------
// Same shape as the old mock data below, so Groups.vue and
// GroupMembersDrawer.vue needed almost no changes:
// group.owner / group.members[].memberStatus etc. all line up.
//
// A couple of real-data caveats worth knowing:
// - groupStatus will currently only ever be "Active" — there's no
//   backend suspend/terminate path for a group yet, so the row menu's
//   "Suspend Group" / "Terminate Group" actions in Groups.vue still
//   only mutate local state, not the database.
// - subscription.paymentStatus will currently only ever be "Unpaid" —
//   real billing isn't built yet.
// - member.memberStatus ('pending' | 'active' | 'suspended') reflects
//   standing WITHIN the group, not overall account standing.
// ---------------------------------------------------------
export const groups = ref([])
export const groupsLoading = ref(false)
export const groupsError = ref(null)

// Same pattern loginAdmin.vue uses on login: ask the Firebase client
// SDK for the current user's ID token at the moment it's needed,
// rather than caching one ourselves.
//
// auth.currentUser is null for a brief moment on a fresh page load,
// while Firebase is still restoring the persisted session — before
// that restore finishes, any code that reads auth.currentUser directly
// sees "nobody's signed in" even when they are. Whichever page happens
// to import this module first is the one exposed to that race, so we
// wait for Firebase's restore to settle (via onAuthStateChanged) before
// concluding there's really no user. Only after that resolves do we
// treat a still-null user as an actual "not signed in".
function waitForAuthReady() {
  return new Promise((resolve) => {
    const unsubscribe = onAuthStateChanged(auth, (user) => {
      unsubscribe()
      resolve(user)
    })
  })
}

async function getAdminToken() {
  const currentUser = auth.currentUser ?? await waitForAuthReady()
  if (!currentUser) return null
  return await currentUser.getIdToken()
}

export async function loadGroups() {
  groupsLoading.value = true
  groupsError.value = null

  try {
    const token = await getAdminToken()
    if (!token) {
      throw new Error('Not signed in.')
    }

    const response = await fetch(`${API_BASE}/api/admin/groups/list`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ token })
    })

    const result = await response.json()

    if (!response.ok || !result.success) {
      throw new Error(result.message || 'Failed to fetch groups')
    }

    groups.value = result.groups
  } catch (err) {
    console.error('Error fetching groups:', err)
    groupsError.value = err.message
  } finally {
    groupsLoading.value = false
  }
}

// Initial load
loadGroups()

/* ---------------------------------------------------------
 * Old mock data, kept below for reference only — no longer used.
 * Delete once you've confirmed the real data renders correctly.
 * ---------------------------------------------------------
export const mockGroups = [
  {
    groupId: 'FB-000142',
    groupName: 'Van Der Bijl Household',

    // Group account/subscription
    accountType: 'Premium',
    groupStatus: 'Active',

    // Dates
    createdAt: '2026-07-18T09:32:00',
    lastActivity: '2026-08-16T14:42:00',

    // Group financial settings
    groupCurrency: 'ZAR',
    groupCountry: 'South Africa',
    groupTimezone: 'Africa/Johannesburg',

    // Subscription/payment information
    subscription: {
      status: 'Premium',
      membershipPaid: true,
      startDate: '2026-07-18',
      endDate: '2027-07-18',
      paymentStatus: 'Paid'
    },

    // Member summary
    memberCounts: {
      total: 5,
      active: 3,
      pending: 1,
      suspended: 1
    },

    // Main group owner
    owner: {
      userId: 'USR-000318',
      preferredName: 'Louis',
      firstName: 'Louis Lourens',
      lastName: 'Van Der Bijl',
      email: 'louis@example.com',
      signInMethod: 'Google',
      country: 'South Africa',
      currency: 'ZAR',
      timezone: 'Africa/Johannesburg'
    },

    // Members
    // IMPORTANT:
    // Main member first, followed by pending, active,
    // then suspended members.
    members: [
      {
        userId: 'USR-000318',
        preferredName: 'Louis',
        firstName: 'Louis Lourens',
        lastName: 'Van Der Bijl',
        email: 'louis@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: true,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-18T09:32:00',
        lastActivity: '2026-08-16T14:42:00'
      },

      {
        userId: 'USR-000319',
        preferredName: 'Sarah',
        firstName: 'Sarah',
        lastName: 'Van Der Bijl',
        email: 'sarah@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'pending',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: null,
        lastActivity: '2026-08-16T12:15:00'
      },

      {
        userId: 'USR-000320',
        preferredName: 'Daniel',
        firstName: 'Daniel',
        lastName: 'Van Der Bijl',
        email: 'daniel@example.com',
        signInMethod: 'Microsoft',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-19T10:20:00',
        lastActivity: '2026-08-15T18:20:00'
      },

      {
        userId: 'USR-000321',
        preferredName: 'Mia',
        firstName: 'Mia',
        lastName: 'Van Der Bijl',
        email: 'mia@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-20T13:45:00',
        lastActivity: '2026-08-14T09:10:00'
      },

      {
        userId: 'USR-000322',
        preferredName: 'James',
        firstName: 'James',
        lastName: 'Van Der Bijl',
        email: 'james@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'suspended',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-22T11:05:00',
        lastActivity: '2026-08-03T16:25:00'
      }
    ]
  },

  {
    groupId: 'FB-000141',
    groupName: 'Smith Household',

    accountType: 'Free',
    groupStatus: 'Active',

    createdAt: '2026-07-15T08:14:00',
    lastActivity: '2026-08-16T13:05:00',

    groupCurrency: 'ZAR',
    groupCountry: 'South Africa',
    groupTimezone: 'Africa/Johannesburg',

    subscription: {
      status: 'Free',
      membershipPaid: true,
      startDate: '2026-07-15',
      endDate: null,
      paymentStatus: 'Not Required'
    },

    memberCounts: {
      total: 4,
      active: 2,
      pending: 1,
      suspended: 1
    },

    owner: {
      userId: 'USR-000301',
      preferredName: 'John',
      firstName: 'John',
      lastName: 'Smith',
      email: 'john.smith@example.com',
      signInMethod: 'Email + Password',
      country: 'South Africa',
      currency: 'ZAR',
      timezone: 'Africa/Johannesburg'
    },

    members: [
      {
        userId: 'USR-000301',
        preferredName: 'John',
        firstName: 'John',
        lastName: 'Smith',
        email: 'john.smith@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'active',
        isOwner: true,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-15T08:14:00',
        lastActivity: '2026-08-16T13:05:00'
      },

      {
        userId: 'USR-000302',
        preferredName: 'Emily',
        firstName: 'Emily',
        lastName: 'Smith',
        email: 'emily.smith@example.com',
        signInMethod: 'Google',

        memberStatus: 'pending',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: null,
        lastActivity: '2026-08-16T11:40:00'
      },

      {
        userId: 'USR-000303',
        preferredName: 'Michael',
        firstName: 'Michael',
        lastName: 'Smith',
        email: 'michael.smith@example.com',
        signInMethod: 'Microsoft',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-17T09:25:00',
        lastActivity: '2026-08-15T17:35:00'
      },

      {
        userId: 'USR-000304',
        preferredName: 'James',
        firstName: 'James',
        lastName: 'Smith',
        email: 'james.smith@example.com',
        signInMethod: 'Yahoo',

        memberStatus: 'suspended',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-18T14:20:00',
        lastActivity: '2026-08-01T10:15:00'
      }
    ]
  },

  {
    groupId: 'FB-000140',
    groupName: 'Johnson Family',

    accountType: 'Premium',
    groupStatus: 'Active',

    createdAt: '2026-07-11T14:30:00',
    lastActivity: '2026-08-16T10:50:00',

    groupCurrency: 'ZAR',
    groupCountry: 'South Africa',
    groupTimezone: 'Africa/Johannesburg',

    subscription: {
      status: 'Premium',
      membershipPaid: true,
      startDate: '2026-07-11',
      endDate: '2027-07-11',
      paymentStatus: 'Paid'
    },

    memberCounts: {
      total: 7,
      active: 4,
      pending: 2,
      suspended: 1
    },

    owner: {
      userId: 'USR-000275',
      preferredName: 'Emily',
      firstName: 'Emily',
      lastName: 'Johnson',
      email: 'emily.johnson@example.com',
      signInMethod: 'Google',
      country: 'South Africa',
      currency: 'ZAR',
      timezone: 'Africa/Johannesburg'
    },

    members: [
      {
        userId: 'USR-000275',
        preferredName: 'Emily',
        firstName: 'Emily',
        lastName: 'Johnson',
        email: 'emily.johnson@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: true,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-11T14:30:00',
        lastActivity: '2026-08-16T10:50:00'
      },

      {
        userId: 'USR-000276',
        preferredName: 'Robert',
        firstName: 'Robert',
        lastName: 'Johnson',
        email: 'robert.johnson@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'pending',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: null,
        lastActivity: '2026-08-16T09:45:00'
      },

      {
        userId: 'USR-000277',
        preferredName: 'Jessica',
        firstName: 'Jessica',
        lastName: 'Johnson',
        email: 'jessica.johnson@example.com',
        signInMethod: 'Microsoft',

        memberStatus: 'pending',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: null,
        lastActivity: '2026-08-15T20:15:00'
      },

      {
        userId: 'USR-000278',
        preferredName: 'David',
        firstName: 'David',
        lastName: 'Johnson',
        email: 'david.johnson@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-13T11:25:00',
        lastActivity: '2026-08-15T15:42:00'
      },

      {
        userId: 'USR-000279',
        preferredName: 'Amanda',
        firstName: 'Amanda',
        lastName: 'Johnson',
        email: 'amanda.johnson@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-14T09:10:00',
        lastActivity: '2026-08-14T12:30:00'
      },

      {
        userId: 'USR-000280',
        preferredName: 'Thomas',
        firstName: 'Thomas',
        lastName: 'Johnson',
        email: 'thomas.johnson@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-15T16:45:00',
        lastActivity: '2026-08-13T18:20:00'
      },

      {
        userId: 'USR-000281',
        preferredName: 'William',
        firstName: 'William',
        lastName: 'Johnson',
        email: 'william.johnson@example.com',
        signInMethod: 'Yahoo',

        memberStatus: 'suspended',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-16T10:20:00',
        lastActivity: '2026-07-31T14:10:00'
      }
    ]
  },

  {
    groupId: 'FB-000139',
    groupName: 'Brown Enterprises',

    accountType: 'Premium',
    groupStatus: 'Active',

    createdAt: '2026-07-08T11:42:00',
    lastActivity: '2026-08-16T08:25:00',

    groupCurrency: 'ZAR',
    groupCountry: 'South Africa',
    groupTimezone: 'Africa/Johannesburg',

    subscription: {
      status: 'Premium',
      membershipPaid: true,
      startDate: '2026-07-08',
      endDate: '2027-07-08',
      paymentStatus: 'Paid'
    },

    memberCounts: {
      total: 3,
      active: 2,
      pending: 0,
      suspended: 1
    },

    owner: {
      userId: 'USR-000250',
      preferredName: 'Michael',
      firstName: 'Michael',
      lastName: 'Brown',
      email: 'michael.brown@example.com',
      signInMethod: 'Microsoft',
      country: 'South Africa',
      currency: 'ZAR',
      timezone: 'Africa/Johannesburg'
    },

    members: [
      {
        userId: 'USR-000250',
        preferredName: 'Michael',
        firstName: 'Michael',
        lastName: 'Brown',
        email: 'michael.brown@example.com',
        signInMethod: 'Microsoft',

        memberStatus: 'active',
        isOwner: true,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-08T11:42:00',
        lastActivity: '2026-08-16T08:25:00'
      },

      {
        userId: 'USR-000251',
        preferredName: 'Jessica',
        firstName: 'Jessica',
        lastName: 'Brown',
        email: 'jessica.brown@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-09T09:15:00',
        lastActivity: '2026-08-15T14:25:00'
      },

      {
        userId: 'USR-000252',
        preferredName: 'Matthew',
        firstName: 'Matthew',
        lastName: 'Brown',
        email: 'matthew.brown@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'suspended',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-07-10T12:30:00',
        lastActivity: '2026-07-29T16:50:00'
      }
    ]
  },

  {
    groupId: 'FB-000138',
    groupName: 'Lee Household',

    accountType: 'Free',
    groupStatus: 'Pending Termination',

    createdAt: '2026-06-28T15:10:00',
    lastActivity: '2026-08-15T16:45:00',

    groupCurrency: 'ZAR',
    groupCountry: 'South Africa',
    groupTimezone: 'Africa/Johannesburg',

    subscription: {
      status: 'Free',
      membershipPaid: true,
      startDate: '2026-06-28',
      endDate: '2026-08-30',
      paymentStatus: 'Not Required'
    },

    memberCounts: {
      total: 2,
      active: 2,
      pending: 0,
      suspended: 0
    },

    owner: {
      userId: 'USR-000220',
      preferredName: 'Sarah',
      firstName: 'Sarah',
      lastName: 'Lee',
      email: 'sarah.lee@example.com',
      signInMethod: 'Email + Password',
      country: 'South Africa',
      currency: 'ZAR',
      timezone: 'Africa/Johannesburg'
    },

    members: [
      {
        userId: 'USR-000220',
        preferredName: 'Sarah',
        firstName: 'Sarah',
        lastName: 'Lee',
        email: 'sarah.lee@example.com',
        signInMethod: 'Email + Password',

        memberStatus: 'active',
        isOwner: true,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-06-28T15:10:00',
        lastActivity: '2026-08-15T16:45:00'
      },

      {
        userId: 'USR-000221',
        preferredName: 'Mark',
        firstName: 'Mark',
        lastName: 'Lee',
        email: 'mark.lee@example.com',
        signInMethod: 'Google',

        memberStatus: 'active',
        isOwner: false,

        country: 'South Africa',
        currency: 'ZAR',
        timezone: 'Africa/Johannesburg',

        joinedAt: '2026-06-29T10:40:00',
        lastActivity: '2026-08-14T11:20:00'
      }
    ]
  }
]
*/

// ---------------------------------------------------------
// Platform Health — live data from the backend
// ---------------------------------------------------------
export const platformHealth = ref([])
export const platformHealthLastChecked = ref(null)

async function loadPlatformHealth() {
  try {
    const response = await fetch(`${API_BASE}/api/platform-health/status`)
    if (!response.ok) throw new Error('Failed to fetch platform health')
    platformHealth.value = await response.json()
    platformHealthLastChecked.value = new Date()
  } catch (err) {
    console.error('Error fetching platform health:', err)
  }
}

// Call this from the dashboard's refresh button — it tells the backend
// to run a fresh check right now, not just re-read the cached result.
export async function refreshPlatformHealth() {
  try {
    const response = await fetch(`${API_BASE}/api/platform-health/refresh`, {
      method: 'POST'
    })
    if (!response.ok) throw new Error('Failed to refresh platform health')
    platformHealth.value = await response.json()
    platformHealthLastChecked.value = new Date()
  } catch (err) {
    console.error('Error refreshing platform health:', err)
  }
}

// Initial load
loadPlatformHealth()

// The backend re-checks every 2 minutes on its own; we just re-read
// that cached result fairly often so the dashboard feels current.
setInterval(loadPlatformHealth, 30000)

// ---------------------------------------------------------
// Logs — live, newest first, only fetches what's new
// ---------------------------------------------------------
export const logs = ref([])

let lastSeenTimestamp = null

async function refreshLogs() {
  try {
    const url = lastSeenTimestamp
      ? `${API_BASE}/api/logs/live?since=${encodeURIComponent(lastSeenTimestamp)}`
      : `${API_BASE}/api/logs/live`

    const response = await fetch(url)
    if (!response.ok) throw new Error('Failed to fetch logs')

    const newLogs = await response.json()
    if (newLogs.length === 0) return

    const combined = [...newLogs, ...logs.value]
    combined.sort((a, b) => new Date(b.timestamp) - new Date(a.timestamp))
    logs.value = combined

    lastSeenTimestamp = combined[0].timestamp
  } catch (err) {
    console.error('Error fetching logs:', err)
  }
}

// Initial load — gets everything logged today so far
refreshLogs()

// From here on, each poll only asks for genuinely new logs
setInterval(refreshLogs, 10000)