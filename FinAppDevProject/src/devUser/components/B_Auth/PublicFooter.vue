<script setup>
import { RouterLink } from 'vue-router'
import logo from '../../../assets/SVG/logo.svg'
import { Linkedin, Twitter, Facebook, Instagram } from 'lucide-vue-next'
import { ref } from 'vue'

const email = ref('')
const subscribed = ref(false)

// No newsletter backend exists yet — this just confirms the input
// visually. Wire this up to a real endpoint once one exists.
function handleSubscribe() {
  if (!email.value) return
  subscribed.value = true
}
</script>

<template>
  <footer class="public-footer">
    <div class="footer-top">
      <div class="footer-brand">
        <div class="brand-row">
          <img :src="logo" alt="FinBine Logo" class="footer-logo" />
          <span class="wordmark">FinBine</span>
        </div>
        <p>Built for groups.<br />Designed for clarity.<br />Secured for your peace of mind.</p>
        <div class="socials">
          <a href="#" aria-label="LinkedIn"><Linkedin size="16" /></a>
          <a href="#" aria-label="Twitter"><Twitter size="16" /></a>
          <a href="#" aria-label="Facebook"><Facebook size="16" /></a>
          <a href="#" aria-label="Instagram"><Instagram size="16" /></a>
        </div>
      </div>

      <div class="footer-col">
        <h4>Product</h4>
        <RouterLink to="/product">Features</RouterLink>
        <RouterLink to="/how-it-works">How It Works</RouterLink>
        <RouterLink to="/pricing">Pricing</RouterLink>
      </div>

      <div class="footer-col">
        <h4>Resources</h4>
        <RouterLink to="/resources">Help Center</RouterLink>
        <RouterLink to="/resources">Blog</RouterLink>
        <RouterLink to="/resources">Documentation</RouterLink>
      </div>

      <div class="footer-col">
        <h4>Company</h4>
        <RouterLink to="/about">About Us</RouterLink>
        <RouterLink to="/about">Careers</RouterLink>
        <RouterLink to="/about">Contact Us</RouterLink>
      </div>

      <div class="footer-col">
        <h4>Legal</h4>
        <RouterLink to="/privacy">Privacy Policy</RouterLink>
        <RouterLink to="/terms">Terms of Service</RouterLink>
        <RouterLink to="/security">Security</RouterLink>
      </div>

      <div class="footer-col newsletter">
        <h4>Stay up to date</h4>
        <p>Get product updates and financial insights.</p>
        <form v-if="!subscribed" class="subscribe-form" @submit.prevent="handleSubscribe">
          <input v-model="email" type="email" placeholder="Enter your email" required />
          <button type="submit">Subscribe</button>
        </form>
        <p v-else class="subscribed-msg">You're on the list.</p>
      </div>
    </div>

    <div class="footer-bottom">
      <p>© {{ new Date().getFullYear() }} FinBine. All rights reserved.</p>
    </div>
  </footer>
</template>

<style scoped>
.public-footer {
  background-color: #04111f;
  color: #fff;
  padding: 40px 40px 20px;
}

.footer-top {
  max-width: 1280px;
  margin: 0 auto;
  display: grid;
  grid-template-columns: 1.6fr repeat(4, 1fr) 1.4fr;
  gap: 24px;
  padding-bottom: 32px;
  border-bottom: 1px solid rgba(255,255,255,0.1);
}

.brand-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 10px;
}

.footer-logo {
  height: 24px;
  filter: brightness(0) saturate(100%) invert(60%) sepia(90%) saturate(1000%) hue-rotate(190deg);
}

.wordmark {
  font-weight: 700;
  font-size: 1rem;
}

.footer-brand p {
  font-size: 0.78rem;
  color: #94A3B8;
  line-height: 1.5;
  margin-bottom: 16px;
}

.socials {
  display: flex;
  gap: 10px;
}

.socials a {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: rgba(255,255,255,0.08);
  color: #CBD5E1;
  display: flex;
  align-items: center;
  justify-content: center;
  text-decoration: none;
  transition: background 0.15s, color 0.15s;
}

.socials a:hover {
  background: #1855b9;
  color: #fff;
}

.footer-col h4 {
  font-size: 0.78rem;
  font-weight: 700;
  color: #fff;
  margin-bottom: 14px;
}

.footer-col a {
  display: block;
  text-decoration: none;
  color: #94A3B8;
  font-size: 0.78rem;
  margin-bottom: 10px;
  transition: color 0.15s;
}

.footer-col a:hover {
  color: #60a5fa;
}

.newsletter p {
  font-size: 0.75rem;
  color: #94A3B8;
  margin-bottom: 12px;
  line-height: 1.5;
}

.subscribe-form {
  display: flex;
  gap: 6px;
}

.subscribe-form input {
  flex: 1;
  min-width: 0;
  padding: 8px 10px;
  border-radius: 5px;
  border: 1px solid rgba(255,255,255,0.15);
  background: rgba(255,255,255,0.05);
  color: #fff;
  font-size: 0.75rem;
}

.subscribe-form input::placeholder {
  color: #64748B;
}

.subscribe-form button {
  background: #1855b9;
  color: #fff;
  border: none;
  border-radius: 5px;
  padding: 8px 14px;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  white-space: nowrap;
}

.subscribe-form button:hover {
  background: #123f8a;
}

.subscribed-msg {
  color: #4ade80 !important;
  font-weight: 600;
}

.footer-bottom {
  max-width: 1280px;
  margin: 0 auto;
  padding-top: 18px;
  text-align: center;
}

.footer-bottom p {
  font-size: 0.72rem;
  color: #64748B;
}

@media (max-width: 900px) {
  .footer-top {
    grid-template-columns: 1fr 1fr;
    row-gap: 32px;
  }
  .footer-brand {
    grid-column: 1 / -1;
  }
}

@media (max-width: 560px) {
  .footer-top {
    grid-template-columns: 1fr;
  }
}
</style>