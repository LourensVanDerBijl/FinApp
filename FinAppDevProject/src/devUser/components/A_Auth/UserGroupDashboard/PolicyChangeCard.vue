<script setup>
// CA2 detail content — "who requested what, when, the impact, actions,
// and comments," per the brief, shown when the compact summary row is
// clicked. Impact is shown both at the group level (old/new total) and
// personally (this user's own old/new share).
import { ref } from 'vue'
import ActionButton from './ActionButton.vue'

const props = defineProps({
  proposal: { type: Object, required: true }
})

const emit = defineEmits(['approve', 'decline', 'question'])

const showCommentField = ref(false)
const comment = ref('')

function submitQuestion() {
  if (!comment.value.trim()) return
  emit('question', { id: props.proposal.id, comment: comment.value.trim() })
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
    <span class="status-pill pending">{{ proposal.status }}</span>

    <h3 class="detail-title">{{ proposal.purpose }}</h3>

    <dl class="meta-list">
      <div>
        <dt>Requested by</dt>
        <dd>{{ proposal.proposedBy }} (Owner)</dd>
      </div>
      <div>
        <dt>When</dt>
        <dd>{{ formatDate(proposal.proposedAt) }}</dd>
      </div>
    </dl>

    <div class="reason-block">
      <span class="reason-label">Owner's comment</span>
      <p>"{{ proposal.ownerComment }}"</p>
    </div>

    <div class="impact-block">
      <span class="reason-label">Impact — Group Total</span>
      <div class="impact-row">
        <span class="old">{{ formatCurrency(proposal.oldTotalContribution) }}</span>
        <span class="arrow">→</span>
        <span class="new">{{ formatCurrency(proposal.newTotalContribution) }}</span>
      </div>

      <span class="reason-label">Impact — Your Contribution</span>
      <div class="impact-row">
        <span class="old">{{ formatCurrency(proposal.myOldShare) }}</span>
        <span class="arrow">→</span>
        <span class="new">{{ formatCurrency(proposal.myNewShare) }}</span>
      </div>
    </div>

    <div v-if="proposal.comments?.length" class="comments-thread">
      <span class="reason-label">Comments</span>
      <div v-for="(c, i) in proposal.comments" :key="i" class="comment-item">
        <span class="comment-author">{{ c.author }}</span>
        <span class="comment-time">{{ formatDate(c.at) }}</span>
        <p>{{ c.text }}</p>
      </div>
    </div>

    <div v-if="!showCommentField" class="detail-actions">
      <ActionButton variant="success" @click="emit('approve', proposal.id)">Approve</ActionButton>
      <ActionButton variant="neutral" @click="showCommentField = true">Question</ActionButton>
      <ActionButton variant="danger" @click="emit('decline', proposal.id)">Decline</ActionButton>
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
  background: #fff7ed;
  color: #c2410c;
  border: 1px solid #fed7aa;
}

.detail-title {
  margin: 0;
  font-size: 1.02rem;
  font-weight: 800;
  color: #0f172a;
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
  font-style: italic;
  line-height: 1.5;
}

.impact-block {
  display: flex;
  flex-direction: column;
  gap: 6px;
  background: #f8fafc;
  border: 1px solid #e2e8f0;
  border-radius: 10px;
  padding: 12px 14px;
}

.impact-row {
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 0.88rem;
  margin-bottom: 4px;
}

.impact-row .old {
  color: #94a3b8;
  text-decoration: line-through;
}

.impact-row .new {
  color: #0f172a;
  font-weight: 800;
}

.impact-row .arrow {
  color: #cbd5e1;
}

.comments-thread {
  display: flex;
  flex-direction: column;
  gap: 6px;
}

.comment-item {
  border-left: 2px solid #e2e8f0;
  padding-left: 10px;
}

.comment-author {
  font-size: 0.76rem;
  font-weight: 700;
  color: #0f172a;
  margin-right: 6px;
}

.comment-time {
  font-size: 0.68rem;
  color: #94a3b8;
}

.comment-item p {
  margin: 3px 0 0;
  font-size: 0.8rem;
  color: #334155;
  line-height: 1.45;
}

.detail-actions {
  display: flex;
  gap: 6px;
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
