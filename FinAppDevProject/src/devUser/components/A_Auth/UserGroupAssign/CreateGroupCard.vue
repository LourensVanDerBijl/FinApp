<script setup>
import { Users, Crown, ShieldCheck, ChevronRight, SlidersHorizontal } from 'lucide-vue-next'

defineProps({
  isHiddenMobile: { type: Boolean, default: false }
})

// Emits 'create' with the chosen tier ('premium' | 'free') — picking a
// tier IS the action now, there's no separate confirm button below.
const emit = defineEmits(['create'])

const ownerCapabilities = [
  { icon: Users, label: 'Manage members' },
  { icon: SlidersHorizontal, label: 'Control group settings' },
  { icon: Crown, label: 'Manage subscriptions' }
]
</script>

<template>
  <section class="option-card create-card" :class="{ 'is-hidden-mobile': isHiddenMobile }">
    <div class="card-icon teal"><Users :size="19" /></div>
    <h2>Create a New Group</h2>
    <p class="card-sub">Start your own FinBine group and invite others to manage finances together.</p>

    <p class="section-label">Choose your group type</p>

    <div class="tier-options">
      <button type="button" class="tier-btn" @click="emit('create', 'premium')">
        <span class="tier-icon premium"><Crown :size="15" /></span>
        <span class="tier-text">
          <span class="tier-badge">★ RECOMMENDED</span>
          <strong>Create Premium Group</strong>
          <span>Unlock all premium features and advanced tools for your group.</span>
        </span>
        <ChevronRight :size="14" class="tier-chevron" />
      </button>

      <button type="button" class="tier-btn" @click="emit('create', 'free')">
        <span class="tier-icon free"><ShieldCheck :size="15" /></span>
        <span class="tier-text">
          <strong>Create Free Group</strong>
          <span>Get started with core features at no cost.</span>
        </span>
        <ChevronRight :size="14" class="tier-chevron" />
      </button>
    </div>

    <div class="owner-box">
      <p class="owner-title">As the group owner, you can:</p>
      <div class="owner-grid">
        <div v-for="cap in ownerCapabilities" :key="cap.label" class="owner-cap">
          <component :is="cap.icon" :size="14" />
          <span>{{ cap.label }}</span>
        </div>
      </div>
    </div>
  </section>
</template>

<style scoped>
.option-card {
  background: rgba(37, 99, 235, 0.05);
  border: 1px solid rgba(37, 99, 235, 0.16);
  border-radius: 13px;
  padding: 14px 16px;
  display: flex;
  flex-direction: column;
  min-height: 0;
  overflow: hidden;
}

.create-card {
  box-shadow: 0 0 0 1px rgba(45, 212, 191, 0.12), 0 14px 28px rgba(15, 23, 42, 0.06);
}

.card-icon {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 7px;
  flex-shrink: 0;
}

.card-icon.teal {
  background: rgba(45, 212, 191, 0.14);
  color: #0f9c8a;
}

.option-card h2 {
  font-size: 0.92rem;
  color: #0f172a;
  margin: 0 0 4px;
  flex-shrink: 0;
}

.card-sub {
  font-size: 0.68rem;
  color: #64748b;
  line-height: 1.35;
  margin: 0 0 10px;
  flex-shrink: 0;
}

.section-label {
  font-size: 0.58rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  color: #64748b;
  text-transform: uppercase;
  margin: 0 0 6px;
  flex-shrink: 0;
}

/* ------------------------- TIER BUTTONS (the CTAs) ------------------------- */
.tier-options {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin-bottom: 10px;
  flex-shrink: 0;
}

/* Same chrome as the login page's primary button (signin-btn / login-button):
   dark navy fill, white text, hover lightens. Reference:
   LoginUser.vue .signin-btn and loginAdmin.css .login-button */
.tier-btn {
  position: relative;
  display: flex;
  align-items: center;
  gap: 10px;
  text-align: left;
  width: 100%;
  padding: 9px 12px;
  border: none;
  border-radius: 10px;
  background: #0b1220;
  cursor: pointer;
  font-family: inherit;
  transition: background 0.15s;
}

.tier-btn:hover {
  background: #14213d;
}

.tier-icon {
  flex-shrink: 0;
  width: 27px;
  height: 27px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}

.tier-icon.premium {
  background: rgba(45, 212, 191, 0.18);
  color: #2dd4bf;
}

.tier-icon.free {
  background: rgba(16, 185, 129, 0.18);
  color: #34d399;
}

.tier-text {
  display: flex;
  flex-direction: column;
  gap: 0;
  flex: 1;
  min-width: 0;
}

.tier-badge {
  font-size: 0.54rem;
  font-weight: 700;
  color: #2dd4bf;
}

.tier-text strong {
  font-size: 0.72rem;
  color: #fff;
}

.tier-text span:not(.tier-badge) {
  font-size: 0.62rem;
  color: #94a3b8;
  line-height: 1.25;
}

.tier-chevron {
  flex-shrink: 0;
  color: #64748b;
}

/* ------------------------------ OWNER BOX ------------------------------ */
.owner-box {
  background: rgba(15, 23, 42, 0.03);
  border: 1px solid rgba(15, 23, 42, 0.06);
  border-radius: 9px;
  padding: 8px 10px;
  margin-top: auto;
  flex-shrink: 0;
}

.owner-title {
  font-size: 0.66rem;
  font-weight: 700;
  color: #334155;
  margin: 0 0 7px;
}

.owner-grid {
  display: flex;
  justify-content: space-between;
  gap: 6px;
}

.owner-cap {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
  text-align: center;
  color: #64748b;
  flex: 1;
}

.owner-cap svg {
  color: #0f9c8a;
}

.owner-cap span {
  font-size: 0.56rem;
  line-height: 1.15;
}

@media (max-width: 960px) {
  .option-card {
    overflow: visible;
    padding: 22px 20px;
  }

  .option-card h2 {
    font-size: 1.05rem;
  }

  .card-sub {
    font-size: 0.78rem;
  }

  .tier-text strong {
    font-size: 0.86rem;
  }

  .tier-text span:not(.tier-badge) {
    font-size: 0.74rem;
  }

  .tier-btn {
    padding: 14px 16px;
  }

  .option-card.is-hidden-mobile {
    display: none;
  }
}
</style>
