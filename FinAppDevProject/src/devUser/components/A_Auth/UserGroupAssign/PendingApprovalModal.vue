<script setup>
import { Clock3 } from 'lucide-vue-next'

defineProps({
  groupName: { type: String, default: '' }
})

const emit = defineEmits(['wait', 'join-different'])
</script>

<template>
  <div class="modal-overlay">
    <div class="modal-card">
      <div class="modal-icon"><Clock3 :size="22" /></div>
      <h2>Waiting for Approval</h2>
      <p class="modal-body">
        Your request to join
        <strong>{{ groupName || 'this group' }}</strong>
        has been sent. You'll get access as soon as the group owner approves it.
      </p>

      <div class="modal-actions">
        <button type="button" class="btn-primary" @click="emit('wait')">
          Let's wait for the owner
        </button>
        <button type="button" class="btn-secondary" @click="emit('join-different')">
          Join a different group
        </button>
      </div>
    </div>
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
  padding: 20px;
}

.modal-card {
  background: #fff;
  border-radius: 14px;
  padding: 28px 26px 24px;
  max-width: 380px;
  width: 100%;
  text-align: center;
  box-shadow: 0 20px 50px rgba(15, 23, 42, 0.25);
}

.modal-icon {
  width: 44px;
  height: 44px;
  border-radius: 50%;
  background: rgba(37, 99, 235, 0.1);
  color: #1855b9;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 14px;
}

.modal-card h2 {
  font-size: 1.05rem;
  color: #0f172a;
  margin: 0 0 8px;
}

.modal-body {
  font-size: 0.82rem;
  color: #475569;
  line-height: 1.5;
  margin: 0 0 20px;
}

.modal-body strong {
  color: #0f172a;
}

.modal-actions {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.btn-primary,
.btn-secondary {
  width: 100%;
  padding: 10px 14px;
  border-radius: 9px;
  font-family: inherit;
  font-size: 0.8rem;
  font-weight: 700;
  cursor: pointer;
  transition: opacity 0.15s, background 0.15s;
}

.btn-primary {
  background: #0b1220;
  color: #fff;
  border: none;
}

.btn-primary:hover {
  background: #14213d;
}

.btn-secondary {
  background: #fff;
  color: #0f172a;
  border: 1px solid #e2e8f0;
}

.btn-secondary:hover {
  background: #f8fafc;
}
</style>
