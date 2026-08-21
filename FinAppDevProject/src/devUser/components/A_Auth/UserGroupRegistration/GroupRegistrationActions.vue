<script setup>
import { Send, Loader2 } from 'lucide-vue-next'

defineProps({
  accountType: { type: String, required: true }, // 'premium' | 'free'
  isSubmitting: { type: Boolean, default: false },
  actionMessage: { type: String, default: '' }
})

defineEmits(['submit'])
</script>

<template>
  <div class="actions-block">
    <button type="button" class="cta-btn" :disabled="isSubmitting" @click="$emit('submit')">
      <Loader2 v-if="isSubmitting" :size="13" class="spin" />
      <Send v-else :size="13" />
      {{ isSubmitting ? 'Creating your group…' : `Create my ${accountType === 'premium' ? 'Premium' : 'Free'} group` }}
    </button>

    <p v-if="actionMessage" class="action-message">{{ actionMessage }}</p>
  </div>
</template>

<style scoped>
.actions-block {
  margin-top: 14px;
}

/* Same chrome as the login page's primary button — dark navy fill,
   white text, hover lightens. Reference: LoginUser.vue .signin-btn and
   loginAdmin.css .login-button */
.cta-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 7px;
  padding: 10px 16px;
  border: none;
  border-radius: 10px;
  background: #0b1220;
  color: #fff;
  font-family: inherit;
  font-size: 0.8rem;
  font-weight: 700;
  cursor: pointer;
  transition: background 0.15s, opacity 0.15s;
}

.cta-btn svg {
  color: #2dd4bf;
}

.cta-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
}

.cta-btn:not(:disabled):hover {
  background: #14213d;
}

.spin {
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.action-message {
  text-align: center;
  font-size: 0.72rem;
  color: #b45309;
  margin: 8px 0 0;
}

@media (max-width: 960px) {
  .cta-btn {
    padding: 13px 16px;
    font-size: 0.88rem;
  }
}
</style>
