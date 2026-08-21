<script setup>
import { computed } from 'vue'
import { UserPlus, Hash, Info, Check, Send } from 'lucide-vue-next'

const props = defineProps({
  groupId: { type: String, required: true },
  isHiddenMobile: { type: Boolean, default: false }
})

const emit = defineEmits(['update:groupId', 'submit'])

const canRequestJoin = computed(() => props.groupId.trim().length === 8)

function handleInput(event) {
  emit('update:groupId', event.target.value)
}
</script>

<template>
  <section class="option-card join-card" :class="{ 'is-hidden-mobile': isHiddenMobile }">
    <div class="card-icon blue"><UserPlus :size="19" /></div>
    <h2>Join an Existing Group</h2>
    <p class="card-sub">Been invited to a group? Enter the Group ID below to request to join an existing group.</p>

    <p class="field-label">Group ID</p>
    <div class="input-wrap">
      <Hash :size="14" class="input-icon" />
      <input
        type="text"
        :value="groupId"
        @input="handleInput"
        maxlength="8"
        placeholder="Enter the 8-character Group ID"
      />
    </div>

    <div class="info-box">
      <Info :size="14" class="info-icon" />
      <div>
        <strong>What happens next?</strong>
        <ul>
          <li><Check :size="11" /> Your request will be sent to the group owner.</li>
          <li><Check :size="11" /> You'll be notified once they approve or decline.</li>
          <li><Check :size="11" /> Once approved, you can start managing group expenses together.</li>
        </ul>
      </div>
    </div>

    <button type="button" class="cta-btn blue" :disabled="!canRequestJoin" @click="emit('submit')">
      <Send :size="13" />
      Request to Join
    </button>
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

.card-icon.blue {
  background: rgba(37, 99, 235, 0.12);
  color: #1855b9;
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

.field-label {
  font-size: 0.58rem;
  font-weight: 700;
  letter-spacing: 0.04em;
  color: #64748b;
  text-transform: uppercase;
  margin: 0 0 6px;
  flex-shrink: 0;
}

/* -------------------------------- INPUT -------------------------------- */
.input-wrap {
  position: relative;
  display: flex;
  align-items: center;
  margin-bottom: 10px;
  flex-shrink: 0;
}

.input-icon {
  position: absolute;
  left: 11px;
  color: #94a3b8;
}

.input-wrap input {
  width: 100%;
  padding: 8px 10px 8px 32px;
  border-radius: 9px;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #0f172a;
  font-family: inherit;
  font-size: 0.76rem;
  outline: none;
  transition: border-color 0.15s;
}

.input-wrap input::placeholder {
  color: #94a3b8;
}

.input-wrap input:focus {
  border-color: #1855b9;
}

.info-box {
  display: flex;
  gap: 8px;
  background: #eff6ff;
  border: 1px solid #dbeafe;
  border-radius: 9px;
  padding: 9px 11px;
  margin-bottom: 10px;
  flex-shrink: 0;
}

.info-icon {
  flex-shrink: 0;
  color: #1855b9;
  margin-top: 1px;
}

.info-box strong {
  display: block;
  font-size: 0.7rem;
  color: #0f172a;
  margin-bottom: 5px;
}

.info-box ul {
  list-style: none;
  margin: 0;
  padding: 0;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.info-box li {
  display: flex;
  align-items: flex-start;
  gap: 5px;
  font-size: 0.66rem;
  color: #334155;
  line-height: 1.35;
}

.info-box li svg {
  flex-shrink: 0;
  color: #16a34a;
  margin-top: 2px;
}

/* -------------------------------- BUTTON -------------------------------- */
.cta-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 7px;
  padding: 9px 14px;
  border: none;
  border-radius: 9px;
  font-family: inherit;
  font-size: 0.74rem;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.15s, background 0.15s;
  flex-shrink: 0;
  margin-top: auto;
}

.cta-btn.blue {
  background: #0b1220;
  color: #fff;
}

.cta-btn.blue svg {
  color: #2dd4bf;
}

.cta-btn:disabled {
  opacity: 0.45;
  cursor: not-allowed;
}

.cta-btn:not(:disabled):hover {
  background: #14213d;
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

  .cta-btn {
    padding: 12px 16px;
    font-size: 0.85rem;
    margin-top: 16px;
  }

  .option-card.is-hidden-mobile {
    display: none;
  }
}
</style>
