<script setup>
// ─────────────────────────────────────────────────────────────────────────
// Admin > Development > Test Data. Upload a JSON file shaped like
// TestDataPayload on the backend (schemaVersion/source/description/groups,
// each group carrying one owner + a members list) and batch-import it
// across Firebase Auth + Firestore + Postgres in one call.
//
// Deliberately dumb on the frontend: this component does NOT know the
// shape of a "user" or a "group" beyond what it needs to show a preview
// count and a per-group/per-member summary table. All real validation
// (required fields, matching GroupStatus rules, duplicate emails, etc.)
// happens server-side in AdminTestDataImportService — so when that
// shape changes next week, this file doesn't need to.
// ─────────────────────────────────────────────────────────────────────────
import { ref, computed } from 'vue'
import { importTestData, clearAllTestData } from '../../data/adminTestDataImport.js'
import { UploadCloud, FileJson, CheckCircle2, XCircle, AlertTriangle, RotateCcw, Trash2, ShieldAlert } from 'lucide-vue-next'

const fileInput = ref(null)
const fileName = ref('')
const parsedPayload = ref(null)
const parseError = ref('')

const isImporting = ref(false)
const result = ref(null) // { success, message, validationErrors, groupsCreated, usersCreated }

const groupCount = computed(() => parsedPayload.value?.groups?.length ?? 0)
const userCount = computed(() => {
  if (!parsedPayload.value?.groups) return 0
  return parsedPayload.value.groups.reduce((sum, g) => sum + 1 + (g.members?.length ?? 0), 0)
})

function triggerFilePicker() {
  fileInput.value?.click()
}

function resetSelection() {
  fileName.value = ''
  parsedPayload.value = null
  parseError.value = ''
  result.value = null
  if (fileInput.value) fileInput.value.value = ''
}

async function handleFileChange(event) {
  const file = event.target.files?.[0]
  if (!file) return

  result.value = null
  parseError.value = ''
  fileName.value = file.name

  try {
    const text = await file.text()
    const json = JSON.parse(text)

    if (!json || typeof json !== 'object' || !Array.isArray(json.groups)) {
      parseError.value = 'This doesn\'t look like a Test Data file — expected a "groups" array at the top level.'
      parsedPayload.value = null
      return
    }

    parsedPayload.value = json
  } catch (err) {
    parseError.value = `Couldn't parse this file as JSON: ${err.message}`
    parsedPayload.value = null
  }
}

async function handleImport() {
  if (!parsedPayload.value || isImporting.value) return

  isImporting.value = true
  result.value = null

  try {
    result.value = await importTestData(parsedPayload.value)
  } catch (err) {
    result.value = { success: false, message: err.message || 'Import failed.' }
  } finally {
    isImporting.value = false
  }
}

// ------------------------------------------------------------
// Clear All — wipes every user row across all 3 stores except this
// email. Backend also hardcodes this same email server-side as a
// second, independent safeguard — this constant here is just what
// gets shown/sent, not the only thing protecting the account.
// ------------------------------------------------------------
const PROTECTED_EMAIL = 'lourensvdbijl@gmail.com'
const CONFIRM_PHRASE = 'DELETE ALL'

const showClearModal = ref(false)
const clearConfirmText = ref('')
const isClearing = ref(false)
const clearResult = ref(null) // { success, message, firebaseAuthDeleted, firestoreDeleted, postgresDeleted, protectedEmails, errors }

const clearConfirmMatches = computed(() => clearConfirmText.value.trim() === CONFIRM_PHRASE)

function openClearModal() {
  clearConfirmText.value = ''
  clearResult.value = null
  showClearModal.value = true
}

function closeClearModal() {
  if (isClearing.value) return
  showClearModal.value = false
}

async function handleClearAll() {
  if (!clearConfirmMatches.value || isClearing.value) return

  isClearing.value = true
  clearResult.value = null

  try {
    clearResult.value = await clearAllTestData(PROTECTED_EMAIL)
  } catch (err) {
    clearResult.value = { success: false, message: err.message || 'Clear failed.' }
  } finally {
    isClearing.value = false
  }
}
</script>

