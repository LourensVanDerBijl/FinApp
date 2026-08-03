// src/main.js
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import { auth } from './firebase/firebaseManager.js'
import { onAuthStateChanged } from 'firebase/auth'

// Import ApexCharts plugin
import VueApexCharts from 'vue3-apexcharts'

const app = createApp(App)

let appInitialized = false

onAuthStateChanged(auth, (user) => {
  if (!appInitialized) {
    // Register router and ApexCharts plugin
    app.use(router)
    app.use(VueApexCharts)

    app.mount('#app')
    appInitialized = true
  }

  if (!user) {
    // If user is logged out, force them to login page
    router.push('/admin/login')
  }
})
