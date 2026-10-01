<script setup>
// CA1 detail content — shown inside DashboardDetailDrawer when a row is
// clicked. Full context (who initiated it, when, why) plus the actions,
// none of which are visible in the compact row.
import { ref } from 'vue'
import { ArrowRight } from 'lucide-vue-next'
import ActionButton from './ActionButton.vue'

const props = defineProps({
  change: { type: Object, required: true }
})

const emit = defineEmits(['approve', 'question'])

const showCommentField = ref(false)
const comment = ref('')

function submitQuestion() {
  if (!comment.value.trim()) return
  emit('question', { id: props.change.id, comment: comment.value.trim() })
  showCommentField.value = false
  comment.value = ''
}

function formatCurrency(value) {
  return `R${value.toLocaleString('en-ZA')}`
}
function formatDate(iso) {
  return new Date(iso).toLocaleString('en-ZA', { day: 'numeric', month: 'short', hour: '2-digit', minute: '2-digit' })
}
</script>

<template>
  <div class="detail-block">
    <span class="status-pill pending">{{ change.status }}</span>

    <h3 class="detail-title">{{ change.obligationName }}</h3>

    <div class="figures-block">
      <div class="figure">
        <span class="figure-label">Old contribution</span>
        <span class="figure-value old">{{ formatCurrency(change.oldContribution) }}</span>
      </div>
      <ArrowRight :size="16" class="figures-arrow" />
      <div class="figure">
        <span class="figure-label">New contribution</span>
        <span class="figure-value new">{{ formatCurrency(change.newContribution) }}</span>
      </div>
    </div>

    <dl class="meta-list">
      <div>
        <dt>Responsible person</dt>
        <dd>{{ change.responsiblePerson }}</dd>
      </div>
      <div>
        <dt>Initiated by</dt>
        <dd>{{ change.initiatedBy }}</dd>
      </div>
      <div>
        <dt>Requested</dt>
        <dd>{{ formatDate(change.requestedAt) }}</dd>
      </div>
    </dl>

    <div class="reason-block">
      <span class="reason-label">Reason</span>
      <p>{{ change.reason }}</p>
    </div>

    <div v-if="!showCommentField" class="detail-actions">
      <ActionButton variant="success" @click="emit('approve', change.id)">Approve</ActionButton>
      <ActionButton variant="neutral" @click="showCommentField = true">Question</ActionButton>
    </div>

    <div v-else class="comment-block">
      <textarea v-model="comment" rows="2" placeholder="What would you like to ask? (required)"></textarea>
      <div class="comment-actions">
        <ActionButton variant="neutral" @click="showCommentField = false">Cancel</ActionButton>
        <ActionButton variant="primary" :disabled="!comment.trim()" @click="submitQuestion">
          Submit question
        </ActionButton>
      </div>
    </div>
  </div>
</template>

<style scoped>
.detail-block {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.status-pill {
  align-self: flex-start;
  font-size: 0.66rem;
  font-weight: 700;
  padding: 3px 9px;
  border-radius: 999px;
}

.status-pill.pending {
  background: #fff7ed;
  color: #c2410c;
  border: 1px solid #fed7aa;
}

.detail-title {
  margin: 0;
  font-size: 1.05rem;
  font-weight: 800;
  color: #0f172a;
}

.figures-block {
  display: flex;
  align-items: center;
  gap: 14px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 12px 14px;
}

.figure {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 3px;
}

.figure-label {
  font-size: 0.66rem;
  color: #94a3b8;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.figure-value {
  font-size: 1.1rem;
  font-weight: 800;
}

.figure-value.old {
  color: #94a3b8;
  text-decoration: line-through;
}

.figure-value.new {
  color: #0f172a;
}

.figures-arrow {
  color: #cbd5e1;
  flex-shrink: 0;
}

.meta-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  margin: 0;
}

.meta-list > div {
  display: flex;
  justify-content: space-between;
  border-bottom: 1px solid #f1f5f9;
  padding-bottom: 8px;
}

.meta-list dt {
  font-size: 0.78rem;
  color: #64748b;
}

.meta-list dd {
  margin: 0;
  font-size: 0.78rem;
  font-weight: 700;
  color: #0f172a;
}

.reason-block {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.reason-label {
  font-size: 0.66rem;
  color: #94a3b8;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.02em;
}

.reason-block p {
  margin: 0;
  font-size: 0.82rem;
  color: #334155;
  line-height: 1.5;
}

.detail-actions {
  display: flex;
  gap: 8px;
  padding-top: 4px;
}

.detail-actions :deep(.action-btn) {
  flex: 1;
  justify-content: center;
}

.comment-block {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.comment-block textarea {
  width: 100%;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 9px 11px;
  font-family: inherit;
  font-size: 0.82rem;
  color: #0f172a;
  resize: vertical;
  outline: none;
}

.comment-block textarea:focus {
  border-color: #1855b9;
}

.comment-actions {
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>
