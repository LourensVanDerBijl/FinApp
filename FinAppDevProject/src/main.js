// src/main.js
import { createApp } from 'vue'
import './style.css'
import App from './App.vue'
import router from './router'
import { auth } from './firebase/firebaseManager.js'
import { onAuthStateChanged } from 'firebase/auth'

const app = createApp(App)

let appInitialized = false

onAuthStateChanged(auth, (user) => {
  if (!appInitialized) {
    app.use(router)
    app.mount('#app')
    appInitialized = true
  }

  if (!user) {
    // If user is logged out, force them to login page
    router.push('/admin/login')
  }
})
