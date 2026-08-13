<script setup>
import logo from '../../../assets/SVG/logo.svg'
import { RouterLink, useRoute } from 'vue-router'
import { ref } from 'vue'

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
      </RouterLink>

      <!-- Desktop Navigation -->
      <nav class="nav-links">
        <RouterLink to="/" :class="{ active: route.name === 'Home' }">Home</RouterLink>
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
        <RouterLink to="/product" class="btn get-started">Get Started</RouterLink>
      </div>

      <!-- Mobile Hamburger -->
      <button class="hamburger" @click="toggleMobileMenu">
        <span :class="{ open: mobileOpen }"></span>
        <span :class="{ open: mobileOpen }"></span>
        <span :class="{ open: mobileOpen }"></span>
      </button>
    </div>

    <!-- Mobile Menu -->
    <div v-if="mobileOpen" class="mobile-menu">
      <RouterLink to="/" @click="closeMobileMenu">Home</RouterLink>
      <RouterLink to="/product" @click="closeMobileMenu">Product</RouterLink>
      <RouterLink to="/how-it-works" @click="closeMobileMenu">How It Works</RouterLink>
      <RouterLink to="/security" @click="closeMobileMenu">Security</RouterLink>
      <RouterLink to="/pricing" @click="closeMobileMenu">Pricing</RouterLink>
      <RouterLink to="/resources" @click="closeMobileMenu">Resources</RouterLink>
      <RouterLink to="/about" @click="closeMobileMenu">About Us</RouterLink>
      <RouterLink to="/user/login" @click="closeMobileMenu" class="btn sign-in">Sign In</RouterLink>
      <RouterLink to="/product" @click="closeMobileMenu" class="btn get-started">Get Started</RouterLink>
    </div>
  </header>
</template>

<style scoped>
.public-navbar {
  background-color: #04111f;
  color: #fff;
  border-bottom: 1px solid rgba(255,255,255,0.1);
  position: sticky;
  top: 0;
  z-index: 50;
}

.navbar-container {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 12px 20px;
}

.logo {
  height: 32px;
  filter: brightness(0) saturate(100%) invert(29%) sepia(98%) saturate(748%) hue-rotate(180deg) brightness(95%) contrast(90%);
}

.nav-links {
  display: flex;
  gap: 16px;
}

.nav-links a {
  text-decoration: none;
  color: #f8fafc;
  font-size: 0.8rem;
  transition: color 0.2s;
}

.nav-links a.active {
  color: #02b0a1;
}

.cta-buttons {
  display: flex;
  gap: 10px;
}

.btn {
  padding: 6px 12px;
  border-radius: 4px;
  font-size: 0.75rem;
  text-decoration: none;
  transition: background 0.2s;
}

.sign-in {
  background: none;
  border: 1px solid #02b0a1;
  color: #02b0a1;
}

.sign-in:hover {
  background: #02b0a1;
  color: #fff;
}

.get-started {
  background: #02b0a1;
  color: #fff;
}

.get-started:hover {
  background: #028c81;
}

.hamburger {
  display: none;
  flex-direction: column;
  gap: 4px;
  background: none;
  border: none;
  cursor: pointer;
}

.hamburger span {
  width: 20px;
  height: 2px;
  background: #fff;
  transition: all 0.3s;
}

.hamburger span.open:nth-child(1) {
  transform: rotate(45deg) translateY(6px);
}
.hamburger span.open:nth-child(2) {
  opacity: 0;
}
.hamburger span.open:nth-child(3) {
  transform: rotate(-45deg) translateY(-6px);
}

.mobile-menu {
  display: flex;
  flex-direction: column;
  background-color: #04111f;
  padding: 12px;
  gap: 10px;
}

.mobile-menu a {
  text-decoration: none;
  color: #f8fafc;
  font-size: 0.85rem;
}

@media (max-width: 768px) {
  .nav-links, .cta-buttons {
    display: none;
  }
  .hamburger {
    display: flex;
  }
}
</style>
