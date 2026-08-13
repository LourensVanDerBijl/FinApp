// src/devUser/composables/B_Auth/usePageScrollNavigation.js
import { onMounted, onUnmounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'

// Define the navigation order
const routeOrder = [
  { path: '/', name: 'Home' },
  { path: '/product', name: 'Product' },
  { path: '/how-it-works', name: 'HowItWorks' },
  { path: '/security', name: 'Security' },
  { path: '/pricing', name: 'Pricing' },
  { path: '/resources', name: 'Resources' },
  { path: '/about', name: 'About' }
]

export function usePageScrollNavigation() {
  const router = useRouter()
  const route = useRoute()

  let lastScrollTop = 0
  let atBottom = false
  let scrollTimeout = null

  function handleScroll(event) {
    const { scrollTop, scrollHeight, clientHeight } = document.documentElement
    const currentScrollTop = scrollTop

    // Detect if user is at bottom
    if (scrollTop + clientHeight >= scrollHeight - 2) {
      atBottom = true
    } else {
      atBottom = false
    }

    // Detect deliberate downward scroll after reaching bottom
    if (atBottom && currentScrollTop > lastScrollTop) {
      clearTimeout(scrollTimeout)
      scrollTimeout = setTimeout(() => {
        navigateNext()
      }, 250) // small delay to prevent accidental triggers
    }

    lastScrollTop = currentScrollTop
  }

  function navigateNext() {
    const currentIndex = routeOrder.findIndex(r => r.path === route.path)
    if (currentIndex !== -1 && currentIndex < routeOrder.length - 1) {
      const nextRoute = routeOrder[currentIndex + 1]
      router.push(nextRoute.path).then(() => {
        window.scrollTo({ top: 0, behavior: 'smooth' })
      })
    }
  }

  onMounted(() => {
    window.addEventListener('scroll', handleScroll, { passive: true })
  })

  onUnmounted(() => {
    window.removeEventListener('scroll', handleScroll)
    clearTimeout(scrollTimeout)
  })
}
