<script setup>
import { Bell, ScanLine, Sparkles, MessageCircleHeart, Info, Crown } from 'lucide-vue-next'

defineProps({
  accountType: { type: String, required: true } // 'premium' | 'free'
})

const emit = defineEmits(['switch-type'])

// Placeholder perks — not live features yet, just enough to communicate
// what Premium is meant to offer.
const perks = [
  { icon: Bell, text: 'Smart reminders for upcoming bills and contributions' },
  { icon: ScanLine, text: 'Scan and upload receipts & documents' },
  { icon: Sparkles, text: 'AI overlay that tracks your expense changes' },
  { icon: MessageCircleHeart, text: 'Personalized advisory assistance' }
]
</script>

<template>
  <!-- ============================== FREE ============================== -->
  <aside v-if="accountType === 'free'" class="info-panel">
    <div class="info-icon"><Info :size="16" /></div>
    <p class="info-kicker">Current selection: Free account</p>
    <h3>Premium is highly recommended</h3>
    <p class="info-lead">A Premium group unlocks the full FinBine experience for everyone in it:</p>

    <ul class="perk-list">
      <li v-for="perk in perks" :key="perk.text">
        <component :is="perk.icon" :size="13" />
        <span>{{ perk.text }}</span>
      </li>
    </ul>

    <button type="button" class="upgrade-btn" @click="emit('switch-type', 'premium')">
      <Crown :size="13" />
      Upgrade to Premium
    </button>
  </aside>

  <!-- ============================= PREMIUM ============================= -->
  <aside v-else class="info-panel premium">
    <div class="info-icon premium"><Crown :size="16" /></div>
    <p class="info-kicker premium">Premium — FinBine's best experience</p>
    <h3>Everything your group needs</h3>
    <p class="info-lead">Premium offers your group:</p>

    <ul class="perk-list">
      <li v-for="perk in perks" :key="perk.text">
        <component :is="perk.icon" :size="13" />
        <span>{{ perk.text }}</span>
      </li>
    </ul>

    <p class="fine-print">
      Account status follows the group's status — group owners are responsible for FinBine fees.
    </p>

    <button type="button" class="try-free-link" @click="emit('switch-type', 'free')">
      I want to give the Free account a try first
    </button>
  </aside>
</template>

<style scoped>
.info-panel {
  background: rgba(37, 99, 235, 0.05);
  border: 1px solid rgba(37, 99, 235, 0.16);
  border-radius: 13px;
  padding: 16px 18px;
}

.info-panel.premium {
  background: rgba(45, 212, 191, 0.06);
  border-color: rgba(45, 212, 191, 0.2);
}

.info-icon {
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: rgba(37, 99, 235, 0.12);
  color: #1855b9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin-bottom: 8px;
}

.info-icon.premium {
  background: rgba(45, 212, 191, 0.16);
  color: #0f9c8a;
}

.info-kicker {
  font-size: 0.62rem;
  font-weight: 700;
  letter-spacing: 0.03em;
  text-transform: uppercase;
  color: #1855b9;
  margin: 0 0 4px;
}

.info-kicker.premium {
  color: #0f9c8a;
}

.info-panel h3 {
  font-size: 0.9rem;
  color: #0f172a;
  margin: 0 0 6px;
}

.info-lead {
  font-size: 0.72rem;
  color: #64748b;
  margin: 0 0 10px;
  line-height: 1.4;
}

.perk-list {
  list-style: none;
  margin: 0 0 12px;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.perk-list li {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  font-size: 0.72rem;
  color: #334155;
  line-height: 1.35;
}

.perk-list li svg {
  flex-shrink: 0;
  margin-top: 2px;
  color: #1855b9;
}

.premium .perk-list li svg {
  color: #0f9c8a;
}

.upgrade-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 7px;
  padding: 9px 14px;
  border: none;
  border-radius: 9px;
  background: #0b1220;
  color: #fff;
  font-family: inherit;
  font-size: 0.74rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.15s;
}

.upgrade-btn svg {
  color: #2dd4bf;
}

.upgrade-btn:hover {
  background: #14213d;
}

.fine-print {
  font-size: 0.64rem;
  color: #64748b;
  line-height: 1.4;
  margin: 0 0 12px;
  padding-top: 10px;
  border-top: 1px solid rgba(15, 23, 42, 0.08);
}

.try-free-link {
  width: 100%;
  text-align: center;
  border: none;
  background: none;
  color: #1855b9;
  font-family: inherit;
  font-size: 0.7rem;
  font-weight: 600;
  cursor: pointer;
  text-decoration: underline;
}

@media (max-width: 960px) {
  .info-panel {
    margin-top: 16px;
  }
}
</style>
