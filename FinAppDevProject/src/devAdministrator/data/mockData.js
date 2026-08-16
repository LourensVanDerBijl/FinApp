// src/devAdministrator/data/mockData.js

import { ref } from 'vue'

const API_BASE = 'https://localhost:5001'

//Side Navigation mock data
export const adminInfo = {
  name: "Louis Van Der Bijl",
  role: "Super Admin"
}

export const stats = [
  { title: 'Total Groups', value: 142, icon: 'groups' },
  { title: 'Total Users', value: 318, icon: 'users' },
  { title: 'Open Tickets', value: 4, icon: 'tickets' },
  { title: 'Pending Responses', value: 2, icon: 'pending' },
  { title: 'Closed Tickets', value: 7, icon: 'closed' }
]

export const subscriptionData = {
  series: [38, 104],
  labels: ['Premium Groups', 'Free Groups'],
  total: 142,
  colors: ['#0d045d', '#0a7633'],
  seriesPercent: [27, 73]
}

export const activities = [
  { user: 'John Smith', action: 'created a new group', group: 'Smith Household', time: '2m ago' },
  { user: 'Emily Johnson', action: 'upgraded subscription', group: 'Johnson Family', time: '10m ago' },
  { user: 'Michael Brown', action: 'added a new user', group: 'Brown Enterprises', time: '25m ago' },
  { user: 'Sarah Lee', action: 'cancelled subscription', group: 'Lee Household', time: '1h ago' },
  { user: 'Admin', action: 'logged in', group: null, time: '2h ago' }
]

// ✅ New grouped System Resources structure
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