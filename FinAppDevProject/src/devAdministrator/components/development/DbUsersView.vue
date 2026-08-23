<script setup>
import { ref, computed, onMounted } from 'vue'
import { fetchDbUsers, addDbUser, deleteDbUser } from '../../data/adminDatabaseView.js'
import {
  Search,
  RefreshCw,
  UserPlus,
  Trash2,
  ShieldCheck,
  Flame,
  Layers,
  AlertTriangle,
  Eye
} from 'lucide-vue-next'

// ------------------------------------------------------------
// Load
// ------------------------------------------------------------
const rows = ref([])
const loading = ref(false)
const loadError = ref('')

async function loadRows() {
  loading.value = true
  loadError.value = ''
  try {
    const data = await fetchDbUsers()
    if (data.success) {
      rows.value = data.rows || []
    } else {
      loadError.value = data.message || 'Failed to load database view.'
    }
  } catch (err) {
    loadError.value = err.message || 'Failed to load database view.'
  } finally {
    loading.value = false
  }
}

onMounted(loadRows)

// ------------------------------------------------------------
// View mode — Default (status summary) or drill into one source
// ------------------------------------------------------------
const viewMode = ref('default')

const viewOptions = [
  { value: 'default', label: 'Default (status summary)' },
  { value: 'firebase', label: 'Firebase Auth' },
  { value: 'firestore', label: 'Firestore' },
  { value: 'postgres', label: 'Postgres' }
]

// ------------------------------------------------------------
// Search
// ------------------------------------------------------------
const searchTerm = ref('')

const filteredRows = computed(() => {
  const term = searchTerm.value.trim().toLowerCase()
  if (!term) return rows.value

  return rows.value.filter((r) => {
    return [
      r.rowKey,
      r.firebaseEmail,
      r.firestoreAccountEmail,
      r.firestoreDisplayName,
      r.firestoreFirstName,
      r.firestoreLastName,
      r.postgresPreferName,
      r.postgresLastName,
      r.firebaseUid
    ]
      .filter(Boolean)
      .some((v) => v.toLowerCase().includes(term))
  })
})

// ------------------------------------------------------------
// Summary counts
// ------------------------------------------------------------
const summary = computed(() => {
  const total = rows.value.length
  const inFirebase = rows.value.filter((r) => r.firebaseAuthExists).length
  const inFirestore = rows.value.filter((r) => r.firestoreExists).length
  const inPostgres = rows.value.filter((r) => r.postgresExists).length
  const orphans = rows.value.filter(
    (r) => !(r.firebaseAuthExists && r.firestoreExists && r.postgresExists)
  ).length
  return { total, inFirebase, inFirestore, inPostgres, orphans }
})

// ------------------------------------------------------------
// Default view — per-source status (Missing / Mismatch / OK)
// ------------------------------------------------------------
function emailsMismatch(row) {
  if (!row.firebaseAuthExists || !row.firestoreExists) return false
  const a = (row.firebaseEmail || '').trim().toLowerCase()
  const b = (row.firestoreAccountEmail || '').trim().toLowerCase()
  return Boolean(a && b && a !== b)
}

function sourceStatus(row, source) {
  if (source === 'firebase') {
    if (!row.firebaseAuthExists) return { label: 'Missing', cls: 'badge-red' }
    if (emailsMismatch(row)) return { label: 'Mismatch', cls: 'badge-amber' }
    return { label: 'OK', cls: 'badge-green' }
  }
  if (source === 'firestore') {
    if (!row.firestoreExists) return { label: 'Missing', cls: 'badge-red' }
    if (emailsMismatch(row)) return { label: 'Mismatch', cls: 'badge-amber' }
    return { label: 'OK', cls: 'badge-green' }
  }
  if (source === 'postgres') {
    if (!row.postgresExists) return { label: 'Missing', cls: 'badge-red' }
    return { label: 'OK', cls: 'badge-green' }
  }
  return { label: '—', cls: 'badge-neutral' }
}

// ------------------------------------------------------------
// Formatting
// ------------------------------------------------------------
function formatDate(isoString) {
  if (!isoString) return '—'
  const d = new Date(isoString)
  if (isNaN(d.getTime())) return isoString
  return d.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' })
}

// ------------------------------------------------------------
// Delete
// ------------------------------------------------------------
const deletingKey = ref(null)

