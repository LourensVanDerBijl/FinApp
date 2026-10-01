<script setup>
import { computed } from 'vue'
import { Plus, UserX, Trash2, RotateCcw } from 'lucide-vue-next'

const props = defineProps({
  node: { type: Object, required: true },
  onAddClick: { type: Function, required: true },
  onTerminateClick: { type: Function, required: true },
  onRemoveClick: { type: Function, required: true },
  depth: { type: Number, default: 0 }
})

const displayName = computed(() =>
  [props.node.preferName, props.node.surname].filter(Boolean).join(' ')
)

const initials = computed(() => {
  const first = props.node.preferName?.[0] ?? ''
  const last = props.node.surname?.[0] ?? ''
  return (first + last).toUpperCase() || '?'
})

// Same idea as the avatar-color hash used in GroupMembersDrawer —
// stable per id, no lookup table to maintain.
const avatarPalette = ['#2563EB', '#0D9488', '#7C3AED', '#D97706', '#DC2626', '#0891B2']
const avatarColor = computed(() => {
  let hash = 0
  for (const char of props.node.id) hash = char.charCodeAt(0) + ((hash << 5) - hash)
  return avatarPalette[Math.abs(hash) % avatarPalette.length]
})

// Tier accent encodes org depth: root = brand teal, direct reports = blue,
// everyone deeper = neutral slate. Not decoration — it's a quick visual
// read of "how senior is this" as the tree grows.
const tierClass = computed(() => {
  if (props.depth === 0) return 'tier-root'
  if (props.depth === 1) return 'tier-exec'
  return 'tier-standard'
})

const isTerminated = computed(() => props.node.status === 'terminated')
const canRemove = computed(() => props.node.reports.length === 0)
</script>

<template>
  <div class="org-node">
    <div class="org-card-wrap">
      <div class="org-card" :class="[tierClass, { terminated: isTerminated }]">
        <span class="org-avatar" :style="{ backgroundColor: avatarColor }">{{ initials }}</span>
        <div class="org-card-text">
          <p class="org-name">{{ displayName }}</p>
          <p class="org-role">{{ node.role }}</p>
          <p v-if="isTerminated" class="org-terminated-badge">Terminated</p>
        </div>

        <div v-if="depth > 0" class="org-card-actions">
          <button
            class="org-icon-btn"
            :class="{ active: isTerminated }"
            :title="isTerminated ? `Reinstate ${displayName}` : `Terminate ${displayName}`"
            @click="onTerminateClick(node)"
          >
            <RotateCcw v-if="isTerminated" size="12" />
            <UserX v-else size="12" />
          </button>
          <button
            class="org-icon-btn danger"
            :disabled="!canRemove"
            :title="canRemove ? `Remove ${displayName}` : 'Reassign their direct reports before removing'"
            @click="onRemoveClick(node)"
          >
            <Trash2 size="12" />
          </button>
        </div>
      </div>

      <button
        v-if="!isTerminated"
        class="org-add-btn"
        @click="onAddClick(node)"
        :title="`Add a direct report to ${displayName}`"
      >
        <Plus size="12" />
      </button>
    </div>

    <div v-if="node.reports.length" class="org-children">
      <div class="org-children-connector"></div>
      <div class="org-children-row">
        <div v-for="child in node.reports" :key="child.id" class="org-child-branch">
          <OrgChartNode
            :node="child"
            :on-add-click="onAddClick"
            :on-terminate-click="onTerminateClick"
            :on-remove-click="onRemoveClick"
            :depth="depth + 1"
          />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.org-node {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.org-card-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.org-card {
  position: relative;
  background: #fff;
  border: 1px solid #E5E7EB;
  border-top: 3px solid #CBD5E1;
  border-radius: 12px;
  padding: 12px 18px;
  display: flex;
  align-items: center;
  gap: 11px;
  min-width: 200px;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
  transition: box-shadow 0.18s ease, transform 0.18s ease;
}

.org-card:hover {
  box-shadow: 0 10px 24px rgba(15, 23, 42, 0.10);
  transform: translateY(-2px);
}

.org-card.tier-root { border-top-color: #02b0a1; }
.org-card.tier-exec { border-top-color: #2563EB; }
.org-card.tier-standard { border-top-color: #94A3B8; }

.org-card.terminated {
  opacity: 0.6;
}

.org-card.terminated:hover {
  transform: none;
}

.org-avatar {
  width: 34px;
  height: 34px;
  border-radius: 50%;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #fff;
  font-size: 0.68rem;
  font-weight: 700;
  box-shadow: 0 0 0 3px #fff, 0 0 0 4px #F1F5F9;
}

.org-card-text {
  display: flex;
  flex-direction: column;
  gap: 1px;
  min-width: 0;
}

.org-name {
  font-size: 0.78rem;
  font-weight: 700;
  color: #0F172A;
  white-space: nowrap;
}

.org-role {
  font-size: 0.66rem;
  color: #64748B;
  font-weight: 600;
  white-space: nowrap;
}

.org-terminated-badge {
  font-size: 0.58rem;
  font-weight: 700;
  color: #DC2626;
  text-transform: uppercase;
  letter-spacing: 0.4px;
  margin-top: 2px;
}

/* Terminate / Remove — small, always visible but muted at rest
   so the card stays calm until you actually need them. */
.org-card-actions {
  display: flex;
  flex-direction: column;
  gap: 4px;
  margin-left: 4px;
  padding-left: 10px;
  border-left: 1px solid #F1F5F9;
}

.org-icon-btn {
  width: 20px;
  height: 20px;
  border-radius: 6px;
  border: none;
  background: none;
  color: #CBD5E1;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.15s;
}

.org-icon-btn:hover {
  background: #F1F5F9;
  color: #64748B;
}

.org-icon-btn.active {
  color: #02b0a1;
}

.org-icon-btn.danger:hover {
  background: #FEE2E2;
  color: #DC2626;
}

.org-icon-btn:disabled {
  color: #E5E7EB;
  cursor: not-allowed;
}

.org-icon-btn:disabled:hover {
  background: none;
  color: #E5E7EB;
}

/* Add-report button — light blue, doubles as the joint marker
   where the connector line leaves the card. */
.org-add-btn {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  border: 1px solid #93C5FD;
  background: #EFF6FF;
  color: #3B82F6;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  margin-top: 8px;
  transition: all 0.15s;
}

.org-add-btn:hover {
  border-color: #3B82F6;
  background: #DBEAFE;
  color: #2563EB;
}

/* ---------- Connectors — sidebar navy, thin and confident ---------- */
.org-children {
  position: relative;
  padding-top: 26px;
}

.org-children-connector {
  position: absolute;
  top: 0;
  left: 50%;
  width: 1.5px;
  height: 26px;
  background: #04111f;
  transform: translateX(-50%);
}

.org-children-row {
  display: flex;
  justify-content: center;
  gap: 28px;
}

.org-child-branch {
  position: relative;
  padding-top: 20px;
}

/* Vertical stub down into each child */
.org-child-branch::before {
  content: '';
  position: absolute;
  top: 0;
  left: 50%;
  width: 1.5px;
  height: 20px;
  background: #04111f;
}

/* Horizontal bar across siblings — trimmed at the first/last
   child so it never overhangs past the outer branches. A lone
   child gets no horizontal bar at all. */
.org-child-branch::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 1.5px;
  background: #04111f;
}

.org-child-branch:first-child::after { left: 50%; }
.org-child-branch:last-child::after { right: 50%; }
.org-child-branch:only-child::after { display: none; }
</style>
