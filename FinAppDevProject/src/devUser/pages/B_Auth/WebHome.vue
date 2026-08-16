<script setup>
import PublicNavbar from '../../components/B_Auth/PublicNavbar.vue'
import PublicFooter from '../../components/B_Auth/PublicFooter.vue'
import { RouterLink } from 'vue-router'
import { ShieldCheck, Users, Lock, BarChart3, FileText, Repeat, ChevronRight } from 'lucide-vue-next'

import WebHomeMainImg from '../../../assets/WebHomeMainImg.png'

const features = [
  { icon: Users, title: 'Manage Together', text: 'Create groups, assign roles and manage access with granular permissions.' },
  { icon: BarChart3, title: 'Financial Overview', text: 'Get a clear picture of your finances across accounts, budgets and categories.' },
  { icon: FileText, title: 'Documents & Records', text: 'Store important documents securely and keep everything organized.' },
  { icon: Repeat, title: 'Track & Plan', text: "Track transactions, set budgets and plan for your group's goals." },
  { icon: ShieldCheck, title: 'Secure by Design', text: 'Your data is encrypted and protected with enterprise-grade security.' },
]
</script>

<template>
  <div class="web-home">
    <PublicNavbar />

    <!-- Hero Section -->
    <section class="hero">
      <div class="hero-glow"></div>
      <div class="hero-content">
        <span class="eyebrow">Financial clarity for every group</span>
        <h1>
          Combine group <span class="highlight">expense clarity</span>
          while maintaining <span class="highlight">financial independence</span>.
        </h1>
        <p>
          FinBine helps groups and organizations manage money, share insights and stay in control—together.
        </p>
        <div class="hero-cta">
          <RouterLink to="/user/register" class="btn primary">Get Started for Free →</RouterLink>
          <RouterLink to="/user/login" class="btn secondary">Sign In →</RouterLink>
        </div>
        <div class="trust-indicators">
          <span><ShieldCheck size="14" /> Bank-level security</span>
          <span><Users size="14" /> Built for any group</span>
          <span><Lock size="14" /> Your data, your control</span>
        </div>
      </div>
      <div class="hero-visual">
        <img :src="WebHomeMainImg" alt="FinBine dashboard preview on laptop and phone" class="dashboard-preview" />
      </div>
    </section>

    <!-- Features Section -->
    <section class="features">
      <h2>Everything your group needs in one place</h2>
      <div class="feature-grid">
        <div v-for="feature in features" :key="feature.title" class="feature">
          <div class="feature-icon">
            <component :is="feature.icon" size="20" />
          </div>
          <h3>{{ feature.title }}</h3>
          <p>{{ feature.text }}</p>
        </div>
      </div>

      <!-- Mobile: compact list style -->
      <div class="feature-list-mobile">
        <div v-for="feature in features" :key="feature.title + '-m'" class="feature-row">
          <div class="feature-icon small">
            <component :is="feature.icon" size="16" />
          </div>
          <div class="feature-row-text">
            <h3>{{ feature.title }}</h3>
            <p>{{ feature.text }}</p>
          </div>
          <ChevronRight size="16" class="chevron" />
        </div>
      </div>
    </section>

    <!-- CTA Section -->
    <section class="cta">
      <div class="cta-content">
        <div class="cta-left">
          <div class="cta-mark">
            <img src="../../../assets/SVG/logo.svg" alt="" class="cta-logo" />
          </div>
          <div>
            <h2>Ready to take control together?</h2>
            <p>Join thousands of groups who trust FinBine to manage what matters most.</p>
          </div>
        </div>
        <RouterLink to="/user/register" class="btn primary">Get Started for Free →</RouterLink>
      </div>
    </section>

    <PublicFooter />
  </div>
</template>

<style scoped>
.web-home {
  display: flex;
  flex-direction: column;
}

/* ---------- Hero ---------- */
.hero {
  position: relative;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 40px;
  background-color: #04111f;
  color: #fff;
  padding: 56px 40px;
  overflow: hidden;
}

.hero-glow {
  position: absolute;
  top: -120px;
  right: -80px;
  width: 480px;
  height: 480px;
  background: radial-gradient(circle, rgba(59,130,246,0.25) 0%, rgba(59,130,246,0) 70%);
  pointer-events: none;
}

.hero-content {
  position: relative;
  flex: 1;
  max-width: 560px;
}

.eyebrow {
  text-transform: uppercase;
  color: #60a5fa;
  font-size: 0.75rem;
  font-weight: 700;
  letter-spacing: 1.2px;
  margin-bottom: 18px;
  display: block;
}