async function handleDelete(row) {
  const parts = []
  if (row.firebaseAuthExists) parts.push('Firebase Authentication')
  if (row.firestoreExists) parts.push('Firestore (fb_users)')
  if (row.postgresExists) parts.push('Postgres (Users)')

  if (parts.length === 0) return

  const confirmed = confirm(
    `Delete "${row.rowKey}"?\n\nThis will remove the record from:\n- ${parts.join('\n- ')}\n\nThis cannot be undone.`
  )
  if (!confirmed) return

  deletingKey.value = row.rowKey
  try {
    const result = await deleteDbUser({
      userId: row.firestoreUserId || row.postgresUserId || null,
      firebaseUid: row.firebaseUid || null
    })

    if (result.success) {
      rows.value = rows.value.filter((r) => r.rowKey !== row.rowKey)
    } else {
      alert(`Delete did not fully complete: ${result.message || 'Unknown error.'}`)
    }
  } catch (err) {
    alert(`Delete failed: ${err.message}`)
  } finally {
    deletingKey.value = null
  }
}

// ------------------------------------------------------------
// Add modal
// ------------------------------------------------------------
const showAddModal = ref(false)
const adding = ref(false)
const addError = ref('')

const newUser = ref({
  email: '',
  password: '',
  displayName: '',
  firstName: '',
  lastName: '',
  accountType: 'Free',
  country: '',
  currency: '',
  timezone: '',
  dateOfBirth: ''
})

function openAddModal() {
  newUser.value = {
    email: '',
    password: '',
    displayName: '',
    firstName: '',
    lastName: '',
    accountType: 'Free',
    country: '',
    currency: '',
    timezone: '',
    dateOfBirth: ''
  }
  addError.value = ''
  showAddModal.value = true
}

function closeAddModal() {
  showAddModal.value = false
}

async function submitAddUser() {
  if (!newUser.value.email) {
    addError.value = 'Email is required.'
    return
  }

  adding.value = true
  addError.value = ''
  try {
    const result = await addDbUser({
      email: newUser.value.email,
      password: newUser.value.password || null,
      displayName: newUser.value.displayName,
      firstName: newUser.value.firstName,
      lastName: newUser.value.lastName,
      accountType: newUser.value.accountType,
      country: newUser.value.country,
      currency: newUser.value.currency,
      timezone: newUser.value.timezone,
      dateOfBirth: newUser.value.dateOfBirth || '1990-01-01'
    })

    if (result.success) {
      showAddModal.value = false
      if (result.generatedPassword) {
        alert(`User "${result.userId}" created.\n\nGenerated password (shown once):\n${result.generatedPassword}`)
      }
      await loadRows()
    } else {
      addError.value = result.message || 'Failed to add user.'
    }
  } catch (err) {
    addError.value = err.message || 'Failed to add user.'
  } finally {
    adding.value = false
  }
}
</script>

