<script setup>
import { reactive, computed, watch } from 'vue'
import { X } from 'lucide-vue-next'

const props = defineProps({
  open: { type: Boolean, required: true },
  parent: { type: Object, default: null } // the node this new admin will report to
})

const emit = defineEmits(['close', 'add'])

const form = reactive({
  preferName: '',
  surname: '',
  role: '',
  accountType: 'Admin'
})

function resetForm() {
  form.preferName = ''
  form.surname = ''
  form.role = ''
  form.accountType = 'Admin'
}

watch(() => props.open, (isOpen) => {
  if (isOpen) resetForm()
})

const canSubmit = computed(() =>
  form.preferName.trim() && form.surname.trim() && form.role.trim()
)

function handleAdd() {
  if (!canSubmit.value) return
  emit('add', { ...form })
}
</script>

<template>
  <Teleport to="body">
    <div v-if="open" class="modal-overlay" @click="emit('close')"></div>

    <div v-if="open" class="modal">
      <div class="modal-header">
        <div>
          <p class="modal-eyebrow">New Admin</p>
          <h2>Add Direct Report</h2>
        </div>
        <button class="close-btn" @click="emit('close')">
          <X size="18" />
        </button>
      </div>

      <div class="modal-body">
        <p v-if="parent" class="reports-to">
          Reports to <strong>{{ [parent.preferName, parent.surname].filter(Boolean).join(' ') }}</strong> ({{ parent.role }})
        </p>

        <div class="field-row">
          <label class="field">
            <span class="field-label">Preferred Name</span>
            <input v-model="form.preferName" type="text" placeholder="e.g. Ane" />
          </label>
          <label class="field">
            <span class="field-label">Surname</span>
            <input v-model="form.surname" type="text" placeholder="e.g. Van Der Bijl" />
          </label>
        </div>

        <label class="field">
          <span class="field-label">Role / Title</span>
          <input v-model="form.role" type="text" placeholder="e.g. Junior Developer" />
        </label>

        <label class="field">
          <span class="field-label">Account Type</span>
          <select v-model="form.accountType">
            <option>Super Admin</option>
            <option>Admin</option>
            <option>Developer Admin</option>
          </select>
        </label>

        <p class="provisioning-note">
          Creating an admin will also need to provision a Premium FinBine user
          account (both the Admin and User tables). That flow isn't built yet —
          this only adds the org-chart entry for now.
        </p>
      </div>

      <div class="modal-footer">
        <button class="btn-outline" @click="emit('close')">Cancel</button>
        <button class="btn-primary" :disabled="!canSubmit" @click="handleAdd">Add to Org Chart</button>
      </div>
    </div>
  </Teleport>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.35);
  z-index: 90;
}

.modal {
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: min(440px, 92vw);
  max-height: 86vh;
  background: #fff;
  border-radius: 10px;
  z-index: 100;
  display: flex;
  flex-direction: column;
  box-shadow: 0 12px 32px rgba(0, 0, 0, 0.18);
}

.modal-header {
  display: flex;
  align-items: flex-start;
  justify-content: space-between;
  padding: 18px 22px;
  border-bottom: 1px solid #E5E7EB;
  flex-shrink: 0;
}

.modal-eyebrow {
  font-size: 0.68rem;
  color: #2563EB;
  font-weight: 700;
  margin: 0 0 2px 0;
}

.modal-header h2 {
  font-size: 1.05rem;
  margin: 0;
  color: #0F172A;
}

.close-btn {
  background: none;
  border: none;
  color: #64748B;
  cursor: pointer;
  padding: 4px;
}

.close-btn:hover {
  color: #0F172A;
}

.modal-body {
  padding: 18px 22px;
  overflow-y: auto;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.reports-to {
  font-size: 0.74rem;
  color: #64748B;
  background: #F8FAFC;
  border: 1px solid #E5E7EB;
  border-radius: 6px;
  padding: 8px 10px;
}

.reports-to strong {
  color: #0F172A;
}

.field-row {
  display: flex;
  gap: 12px;
}

.field {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.field-label {
  font-size: 0.64rem;
  font-weight: 700;
  color: #64748B;
  text-transform: uppercase;
  letter-spacing: 0.3px;
}

.field input,
.field select {
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 7px 10px;
  font-size: 0.78rem;
  color: #0F172A;
  outline: none;
}

.field input:focus,
.field select:focus {
  border-color: #2563EB;
}

.provisioning-note {
  font-size: 0.68rem;
  color: #94A3B8;
  line-height: 1.5;
  border-top: 1px dashed #E2E8F0;
  padding-top: 12px;
}

.modal-footer {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 14px 22px;
  border-top: 1px solid #E5E7EB;
  flex-shrink: 0;
}

.btn-outline,
.btn-primary {
  border-radius: 6px;
  font-size: 0.72rem;
  font-weight: 600;
  padding: 7px 14px;
  cursor: pointer;
}

.btn-outline {
  background: #fff;
  border: 1px solid #E2E8F0;
  color: #334155;
}

.btn-primary {
  background: #2563EB;
  border: none;
  color: #fff;
}

.btn-primary:disabled {
  background: #94A3B8;
  cursor: not-allowed;
}
</style>
