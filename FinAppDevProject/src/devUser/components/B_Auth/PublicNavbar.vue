<script setup>
import logo from '../../../assets/SVG/logo.svg'
import { RouterLink, useRoute } from 'vue-router'
import { ref } from 'vue'
import { ChevronDown, Menu, X } from 'lucide-vue-next'

const route = useRoute()
const mobileOpen = ref(false)

function toggleMobileMenu() {
  mobileOpen.value = !mobileOpen.value
}

function closeMobileMenu() {
  mobileOpen.value = false
}
</script>

<template>
  <header class="public-navbar">
    <div class="navbar-container">
      <!-- Logo -->
      <RouterLink to="/" class="logo-link" @click="closeMobileMenu">
        <img :src="logo" alt="FinBine Logo" class="logo" />
        <span class="wordmark">FinBine</span>
      </RouterLink>

      <!-- Desktop Navigation -->
      <nav class="nav-links">
        <RouterLink to="/product" :class="{ active: route.name === 'Product' }">Product</RouterLink>
        <RouterLink to="/how-it-works" :class="{ active: route.name === 'HowItWorks' }">How It Works</RouterLink>
        <RouterLink to="/security" :class="{ active: route.name === 'Security' }">Security</RouterLink>
        <RouterLink to="/pricing" :class="{ active: route.name === 'Pricing' }">Pricing</RouterLink>
        <RouterLink to="/resources" :class="{ active: route.name === 'Resources' }">Resources</RouterLink>
        <RouterLink to="/about" :class="{ active: route.name === 'About' }">About Us</RouterLink>
      </nav>

      <!-- CTA Buttons -->
      <div class="cta-buttons">
        <RouterLink to="/user/login" class="btn sign-in">Sign In</RouterLink>
        <RouterLink to="/user/register" class="btn get-started">Get Started</RouterLink>
      </div>

      <!-- Mobile Hamburger -->
      <button class="hamburger" @click="toggleMobileMenu" aria-label="Open menu">
        <Menu size="22" />
      </button>
    </div>

    <!-- Mobile Menu -->
    <div v-if="mobileOpen" class="mobile-menu">
      <div class="mobile-menu-header">
        <RouterLink to="/" class="logo-link" @click="closeMobileMenu">
          <img :src="logo" alt="FinBine Logo" class="logo" />
          <span class="wordmark">FinBine</span>
        </RouterLink>
        <button class="close-btn" @click="closeMobileMenu" aria-label="Close menu">
          <X size="20" />
        </button>
      </div>

      <nav class="mobile-nav-links">
        <RouterLink to="/product" @click="closeMobileMenu">Product</RouterLink>
        <RouterLink to="/how-it-works" @click="closeMobileMenu">How It Works</RouterLink>
        <RouterLink to="/security" @click="closeMobileMenu">Security</RouterLink>
        <RouterLink to="/pricing" @click="closeMobileMenu">Pricing</RouterLink>
        <RouterLink to="/resources" @click="closeMobileMenu">Resources</RouterLink>
        <RouterLink to="/about" @click="closeMobileMenu">About Us</RouterLink>
      </nav>

      <div class="mobile-menu-footer">
        <RouterLink to="/product" @click="closeMobileMenu" class="btn get-started full">Get Started for Free</RouterLink>
        <RouterLink to="/user/login" @click="closeMobileMenu" class="btn sign-in full">Sign In</RouterLink>
      </div>
    </div>
  </header>
</template>

<style scoped>
.public-navbar {
  background-color: #FFFFFF;
  color: #0F172A;
  border-bottom: 1px solid #E5E7EB;
  position: sticky;
  top: 0;
  z-index: 50;
}

.navbar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 28px;
  max-width: 1280px;
  margin: 0 auto;
}

.logo-link {
  display: flex;
  align-items: center;
  gap: 8px;
  text-decoration: none;
}

.logo {
  height: 28px;
  filter: brightness(0) saturate(100%) invert(29%) sepia(98%) saturate(748%) hue-rotate(180deg) brightness(95%) contrast(90%);
}

.wordmark {
  font-size: 1rem;
  font-weight: 700;
  color: #0F172A;
}

.nav-links {
  display: flex;
  align-items: center;
  gap: 30px;
}

.nav-links a {
  display: flex;
  align-items: center;
  gap: 3px;
  text-decoration: none;
  color: #334155;
  font-size: 0.82rem;
  font-weight: 600;
  transition: color 0.15s;
}

.nav-links a:hover {
  color: #1855b9;
}

.nav-links a.active {
  color: #0F172A;
  background-color: #F1F5F9;
  border-radius: 16px;
  padding: 5px 14px;
}

.cta-buttons {
  display: flex;
  align-items: center;
  gap: 14px;
}

.btn {
  padding: 8px 16px;
  border-radius: 6px;
  font-size: 0.8rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.15s, color 0.15s, border-color 0.15s;
}

.sign-in {
  background: none;
  border: none;
  color: #0F172A;
  padding: 8px 4px;
}

.sign-in:hover {
  color: #1855b9;
}

.get-started {
  background: #1855b9;
  color: #fff;
}

.get-started:hover {
  background: #123f8a;
}

.hamburger {
  display: none;
  align-items: center;
  justify-content: center;
  background: none;
  border: none;
  color: #0F172A;
  cursor: pointer;
  padding: 4px;
}

/* Mobile drawer */
.mobile-menu {
  position: fixed;
  inset: 0;
  background: #FFFFFF;
  z-index: 100;
  display: flex;
  flex-direction: column;
  padding: 14px 20px 24px;
}

.mobile-menu-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding-bottom: 16px;
  border-bottom: 1px solid #E5E7EB;
}

.close-btn {
  background: none;
  border: none;
  color: #0F172A;
  cursor: pointer;
  padding: 4px;
}

.mobile-nav-links {
  display: flex;
  flex-direction: column;
  margin-top: 8px;
}

.mobile-nav-links a {
  text-decoration: none;
  color: #1E293B;
  font-size: 0.95rem;
  font-weight: 600;
  padding: 14px 4px;
  border-bottom: 1px solid #F1F5F9;
}

.mobile-menu-footer {
  margin-top: auto;
  display: flex;
  flex-direction: column;
  gap: 10px;
  padding-top: 16px;
}

.btn.full {
  width: 100%;
  text-align: center;
  box-sizing: border-box;
  padding: 12px 16px;
}

.mobile-menu-footer .sign-in {
  border: 1px solid #CBD5E1;
  color: #0F172A;
}

@media (max-width: 900px) {
  .nav-links,
  .cta-buttons {
    display: none;
  }
  .hamburger {
    display: flex;
  }
  .wordmark {
    display: none;
  }
}
</style>