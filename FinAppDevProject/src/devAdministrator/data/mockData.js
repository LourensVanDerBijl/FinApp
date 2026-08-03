// src/devAdministrator/data/mockData.js

export const stats = [
  { title: 'Total Groups', value: 142, icon: 'LayoutDashboard' },
  { title: 'Total Users', value: 318, icon: 'Users' },
  { title: 'Open Tickets', value: 4, icon: 'Ticket' },
  { title: 'Pending Responses', value: 2, icon: 'Activity' },
  { title: 'Closed Tickets', value: 7, icon: 'Shield' }
]

export const subscriptionData = {
  series: [38, 104], // Premium, Free
  labels: ['Premium Groups', 'Free Groups']
}

export const activities = [
  'Group created',
  'User registered',
  'Group upgraded',
  'Subscription cancelled',
  'Group suspended',
  'Admin login',
  'Ticket opened',
  'Ticket closed',
  'API key generated',
  'Background job completed'
]

export const platformHealth = [
  { name: 'Firebase', status: 'Connected' },
  { name: 'Authentication', status: 'Running' },
  { name: 'SQL Database', status: 'Online' },
  { name: 'API Backend', status: 'Online' },
  { name: 'Background Jobs', status: 'Running' },
  { name: 'Frontend Hosting', status: 'Connected' }
]

export const systemOverview = [
  { name: 'SQL Database Storage', value: 38, max: 100 },
  { name: 'API CPU Usage', value: 22, max: 100 },
  { name: 'API RAM Usage', value: 41, max: 100 },
  { name: 'Frontend Storage', value: 57, max: 100 },
  { name: 'Bandwidth Usage', value: 132.4, max: 500 },
  { name: 'API Requests', value: 245812, max: 500000 }
]

export const logs = [
  { time: '03 Aug 2026 10:23:45', type: 'API Error', source: 'User Service', message: 'Failed to update user profile: Validation error', level: 'Error' },
  { time: '03 Aug 2026 10:21:16', type: 'System', source: 'Background Jobs', message: 'Group usage statistics job completed successfully', level: 'Info' },
  { time: '03 Aug 2026 10:12:08', type: 'Auth', source: 'Authentication Service', message: 'User login successful: louis@finbine.com', level: 'Info' },
  { time: '03 Aug 2026 10:12:08', type: 'API Error', source: 'Payment Service', message: 'Payment gateway timeout', level: 'Warning' },
  { time: '03 Aug 2026 09:55:32', type: 'System', source: 'Frontend Hosting', message: 'Deployment completed successfully', level: 'Success' },
  { time: '03 Aug 2026 09:40:12', type: 'Database', source: 'SQL Database', message: 'Backup completed', level: 'Info' },
  { time: '03 Aug 2026 09:30:00', type: 'API', source: 'Backend', message: 'New API key issued', level: 'Info' },
  { time: '03 Aug 2026 09:15:45', type: 'Auth', source: 'Authentication Service', message: 'Password reset requested', level: 'Warning' },
  { time: '03 Aug 2026 09:05:20', type: 'System', source: 'Background Jobs', message: 'Cleanup job executed', level: 'Info' },
  { time: '03 Aug 2026 08:50:00', type: 'API Error', source: 'Payment Service', message: 'Transaction declined', level: 'Error' }
]
