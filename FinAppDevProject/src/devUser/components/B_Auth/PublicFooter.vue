<script setup>
import { RouterLink } from 'vue-router'
</script>

<template>
  <footer class="public-footer">
    <div class="footer-container">
      <!-- Navigation Links -->
      <nav class="footer-links">
        <RouterLink to="/product">Product</RouterLink>
        <RouterLink to="/how-it-works">How It Works</RouterLink>
        <RouterLink to="/pricing">Pricing</RouterLink>
        <RouterLink to="/resources">Resources</RouterLink>
        <RouterLink to="/about">About Us</RouterLink>
        <RouterLink to="/security">Security</RouterLink>
        <RouterLink to="/privacy">Privacy</RouterLink>
        <RouterLink to="/terms">Terms</RouterLink>
      </nav>

      <!-- Branding -->
      <div class="footer-brand">
        <p>© {{ new Date().getFullYear() }} FinBine. All rights reserved.</p>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.public-footer {
  background-color: #04111f;
  color: #fff;
  padding: 20px;
  border-top: 1px solid rgba(255,255,255,0.1);
}

.footer-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 12px;
}

.footer-links {
  display: flex;
  flex-wrap: wrap;
  justify-content: center;
  gap: 16px;
}

.footer-links a {
  text-decoration: none;
  color: #f8fafc;
  font-size: 0.75rem;
  transition: color 0.2s;
}

.footer-links a:hover {
  color: #02b0a1;
}

.footer-brand {
  font-size: 0.7rem;
  color: #94a3b8;
  text-align: center;
}
</style>
