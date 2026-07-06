import { createRouter, createWebHistory } from 'vue-router'
import loginAdmin from '../devAdministrator/pages/loginAdmin.vue'
import dashboardAdmin from '../devAdministrator/pages/dashboardAdmin.vue'
import { auth } from '../firebase/firebaseManager.js'

const routes = [
  { path: '/', name: 'Home', component: loginAdmin },
  { path: '/admin/login', name: 'AdminLogin', component: loginAdmin },
  { path: '/admin/dashboard', name: 'AdminDashboard', component: dashboardAdmin }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// ✅ Modern guard style: return instead of next()
router.beforeEach((to) => {
  if (to.name === 'AdminDashboard') {
    const user = auth.currentUser
    if (!user) {
      return '/admin/login'   // redirect if not logged in
    }
  }
  // returning nothing means allow navigation
})

export default router
