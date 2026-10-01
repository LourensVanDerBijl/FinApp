<script setup>
// ─────────────────────────────────────────────────────────────────────────
// One button component, every action in the dashboard uses it. Before this,
// Approve/Question/Decline/Paid/Unpaid/Short/Over Paid each had their own
// slightly different button CSS. Now there's exactly one place that
// defines what an action button looks like, sized and colored by variant.
// ─────────────────────────────────────────────────────────────────────────
defineProps({
  variant: {
    type: String,
    default: 'neutral', // 'primary' | 'success' | 'danger' | 'warning' | 'neutral' | 'ghost'
    validator: (v) => ['primary', 'success', 'danger', 'warning', 'neutral', 'ghost'].includes(v)
  },
  size: {
    type: String,
    default: 'sm', // 'sm' | 'xs'
    validator: (v) => ['sm', 'xs'].includes(v)
  },
  disabled: { type: Boolean, default: false }
})

defineEmits(['click'])
</script>

<template>
  <button
    type="button"
    class="action-btn"
    :class="[variant, size]"
    :disabled="disabled"
    @click="$emit('click', $event)"
  >
    <slot name="icon" />
    <span><slot /></span>
  </button>
</template>

<style scoped>
.action-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 5px;
  border-radius: 7px;
  font-family: inherit;
  font-weight: 700;
  cursor: pointer;
  border: 1px solid transparent;
  transition: background 0.15s, border-color 0.15s, opacity 0.15s;
  white-space: nowrap;
}

.action-btn.sm {
  padding: 6px 11px;
  font-size: 0.74rem;
}

.action-btn.xs {
  padding: 4px 8px;
  font-size: 0.68rem;
}

.action-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

/* primary — the default "do the main thing" action */
.action-btn.primary {
  background: #1855b9;
  border-color: #1855b9;
  color: #fff;
}
.action-btn.primary:hover:not(:disabled) {
  background: #154da3;
}

/* success — Approve, mark Paid */
.action-btn.success {
  background: #fff;
  border-color: #bbf7d0;
  color: #16a34a;
}
.action-btn.success:hover:not(:disabled) {
  background: #f0fdf4;
}

/* danger — Decline */
.action-btn.danger {
  background: #fff;
  border-color: #fecaca;
  color: #dc2626;
}
.action-btn.danger:hover:not(:disabled) {
  background: #fef2f2;
}

/* warning — Short Payment / Unpaid-type flags */
.action-btn.warning {
  background: #fff;
  border-color: #fed7aa;
  color: #c2410c;
}
.action-btn.warning:hover:not(:disabled) {
  background: #fff7ed;
}

/* neutral — Question, Cancel, secondary actions */
.action-btn.neutral {
  background: #fff;
  border-color: #e2e8f0;
  color: #334155;
}
.action-btn.neutral:hover:not(:disabled) {
  background: #f8fafc;
  border-color: #cbd5e1;
}

/* ghost — icon-only / low-emphasis triggers (e.g. row chevrons) */
.action-btn.ghost {
  background: transparent;
  border-color: transparent;
  color: #94a3b8;
  padding: 4px;
}
.action-btn.ghost:hover:not(:disabled) {
  background: #f1f5f9;
  color: #64748b;
}
</style>
