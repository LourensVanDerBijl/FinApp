<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Same visual language as GroupAssignSidebar (dark navy gradient, radial
// glow, faint logo watermark) — kept as its own component in this page's
// folder rather than shared, so each page's component set stays
// self-contained per the project's folder convention.
// ─────────────────────────────────────────────────────────────────────────
import logo from '../../../../assets/SVG/logo.svg'
import { ShieldCheck, Sparkles, Users, BarChart3 } from 'lucide-vue-next'

const features = [
  {
    icon: ShieldCheck,
    title: 'Secure',
    description: 'Protected with banking-grade security.'
  },
  {
    icon: Sparkles,
    title: 'Quick Setup',
    description: 'Just a couple of details and your group is ready.'
  },
  {
    icon: Users,
    title: 'Group-Focused',
    description: 'Share and manage group expenses, not personal accounts.'
  },
  {
    icon: BarChart3,
    title: 'Transparent',
    description: 'Clear visibility into group finances for everyone.'
  }
]
</script>

<template>
  <aside class="sidebar">
    <div class="sidebar-decor" aria-hidden="true"></div>

    <div class="sidebar-content">
      <div class="brand-row">
        <img :src="logo" alt="FinBine" class="brand-logo" />
        <span class="brand-wordmark">FinBine</span>
      </div>

      <p class="brand-tagline">
        You're almost done — just name your group and
        <span class="highlight">FinBine</span> takes care of the rest.
      </p>

      <ul class="feature-list">
        <li v-for="feature in features" :key="feature.title" class="feature-item">
          <span class="feature-icon">
            <component :is="feature.icon" :size="15" />
          </span>
          <div>
            <h4>{{ feature.title }}</h4>
            <p>{{ feature.description }}</p>
          </div>
        </li>
      </ul>
    </div>

    <p class="sidebar-copyright">© {{ new Date().getFullYear() }} FinBine. All rights reserved.</p>

    <!-- Faint logo watermark, echoing the Admin/User login panels -->
    <img :src="logo" alt="" aria-hidden="true" class="sidebar-watermark" />
  </aside>
</template>

<style scoped>
.sidebar {
  flex: 0 0 260px;
  position: relative;
  min-height: 100vh;
  overflow: hidden;
  padding: 22px 24px 16px;
  background: linear-gradient(165deg, #0d1728 0%, #0b1220 55%, #0a0f1c 100%);
  border-right: 1px solid rgba(255, 255, 255, 0.08);
  display: flex;
  flex-direction: column;
}

.sidebar-decor {
  position: absolute;
  inset: 0;
  pointer-events: none;
  background:
    radial-gradient(circle at 10% 90%, rgba(45, 212, 191, 0.14) 0%, rgba(45, 212, 191, 0) 50%),
    radial-gradient(circle at 90% 10%, rgba(79, 142, 247, 0.14) 0%, rgba(79, 142, 247, 0) 50%);
}

.sidebar-content {
  position: relative;
  z-index: 1;
  min-height: 0;
}

.brand-row {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-bottom: 10px;
}

.brand-logo {
  height: 26px;
}

.brand-wordmark {
  font-size: 1rem;
  font-weight: 700;
  color: #fff;
}

.brand-tagline {
  font-size: 0.74rem;
  color: #94a3b8;
  line-height: 1.45;
  margin: 0 0 22px;
}

.highlight {
  color: #1855b9;
}

.feature-list {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.feature-item {
  display: flex;
  gap: 9px;
}

.feature-icon {
  flex-shrink: 0;
  width: 28px;
  height: 28px;
  border-radius: 50%;
  background: rgba(45, 212, 191, 0.14);
  color: #2dd4bf;
  display: flex;
  align-items: center;
  justify-content: center;
}

.feature-item h4 {
  font-size: 0.76rem;
  font-weight: 700;
  color: #fff;
  margin: 0 0 1px;
}

.feature-item p {
  font-size: 0.68rem;
  color: #94a3b8;
  line-height: 1.35;
  margin: 0;
}

.sidebar-copyright {
  position: relative;
  z-index: 1;
  font-size: 0.64rem;
  color: #64748b;
  margin: auto 0 0;
  flex-shrink: 0;
}

/* Faint large logo watermark, bottom-right, echoing the Admin/User login panels */
.sidebar-watermark {
  position: absolute;
  bottom: 0px;
  right: -40px;
  width: 220px;
  opacity: 0.05;
  pointer-events: none;
}

@media (max-width: 960px) {
  .sidebar {
    display: none;
  }
}
</style>
