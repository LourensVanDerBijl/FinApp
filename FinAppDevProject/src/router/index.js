import { createRouter, createWebHistory } from 'vue-router'
import loginAdmin from '../devAdministrator/pages/loginAdmin.vue'

const routes = [
  {
    path: '/',
    name: 'Home',
    component: loginAdmin   // default route shows Admin Login
  },
  {
    path: '/admin/login',
    name: 'AdminLogin',
    component: loginAdmin
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
