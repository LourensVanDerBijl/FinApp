import { createRouter, createWebHistory } from 'vue-router'
import loginAdmin from '../devAdministrator/pages/loginAdmin.vue'
import AppLayout from '../devAdministrator/components/layout/AppLayout.vue'

// Admin pages
import Dashboard from '../devAdministrator/pages/Dashboard.vue'
import Groups from '../devAdministrator/pages/Groups.vue'
import Activity from '../devAdministrator/pages/Activity.vue'
import ApiControl from '../devAdministrator/pages/ApiControl.vue'
import Tickets from '../devAdministrator/pages/Tickets.vue'
import Development from '../devAdministrator/pages/Development.vue'
import Settings from '../devAdministrator/pages/Settings.vue'

import { auth } from '../firebase/firebaseManager.js'

const routes = [
  { path: '/', name: 'Home', component: loginAdmin },
  { path: '/admin/login', name: 'AdminLogin', component: loginAdmin },

  {
    path: '/admin',
    component: AppLayout,
    children: [
      { path: '', redirect: '/admin/dashboard' }, // ✅ redirect root /admin
      { path: 'dashboard', name: 'AdminDashboard', component: Dashboard },
      { path: 'groups', name: 'AdminGroups', component: Groups },
      { path: 'activity', name: 'AdminActivity', component: Activity },
      { path: 'api-control', name: 'AdminApiControl', component: ApiControl },
      { path: 'tickets', name: 'AdminTickets', component: Tickets },
      { path: 'development', name: 'AdminDevelopment', component: Development },
      { path: 'settings', name: 'AdminSettings', component: Settings }
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

router.beforeEach((to) => {
  if (to.path.startsWith('/admin') && to.name !== 'AdminLogin') {
    const user = auth.currentUser
    if (!user) {
      return '/admin/login'
    }
  }
})

export default router