<template>
  <div class="db-users-view">
    <!-- Toolbar -->
    <div class="toolbar">
      <div class="toolbar-left">
        <div class="search-box">
          <Search size="12" class="search-icon" />
          <input v-model="searchTerm" type="text" placeholder="Search by ID, email, or name..." />
        </div>

        <div class="view-select">
          <Eye size="12" class="view-icon" />
          <select v-model="viewMode">
            <option v-for="opt in viewOptions" :key="opt.value" :value="opt.value">{{ opt.label }}</option>
          </select>
        </div>
      </div>

      <div class="toolbar-actions">
        <button class="btn-outline" :disabled="loading" @click="loadRows">
          <RefreshCw size="12" :class="{ spinning: loading }" /> Refresh
        </button>
        <button class="btn-primary" @click="openAddModal">
          <UserPlus size="12" /> Add User
        </button>
      </div>
    </div>

    <!-- Summary chips -->
    <div class="summary-row">
      <div class="summary-chip">
        <Layers size="12" /> {{ summary.total }} total rows
      </div>
      <div class="summary-chip">
        <ShieldCheck size="12" class="ic-blue" /> {{ summary.inFirebase }} in Firebase Auth
      </div>
      <div class="summary-chip">
        <Flame size="12" class="ic-amber" /> {{ summary.inFirestore }} in Firestore
      </div>
      <div class="summary-chip">
        <Layers size="12" class="ic-purple" /> {{ summary.inPostgres }} in Postgres
      </div>
      <div class="summary-chip" :class="{ warn: summary.orphans > 0 }">
        <AlertTriangle size="12" /> {{ summary.orphans }} incomplete
      </div>
    </div>

    <p v-if="loadError" class="error-banner">{{ loadError }}</p>

    <!-- Table -->
    <div class="table-scroll">

      <!-- ============================================================ -->
      <!-- DEFAULT VIEW — User ID + a status badge per source            -->
      <!-- ============================================================ -->
      <table v-if="viewMode === 'default'" class="db-table">
        <thead>
          <tr>
            <th class="col-key">User ID</th>
            <th class="center">Firebase Auth</th>
            <th class="center">Firestore</th>
            <th class="center">Postgres</th>
            <th class="col-actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in filteredRows" :key="row.rowKey">
            <td class="col-key">{{ row.rowKey }}</td>
            <td class="center">
              <span class="badge" :class="sourceStatus(row, 'firebase').cls">{{ sourceStatus(row, 'firebase').label }}</span>
            </td>
            <td class="center">
              <span class="badge" :class="sourceStatus(row, 'firestore').cls">{{ sourceStatus(row, 'firestore').label }}</span>
            </td>
            <td class="center">
              <span class="badge" :class="sourceStatus(row, 'postgres').cls">{{ sourceStatus(row, 'postgres').label }}</span>
            </td>
            <td class="col-actions">
              <button
                class="btn-icon-danger"
                :disabled="deletingKey === row.rowKey"
                title="Delete from all tables"
                @click="handleDelete(row)"
              >
                <Trash2 size="13" />
              </button>
            </td>
          </tr>
          <tr v-if="!loading && filteredRows.length === 0">
            <td class="no-results" colspan="5">No matching records.</td>
          </tr>
        </tbody>
      </table>

      <!-- ============================================================ -->
      <!-- FIREBASE AUTH VIEW                                            -->
      <!-- ============================================================ -->
      <table v-else-if="viewMode === 'firebase'" class="db-table">
        <thead>
          <tr>
            <th class="col-key">User ID</th>
            <th>Email</th>
            <th>UID</th>
            <th>Verified</th>
            <th>Disabled</th>
            <th>Created</th>
            <th>Last Sign-in</th>
            <th class="col-actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in filteredRows" :key="row.rowKey">
            <td class="col-key">{{ row.rowKey }}</td>
            <template v-if="row.firebaseAuthExists">
              <td>{{ row.firebaseEmail || '—' }}</td>
              <td class="mono">{{ row.firebaseUid }}</td>
              <td>
                <span class="badge" :class="row.firebaseEmailVerified ? 'badge-green' : 'badge-neutral'">
                  {{ row.firebaseEmailVerified ? 'Yes' : 'No' }}
                </span>
              </td>
              <td>
                <span class="badge" :class="row.firebaseDisabled ? 'badge-red' : 'badge-green'">
                  {{ row.firebaseDisabled ? 'Yes' : 'No' }}
                </span>
              </td>
              <td>{{ formatDate(row.firebaseCreatedAt) }}</td>
              <td>{{ formatDate(row.firebaseLastSignIn) }}</td>
            </template>
            <td v-else colspan="6" class="missing">Not in Firebase Auth</td>
            <td class="col-actions">
              <button
                class="btn-icon-danger"
                :disabled="deletingKey === row.rowKey"
                title="Delete from all tables"
                @click="handleDelete(row)"
              >
                <Trash2 size="13" />
              </button>
            </td>
          </tr>
          <tr v-if="!loading && filteredRows.length === 0">
            <td class="no-results" colspan="8">No matching records.</td>
          </tr>
        </tbody>
      </table>

      <!-- ============================================================ -->
      <!-- FIRESTORE VIEW                                                -->
      <!-- ============================================================ -->
      <table v-else-if="viewMode === 'firestore'" class="db-table">
        <thead>
          <tr>
            <th class="col-key">User ID</th>
            <th>Display Name</th>
            <th>Account Email</th>
            <th>Account Type</th>
            <th>Status</th>
            <th>Group</th>
            <th>Country</th>
            <th>Joined</th>
            <th class="col-actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in filteredRows" :key="row.rowKey">
            <td class="col-key">{{ row.rowKey }}</td>
            <template v-if="row.firestoreExists">
              <td>{{ row.firestoreDisplayName || '—' }}</td>
              <td>{{ row.firestoreAccountEmail || '—' }}</td>
              <td><span class="badge badge-blue">{{ row.firestoreAccountType || '—' }}</span></td>
              <td><span class="badge badge-green">{{ row.firestoreMemberStatus || '—' }}</span></td>
              <td>{{ row.firestoreGroupName || '—' }}</td>
              <td>{{ row.firestoreCountry || '—' }}</td>
              <td>{{ formatDate(row.firestoreJoinedAt) }}</td>
            </template>
            <td v-else colspan="7" class="missing">Not in Firestore</td>
            <td class="col-actions">
              <button
                class="btn-icon-danger"
                :disabled="deletingKey === row.rowKey"
                title="Delete from all tables"
                @click="handleDelete(row)"
              >
                <Trash2 size="13" />
              </button>
            </td>
          </tr>
          <tr v-if="!loading && filteredRows.length === 0">
            <td class="no-results" colspan="9">No matching records.</td>
          </tr>
        </tbody>
      </table>

      <!-- ============================================================ -->
      <!-- POSTGRES VIEW                                                 -->
      <!-- ============================================================ -->
      <table v-else class="db-table">
        <thead>
          <tr>
            <th class="col-key">User ID</th>
            <th>Prefer Name</th>
            <th>Last Name</th>
            <th>DOB</th>
            <th>Account Type</th>
            <th>Group ID</th>
            <th>Created</th>
            <th class="col-actions">Actions</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="row in filteredRows" :key="row.rowKey">
            <td class="col-key">{{ row.rowKey }}</td>
            <template v-if="row.postgresExists">
              <td>{{ row.postgresPreferName || '—' }}</td>
              <td>{{ row.postgresLastName || '—' }}</td>
              <td>{{ row.postgresDateOfBirth || '—' }}</td>
              <td><span class="badge badge-blue">{{ row.postgresAccountType || '—' }}</span></td>
              <td>{{ row.postgresGroupId || '—' }}</td>
              <td>{{ formatDate(row.postgresCreatedAt) }}</td>
            </template>
            <td v-else colspan="6" class="missing">Not in Postgres</td>
            <td class="col-actions">
              <button
                class="btn-icon-danger"
                :disabled="deletingKey === row.rowKey"
                title="Delete from all tables"
                @click="handleDelete(row)"
              >
                <Trash2 size="13" />
              </button>
            </td>
          </tr>
          <tr v-if="!loading && filteredRows.length === 0">
            <td class="no-results" colspan="8">No matching records.</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Add User modal -->
    <div v-if="showAddModal" class="modal-overlay" @click.self="closeAddModal">
      <div class="modal">
        <h3>Add User (test data)</h3>
        <p class="modal-subtitle">
          Writes a real row into Firebase Auth, Firestore and Postgres — for seeding test data, not the public signup flow.
        </p>

        <div class="form-grid">
          <label>Email *<input v-model="newUser.email" type="email" placeholder="test.user@example.com" /></label>
          <label>Password (optional)<input v-model="newUser.password" type="text" placeholder="Leave blank to auto-generate" /></label>
          <label>Display Name<input v-model="newUser.displayName" type="text" /></label>
          <label>First Name<input v-model="newUser.firstName" type="text" /></label>
          <label>Last Name<input v-model="newUser.lastName" type="text" /></label>
          <label>Account Type
            <select v-model="newUser.accountType">
              <option value="Free">Free</option>
              <option value="Premium">Premium</option>
            </select>
          </label>
          <label>Country<input v-model="newUser.country" type="text" /></label>
          <label>Currency<input v-model="newUser.currency" type="text" /></label>
          <label>Timezone<input v-model="newUser.timezone" type="text" /></label>
          <label>Date of Birth<input v-model="newUser.dateOfBirth" type="date" /></label>
        </div>

        <p v-if="addError" class="error-banner">{{ addError }}</p>

        <div class="modal-actions">
          <button class="btn-outline" @click="closeAddModal">Cancel</button>
          <button class="btn-primary" :disabled="adding" @click="submitAddUser">
            {{ adding ? 'Adding...' : 'Add to all 3 tables' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.db-users-view {
  padding: 12px 16px 16px;
  font-size: 13px;
  display: flex;
  flex-direction: column;
  gap: 10px;
  flex: 1;
  min-height: 0;
}

/* ---------- Toolbar ---------- */
.toolbar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 12px;
  flex-wrap: wrap;
}

.toolbar-left {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
  align-items: center;
}

.search-box {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 5px 10px;
  min-width: 260px;
}

.search-box input {
  border: none;
  outline: none;
  font-size: 0.7rem;
  flex: 1;
  color: #0F172A;
}

.search-icon, .view-icon { color: #94A3B8; }

.view-select {
  display: flex;
  align-items: center;
  gap: 6px;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 5px 10px;
}

.view-select select {
  border: none;
  outline: none;
  font-size: 0.7rem;
  font-weight: 600;
  color: #0F172A;
  background: transparent;
}

.toolbar-actions { display: flex; gap: 8px; }

.btn-outline, .btn-primary {
  display: flex;
  align-items: center;
  gap: 5px;
  border-radius: 6px;
  font-size: 0.7rem;
  font-weight: 600;
  padding: 6px 10px;
  cursor: pointer;
  white-space: nowrap;
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

.btn-outline:disabled, .btn-primary:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.spinning { animation: spin 0.8s linear infinite; }
@keyframes spin { from { transform: rotate(0deg); } to { transform: rotate(360deg); } }

/* ---------- Summary chips ---------- */
.summary-row {
  display: flex;
  gap: 8px;
  flex-wrap: wrap;
}

.summary-chip {
  display: flex;
  align-items: center;
  gap: 5px;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 999px;
  padding: 4px 10px;
  font-size: 0.68rem;
  color: #334155;
  font-weight: 600;
}

.summary-chip.warn {
  border-color: #FDE68A;
  background: #FFFBEB;
  color: #92400E;
}

.ic-blue { color: #2563EB; }
.ic-amber { color: #D97706; }
.ic-purple { color: #7C3AED; }

.error-banner {
  background: #FEF2F2;
  border: 1px solid #FECACA;
  color: #B91C1C;
  padding: 6px 10px;
  border-radius: 6px;
  font-size: 0.7rem;
}

/* ---------- Table ---------- */
.table-scroll {
  flex: 1;
  min-height: 0;
  overflow: auto;
  background: #fff;
  border: 1px solid #E2E8F0;
  border-radius: 8px;
}

.db-table {
  border-collapse: collapse;
  width: 100%;
  font-size: 0.7rem;
}

.db-table th, .db-table td {
  padding: 7px 10px;
  border-bottom: 1px solid #F1F5F9;
  text-align: left;
  white-space: nowrap;
}

.db-table th.center, .db-table td.center {
  text-align: center;
}

.db-table thead th {
  position: sticky;
  top: 0;
  background: #F8FAFC;
  color: #475569;
  font-weight: 700;
  z-index: 1;
}

.col-key {
  position: sticky;
  left: 0;
  background: #fff;
  font-weight: 700;
  color: #0F172A;
  z-index: 2;
}

.db-table thead .col-key { background: #F8FAFC; z-index: 3; }

.mono { font-family: ui-monospace, monospace; font-size: 0.64rem; color: #64748B; }

.missing {
  text-align: center;
  color: #94A3B8;
  font-style: italic;
  background: #F8FAFC;
}

.col-actions {
  position: sticky;
  right: 0;
  background: #fff;
  text-align: center;
}

.db-table thead .col-actions { background: #F8FAFC; }

.btn-icon-danger {
  background: #FEF2F2;
  border: 1px solid #FECACA;
  color: #DC2626;
  border-radius: 6px;
  padding: 5px 7px;
  cursor: pointer;
  display: inline-flex;
}

.btn-icon-danger:disabled { opacity: 0.5; cursor: not-allowed; }

.no-results {
  text-align: center;
  padding: 20px;
  color: #94A3B8;
}

/* ---------- Badges ---------- */
.badge {
  display: inline-block;
  padding: 2px 8px;
  border-radius: 999px;
  font-size: 0.64rem;
  font-weight: 700;
}

.badge-green { background: #DCFCE7; color: #166534; }
.badge-red { background: #FEE2E2; color: #991B1B; }
.badge-amber { background: #FEF3C7; color: #92400E; }
.badge-blue { background: #DBEAFE; color: #1E40AF; }
.badge-neutral { background: #F1F5F9; color: #64748B; }

/* ---------- Add modal ---------- */
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
  width: 520px;
  max-width: 92vw;
  max-height: 88vh;
  overflow-y: auto;
}

.modal h3 {
  margin: 0 0 4px;
  font-size: 0.95rem;
  color: #0F172A;
}

.modal-subtitle {
  font-size: 0.68rem;
  color: #64748B;
  margin: 0 0 14px;
}

.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 10px;
}

.form-grid label {
  display: flex;
  flex-direction: column;
  gap: 4px;
  font-size: 0.68rem;
  font-weight: 600;
  color: #334155;
}

.form-grid input, .form-grid select {
  border: 1px solid #E2E8F0;
  border-radius: 6px;
  padding: 6px 8px;
  font-size: 0.72rem;
  font-weight: 400;
  color: #0F172A;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
  margin-top: 16px;
}
</style>
