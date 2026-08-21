<script setup>
import { ref, watch, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { Check, UserCheck, AlertTriangle } from 'lucide-vue-next'

const props = defineProps({
  state: { type: String, required: true } // 'loading' | 'success' | 'duplicate' | 'error'
})

const emit = defineEmits(['retry'])

const router = useRouter()

const loadingStages = [
  { title: 'Checking your details', subtitle: 'Making sure everything looks good.' },
  { title: 'Building your profile', subtitle: 'Preparing your personal FinBine space.' },
  { title: 'Securing your account', subtitle: 'Setting up your authentication securely.' },
  { title: 'Preparing your workspace', subtitle: 'Getting everything ready for you.' }
]

const stageIndex = ref(0)
let stageTimer = null

function startCycling() {
  stageIndex.value = 0
  stageTimer = setInterval(() => {
    // Stop at the last stage rather than looping back to the start —
    // looping backwards would read as something going wrong.
    if (stageIndex.value < loadingStages.length - 1) {
      stageIndex.value++
    }
  }, 1100)
}

function stopCycling() {
  clearInterval(stageTimer)
  stageTimer = null
}

watch(
  () => props.state,
  (newState) => {
    if (newState === 'loading') startCycling()
    else stopCycling()
  },
  { immediate: true }
)

onUnmounted(stopCycling)

function goLoginNow() {
  router.push('/user/login')
}
function goLater() {
  router.push('/')
}
function tryAgainNow() {
  emit('retry')
}
</script>

<template>
  <Teleport to="body">
    <div class="modal-overlay">
      <div class="modal-card">
        <template v-if="state === 'loading'">
          <div class="spinner"></div>
          <h3>{{ loadingStages[stageIndex].title }}</h3>
          <p>{{ loadingStages[stageIndex].subtitle }}</p>
        </template>

        <template v-else-if="state === 'success'">
          <div class="icon-badge success"><Check :size="30" /></div>
          <h3>Welcome to FinBine</h3>
          <p>Your account has been created successfully.<br />You're all set and ready to go.</p>
          <div class="modal-actions">
            <button class="btn-primary" @click="goLoginNow">Login Now</button>
            <button class="btn-secondary" @click="goLater">Login Later</button>
          </div>
        </template>
         <template v-else-if="state === 'success-email-warning'">
          <div class="icon-badge warning"><AlertTriangle :size="28" /></div>
          <h3>Account created — one more step</h3>
          <p>Your FinBine account was created, but we couldn't send the email to set your password. Please contact support so we can get you signed in.</p>
          <div class="modal-actions">
            <button class="btn-primary" @click="goLater">Back to Home</button>
          </div>
        </template>

        <template v-else-if="state === 'duplicate'">
          <div class="icon-badge duplicate"><UserCheck :size="28" /></div>
          <h3>You're already part of FinBine</h3>
          <p>Looks like this account already exists —<br />let's get you signed in instead.</p>
          <div class="modal-actions">
            <button class="btn-primary" @click="goLoginNow">Login Now</button>
            <button class="btn-secondary" @click="goLater">Login Later</button>
          </div>
        </template>

        <template v-else-if="state === 'error'">
          <div class="icon-badge error"><AlertTriangle :size="28" /></div>
          <h3>We couldn't complete your registration</h3>
          <p>Something prevented us from completing your FinBine account.<br />The FinBine team has been notified and will investigate the issue.</p>
          <div class="modal-actions">
            <button class="btn-primary" @click="tryAgainNow">Try Again Now</button>
            <button class="btn-secondary" @click="goLater">Try Again Later</button>
          </div>
        </template>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(4, 17, 31, 0.6);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 200;
  padding: 20px;
}

.modal-card {
  background: #fff;
  border-radius: 12px;
  padding: 36px 32px;
  max-width: 380px;
  width: 100%;
  text-align: center;
  box-shadow: 0 12px 40px rgba(0, 0, 0, 0.25);
}

.spinner {
  width: 46px;
  height: 46px;
  border: 4px solid #DBEAFE;
  border-top-color: #1855b9;
  border-radius: 50%;
  margin: 0 auto 22px;
  animation: spin 0.8s linear infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.modal-card h3 {
  font-size: 1.05rem;
  color: #0F172A;
  margin: 0 0 8px;
}

.modal-card p {
  font-size: 0.85rem;
  color: #64748B;
  line-height: 1.55;
  margin: 0;
}

.icon-badge {
  width: 56px;
  height: 56px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  margin: 0 auto 18px;
}

.icon-badge.success { background: #DCFCE7; color: #15803D; }
.icon-badge.duplicate { background: #DBEAFE; color: #1D4ED8; }
.icon-badge.error { background: #FEE2E2; color: #B91C1C; }
.icon-badge.warning { background: #FEF3C7; color: #B45309; }

.modal-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
  margin-top: 24px;
}

.btn-primary,
.btn-secondary {
  padding: 10px 16px;
  border-radius: 6px;
  font-size: 0.85rem;
  font-weight: 600;
  cursor: pointer;
  border: none;
}

.btn-primary {
  background: #1855b9;
  color: #fff;
}

.btn-primary:hover {
  background: #123f8a;
}

.btn-secondary {
  background: #fff;
  color: #334155;
  border: 1px solid #E2E8F0;
}

.btn-secondary:hover {
  background: #F8FAFC;
}
</style>