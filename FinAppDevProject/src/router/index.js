import { createRouter, createWebHistory } from 'vue-router'
import AppLayout from '../devAdministrator/components/layout/AppLayout.vue'

// Registration pages
import UserRegistration from '../devUser/pages/Register/UserRegistration.vue'

// User pages
import WebHome from '../devUser/pages/B_Auth/WebHome.vue'
import LoginUser from '../devUser/pages/A_Auth/LoginUser.vue'

// User Gaurded pages
import UserGroupAssign from '../devUser/pages/A_Auth/UserGroupAssign.vue'
import UserGroupRegistration from '../devUser/pages/A_Auth/UserGroupRegistration.vue'
import UserGroupDashboard from '../devUser/pages/A_Auth/UserGroupDashboard.vue'

// Public marketing pages
import Product from '../devUser/pages/B_Auth/Product.vue'
import HowItWorks from '../devUser/pages/B_Auth/HowItWorks.vue'
import Security from '../devUser/pages/B_Auth/Security.vue'
import Pricing from '../devUser/pages/B_Auth/Pricing.vue'
import Resources from '../devUser/pages/B_Auth/Resources.vue'
import About from '../devUser/pages/B_Auth/About.vue'
import Privacy from '../devUser/pages/B_Auth/Privacy.vue'
import Terms from '../devUser/pages/B_Auth/Terms.vue'


// Admin pages
import loginAdmin from '../devAdministrator/pages/loginAdmin.vue'
import Dashboard from '../devAdministrator/pages/Dashboard.vue'
import Groups from '../devAdministrator/pages/Groups.vue'
import Activity from '../devAdministrator/pages/Activity.vue'
import ApiControl from '../devAdministrator/pages/ApiControl.vue'
import Tickets from '../devAdministrator/pages/Tickets.vue'
import Development from '../devAdministrator/pages/Development.vue'
import Settings from '../devAdministrator/pages/Settings.vue'

import { auth } from '../firebase/firebaseManager.js'
import { loadCurrentUserProfile } from '../devUser/data/userSession.js'

const routes = [
  // Public marketing site (no guard)
  { path: '/', name: 'Home', component: WebHome },
  { path: '/product', name: 'Product', component: Product },
  { path: '/how-it-works', name: 'HowItWorks', component: HowItWorks },
  { path: '/security', name: 'Security', component: Security },
  { path: '/pricing', name: 'Pricing', component: Pricing },
  { path: '/resources', name: 'Resources', component: Resources },
  { path: '/about', name: 'About', component: About },
  { path: '/privacy', name: 'Privacy', component: Privacy },
  { path: '/terms', name: 'Terms', component: Terms },

  // User routes (no guard)
  { path: '/user/login', name: 'UserLogin', component: LoginUser },

  // User Routes (gaurded routes restricted to user login access)
  {
    path: '/user/group-assign',
    name: 'UserGroupAssign',
    component: UserGroupAssign,
    meta: { requiresUserAuth: true }
  },
  {
    path: '/user/group-registration',
    name: 'UserGroupRegistration',
    component: UserGroupRegistration,
    meta: { requiresUserAuth: true }
  },
  {
    path: '/user/dashboard',
    name: 'UserGroupDashboard',
    component: UserGroupDashboard,
    meta: { requiresUserAuth: true }
  },

  // User registration (no guard)
  { path: '/user/register', name: 'UserRegister', component: UserRegistration },

  // Admin login (always accessible directly)
  { path: '/admin/login', name: 'AdminLogin', component: loginAdmin },

  // Admin dashboard and protected routes
  {
    path: '/admin',
    component: AppLayout,
    children: [
      { path: '', redirect: '/admin/dashboard' },
      { path: 'dashboard', name: 'AdminDashboard', component: Dashboard },
      { path: 'groups', name: 'AdminGroups', component: Groups },
      { path: 'activity', name: 'AdminActivity', component: Activity },
      { path: 'api-control', name: 'AdminApiControl', component: ApiControl },
      { path: 'tickets', name: 'AdminTickets', component: Tickets },
      { path: 'development', name: 'AdminDevelopment', component: Development },
      { path: 'settings', name: 'AdminSettings', component: Settings }
    ]
  },

  // Catch-all route → send unknown URLs to Home
  { path: '/:pathMatch(.*)*', name: 'NotFound', component: WebHome }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

// ✅ Guard applies to /admin routes (except /admin/login) AND any route
// tagged meta.requiresUserAuth (see UserGroupAssign/UserGroupDashboard above)
router.beforeEach(async (to) => {
  // --- Admin ---
  if (to.path.startsWith('/admin') && to.name !== 'AdminLogin') {
    const user = auth.currentUser
    if (!user) {
      return '/admin/login'
    }
  }

  // --- User-facing protected pages ---
  if (to.meta.requiresUserAuth) {
    const isValid = await loadCurrentUserProfile()
    if (!isValid) {
      return '/user/login'
    }
  }
})

export default router