<template>
  <div class="test-data-view">
    <div class="intro">
      <p>
        Upload a JSON test-data file (groups, each with one owner and a list of members) to batch-create
        real accounts across Firebase Auth, Firestore, and Postgres — the whole file is validated before
        anything is written, so a bad file is rejected with a full list of problems, not a partial import.
      </p>
    </div>

    <input
      ref="fileInput"
      type="file"
      accept="application/json,.json"
      class="hidden-input"
      @change="handleFileChange"
    />

    <div class="upload-row">
      <button class="upload-btn" :disabled="isImporting" @click="triggerFilePicker">
        <UploadCloud size="14" />
        Choose JSON file
      </button>

      <span v-if="fileName" class="file-name">
        <FileJson size="13" />
        {{ fileName }}
      </span>

      <button v-if="fileName" class="reset-btn" :disabled="isImporting" @click="resetSelection">
        <RotateCcw size="12" />
        Clear
      </button>
    </div>

    <div v-if="parseError" class="banner error">
      <XCircle size="14" />
      {{ parseError }}
    </div>

    <div v-if="parsedPayload && !parseError" class="preview">
      <div class="preview-summary">
        <div class="preview-stat">
          <span class="preview-stat-value">{{ groupCount }}</span>
          <span class="preview-stat-label">group{{ groupCount === 1 ? '' : 's' }}</span>
        </div>
        <div class="preview-stat">
          <span class="preview-stat-value">{{ userCount }}</span>
          <span class="preview-stat-label">user{{ userCount === 1 ? '' : 's' }}</span>
        </div>
        <div v-if="parsedPayload.source" class="preview-source">Source: {{ parsedPayload.source }}</div>
      </div>

      <div class="preview-table-wrap">
        <table class="preview-table">
          <thead>
            <tr>
              <th>Group</th>
              <th>Type</th>
              <th>Owner</th>
              <th>Members</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="group in parsedPayload.groups" :key="group.groupId">
              <td>{{ group.groupName || group.groupId }}</td>
              <td>{{ group.groupType }}</td>
              <td>{{ group.owner?.email || '—' }}</td>
              <td>{{ group.members?.length ?? 0 }}</td>
            </tr>
          </tbody>
        </table>
      </div>

      <button class="import-btn" :disabled="isImporting" @click="handleImport">
        {{ isImporting ? 'Importing…' : `Import ${groupCount} group${groupCount === 1 ? '' : 's'} / ${userCount} user${userCount === 1 ? '' : 's'}` }}
      </button>
    </div>

    <div v-if="result" class="banner" :class="result.success ? 'success' : 'error'">
      <CheckCircle2 v-if="result.success" size="14" />
      <XCircle v-else size="14" />
      <div class="banner-body">
        <p class="banner-message">{{ result.message }}</p>

        <ul v-if="result.validationErrors?.length" class="error-list">
          <li v-for="(err, i) in result.validationErrors" :key="i">
            <AlertTriangle size="11" />
            {{ err }}
          </li>
        </ul>
      </div>
    </div>

    <!-- ================================================================ -->
    <!-- DANGER ZONE — clear all test users                               -->
    <!-- ================================================================ -->
    <div class="danger-zone">
      <div class="danger-header">
        <ShieldAlert size="14" />
        <div>
          <p class="danger-title">Clear All Test Data</p>
          <p class="danger-subtitle">
            Deletes every user row from Firebase Auth, Firestore, and Postgres — except
            <strong>{{ PROTECTED_EMAIL }}</strong>, which is always kept, no matter what.
            Groups aren't touched by this (a group whose owner gets removed will be left pointing at nothing).
          </p>
        </div>
      </div>
      <button class="clear-btn" @click="openClearModal">
        <Trash2 size="13" />
        Clear All Test Data
      </button>

      <div v-if="clearResult" class="banner" :class="clearResult.success ? 'success' : 'error'">
        <CheckCircle2 v-if="clearResult.success" size="14" />
        <XCircle v-else size="14" />
        <div class="banner-body">
          <p class="banner-message">{{ clearResult.message }}</p>
          <ul v-if="clearResult.errors?.length" class="error-list">
            <li v-for="(err, i) in clearResult.errors" :key="i">
              <AlertTriangle size="11" />
              {{ err }}
            </li>
          </ul>
        </div>
      </div>
    </div>

    <!-- Confirm modal -->
    <div v-if="showClearModal" class="modal-overlay" @click.self="closeClearModal">
      <div class="modal">
        <h3>Clear All Test Data?</h3>
        <p class="modal-subtitle">
          This permanently deletes every user from Firebase Auth, Firestore, and Postgres except
          <strong>{{ PROTECTED_EMAIL }}</strong>. This cannot be undone.
        </p>

        <label class="confirm-label">
          Type <strong>{{ CONFIRM_PHRASE }}</strong> to confirm
          <input v-model="clearConfirmText" type="text" :disabled="isClearing" autocomplete="off" />
        </label>

        <div class="modal-actions">
          <button class="btn-outline" :disabled="isClearing" @click="closeClearModal">Cancel</button>
          <button class="clear-btn" :disabled="!clearConfirmMatches || isClearing" @click="handleClearAll">
            {{ isClearing ? 'Clearing…' : 'Delete Everyone Else' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.test-data-view {
  flex: 1;
  min-height: 0;
  overflow-y: auto;
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.intro p {
  font-size: 0.72rem;
  color: #64748B;
  margin: 0;
  max-width: 720px;
  line-height: 1.5;
}

.hidden-input {
  display: none;
}

.upload-row {
  display: flex;
  align-items: center;
  gap: 10px;
  flex-wrap: wrap;
}

.upload-btn,
.import-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-size: 0.72rem;
  font-weight: 600;
  padding: 7px 12px;
  border-radius: 6px;
  border: none;
  background: #2563EB;
  color: #fff;
  cursor: pointer;
}

.upload-btn:disabled,
.import-btn:disabled {
  background: #93C5FD;
  cursor: not-allowed;
}

.import-btn {
  align-self: flex-start;
  background: #16A34A;
}

.import-btn:disabled {
  background: #86EFAC;
}

.file-name {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  font-size: 0.72rem;
  color: #334155;
  font-weight: 600;
}

.reset-btn {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  font-size: 0.68rem;
  font-weight: 600;
  color: #64748B;
  background: none;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 4px 8px;
  cursor: pointer;
}

.reset-btn:disabled {
  cursor: not-allowed;
  opacity: 0.5;
}

.preview {
  display: flex;
  flex-direction: column;
  gap: 10px;
  border: 1px solid #E2E8F0;
  border-radius: 8px;
  padding: 12px;
  background: #fff;
}

.preview-summary {
  display: flex;
  align-items: baseline;
  gap: 20px;
}

.preview-stat {
  display: flex;
  align-items: baseline;
  gap: 5px;
}

.preview-stat-value {
  font-size: 1.1rem;
  font-weight: 800;
  color: #0F172A;
}

.preview-stat-label {
  font-size: 0.68rem;
  color: #64748B;
}

.preview-source {
  font-size: 0.66rem;
  color: #94A3B8;
  margin-left: auto;
}

.preview-table-wrap {
  max-height: 280px;
  overflow-y: auto;
  border: 1px solid #F1F5F9;
  border-radius: 6px;
}

.preview-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.7rem;
}

.preview-table th {
  position: sticky;
  top: 0;
  background: #F8FAFC;
  text-align: left;
  padding: 6px 10px;
  color: #64748B;
  font-weight: 700;
  border-bottom: 1px solid #E2E8F0;
}

.preview-table td {
  padding: 5px 10px;
  border-bottom: 1px solid #F1F5F9;
  color: #334155;
}

.banner {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  padding: 10px 12px;
  border-radius: 8px;
  font-size: 0.72rem;
  line-height: 1.5;
}

.banner.success {
  background: #F0FDF4;
  color: #15803D;
  border: 1px solid #BBF7D0;
}

.banner.error {
  background: #FEF2F2;
  color: #B91C1C;
  border: 1px solid #FECACA;
}

.banner-body {
  flex: 1;
  min-width: 0;
}

.banner-message {
  margin: 0;
  font-weight: 600;
}

.error-list {
  margin: 6px 0 0;
  padding: 0;
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 3px;
  max-height: 220px;
  overflow-y: auto;
}

.error-list li {
  display: flex;
  align-items: flex-start;
  gap: 5px;
  font-size: 0.68rem;
  font-weight: 400;
}

/* ---------- Danger zone ---------- */
.danger-zone {
  margin-top: 8px;
  padding: 14px;
  border: 1px solid #FECACA;
  background: #FEF2F2;
  border-radius: 8px;
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.danger-header {
  display: flex;
  align-items: flex-start;
  gap: 8px;
  color: #B91C1C;
}

.danger-title {
  margin: 0 0 2px;
  font-size: 0.78rem;
  font-weight: 700;
  color: #991B1B;
}

.danger-subtitle {
  margin: 0;
  font-size: 0.68rem;
  color: #B91C1C;
  line-height: 1.5;
  max-width: 640px;
}

.clear-btn {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  align-self: flex-start;
  font-size: 0.72rem;
  font-weight: 700;
  padding: 7px 12px;
  border-radius: 6px;
  border: none;
  background: #DC2626;
  color: #fff;
  cursor: pointer;
}

.clear-btn:disabled {
  background: #FCA5A5;
  cursor: not-allowed;
}

/* ---------- Confirm modal ---------- */
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.45);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 50;
}

.modal {
  background: #fff;
  border-radius: 10px;
  padding: 20px;
  width: 460px;
  max-width: 92vw;
}

.modal h3 {
  margin: 0 0 4px;
  font-size: 0.95rem;
  color: #0F172A;
}

.modal-subtitle {
  font-size: 0.72rem;
  color: #64748B;
  margin: 0 0 14px;
  line-height: 1.5;
}

.confirm-label {
  display: flex;
  flex-direction: column;
  gap: 6px;
  font-size: 0.72rem;
  font-weight: 600;
  color: #334155;
}

.confirm-label input {
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 7px 9px;
  font-size: 0.78rem;
  font-weight: 400;
  color: #0F172A;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}

.btn-outline {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 600;
  padding: 6px 10px;
  cursor: pointer;
  background: #fff;
  border: 1px solid #E2E8F0;
  color: #334155;
}

.btn-outline:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>