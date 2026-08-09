// src/devAdministrator/data/mockData.js

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
  colors: ['#0d045d', '#0a7633'],       // slice colors
  seriesPercent: [27, 73]               // precomputed percentages
}

export const activities = [
  { user: 'John Smith', action: 'created a new group', group: 'Smith Household', time: '2m ago' },
  { user: 'Emily Johnson', action: 'upgraded subscription', group: 'Johnson Family', time: '10m ago' },
  { user: 'Michael Brown', action: 'added a new user', group: 'Brown Enterprises', time: '25m ago' },
  { user: 'Sarah Lee', action: 'cancelled subscription', group: 'Lee Household', time: '1h ago' },
  { user: 'Admin', action: 'logged in', group: null, time: '2h ago' }
]

// ✅ Updated Platform Health mock data structure
export const platformHealth = [
  { name: 'Firebase', status: 'Offline', responseTime: 42, errors: 0 },
  { name: 'PostgreSQL', status: 'Healthy', responseTime: 18, errors: 1 },
  { name: 'C# API', status: 'Critical', responseTime: 143, errors: 2 },
  { name: 'Netlify', status: 'Healthy', responseTime: null, errors: 0 }
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

export const logs = [
  { time: '03 Aug 2026 10:23:45', type: 'API Error', source: 'User Service', message: 'Failed to update user profile: Validation error', level: 'Error' },
  { time: '03 Aug 2026 10:21:16', type: 'System', source: 'Background Jobs', message: 'Group usage statistics job completed successfully, Group usage statistics job completed successfully, Group usage statistics job completed successfully, Group usage statistics job completed successfully Group usage statistics job completed successfully, Group usage statistics job completed successfully, Group usage statistics job completed successfully, Group usage statistics job completed successfully', level: 'Info' },
  { time: '03 Aug 2026 10:12:08', type: 'Auth', source: 'Authentication Service', message: 'User login successful: louis@finbine.com', level: 'Info' },
  { time: '03 Aug 2026 10:12:08', type: 'API Error', source: 'Payment Service', message: 'Payment gateway timeout', level: 'Warning' },
  { time: '03 Aug 2026 09:55:32', type: 'System', source: 'Frontend Hosting', message: 'Deployment completed successfully', level: 'Success' },
  { time: '03 Aug 2026 09:40:12', type: 'Database', source: 'SQL Database', message: 'Backup completed', level: 'Info' },
  { time: '03 Aug 2026 09:30:00', type: 'API', source: 'Backend', message: 'New API key issued', level: 'Info' },
  { time: '03 Aug 2026 09:15:45', type: 'Auth', source: 'Authentication Service', message: 'Password reset requested', level: 'Warning' },
  { time: '03 Aug 2026 09:05:20', type: 'System', source: 'Background Jobs', message: 'Cleanup job executed', level: 'Info' },
  { time: '03 Aug 2026 08:50:00', type: 'API Error', source: 'Payment Service', message: 'Transaction declined', level: 'Error' }
]