.hero-content h1 {
  font-size: 2.4rem;
  line-height: 1.15;
  margin-bottom: 20px;
  color: #F1F5F9;
  font-weight: 700;
}

.hero-content .highlight {
  color: #60a5fa;
}

.hero-content p {
  font-size: 1rem;
  line-height: 1.6;
  margin-bottom: 28px;
  color: #CBD5E1;
  max-width: 480px;
}

.hero-cta {
  display: flex;
  gap: 14px;
  margin-bottom: 28px;
  flex-wrap: wrap;
}

.btn {
  padding: 10px 20px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  text-decoration: none;
  transition: background 0.15s, border-color 0.15s, transform 0.1s;
  display: inline-block;
}

.btn.primary {
  background: #1855b9;
  color: #fff;
  border: 1px solid #1855b9;
}

.btn.primary:hover {
  background: #123f8a;
}

.btn.secondary {
  background: transparent;
  color: #F1F5F9;
  border: 1px solid rgba(255,255,255,0.3);
}

.btn.secondary:hover {
  border-color: rgba(255,255,255,0.6);
}

.trust-indicators {
  display: flex;
  gap: 24px;
  flex-wrap: wrap;
  font-size: 0.78rem;
  color: #94A3B8;
}

.trust-indicators span {
  display: flex;
  align-items: center;
  gap: 6px;
}

.hero-visual {
  position: relative;
  flex: 1;
  display: flex;
  justify-content: center;
  min-width: 0;
}

.dashboard-preview {
  max-width: 100%;
  height: auto;
}

/* ---------- Features ---------- */
.features {
  background: #fff;
  padding: 64px 40px;
  text-align: center;
}

.features h2 {
  font-size: 1.7rem;
  font-weight: 700;
  color: #0F172A;
  margin-bottom: 40px;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(5, 1fr);
  gap: 8px;
  max-width: 1200px;
  margin: 0 auto;
}

.feature {
  padding: 0 16px;
  border-right: 1px solid #E5E7EB;
}

.feature:last-child {
  border-right: none;
}

.feature-icon {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: #DBEAFE;
  color: #1855b9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 14px;
}

.feature h3 {
  font-size: 0.9rem;
  font-weight: 700;
  color: #0F172A;
  margin-bottom: 8px;
}

.feature p {
  font-size: 0.78rem;
  line-height: 1.5;
  color: #64748B;
}

.feature-list-mobile {
  display: none;
}

/* ---------- CTA ---------- */
.cta {
  background: #04111f;
  padding: 36px 40px;
}

.cta-content {
  max-width: 1200px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 24px;
  flex-wrap: wrap;
}

.cta-left {
  display: flex;
  align-items: center;
  gap: 16px;
}

.cta-mark {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: #0d2847;
  display: flex;
  align-items: center;
  justify-content: center;
  flex-shrink: 0;
}

.cta-logo {
  height: 22px;
  filter: brightness(0) saturate(100%) invert(60%) sepia(90%) saturate(1000%) hue-rotate(190deg);
}

.cta h2 {
  font-size: 1.15rem;
  color: #fff;
  margin-bottom: 4px;
}

.cta p {
  font-size: 0.8rem;
  color: #94A3B8;
  margin: 0;
}

/* ---------- Responsive ---------- */
@media (max-width: 900px) {
  .hero {
    flex-direction: column;
    text-align: left;
    padding: 32px 20px 40px;
  }

  .hero-content {
    max-width: 100%;
  }

  .hero-content h1 {
    font-size: 1.7rem;
  }

  .hero-visual {
    margin-top: 24px;
    width: 100%;
  }

  .features {
    padding: 40px 20px;
    text-align: left;
  }

  .features h2 {
    text-align: center;
    font-size: 1.3rem;
  }

  .feature-grid {
    display: none;
  }

  .feature-list-mobile {
    display: flex;
    flex-direction: column;
    max-width: 480px;
    margin: 0 auto;
  }

  .feature-row {
    display: flex;
    align-items: flex-start;
    gap: 12px;
    padding: 14px 4px;
    border-bottom: 1px solid #F1F5F9;
  }

  .feature-icon.small {
    width: 34px;
    height: 34px;
    flex-shrink: 0;
    margin: 0;
  }

  .feature-row-text {
    flex: 1;
  }

  .feature-row-text h3 {
    margin-bottom: 3px;
  }

  .chevron {
    color: #94A3B8;
    margin-top: 8px;
    flex-shrink: 0;
  }

  .cta-content {
    flex-direction: column;
    align-items: flex-start;
  }

  .cta .btn.primary {
    width: 100%;
    text-align: center;
  }
}
</style>