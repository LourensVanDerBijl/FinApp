<script setup>
// ─────────────────────────────────────────────────────────────────────────
// One detail surface, reused for CA1 (contribution change), CA2 (policy
// proposal), and CA3 (payment). Rows stay compact; nothing is lost — the
// full field set from the source spec lives here, one click away, instead
// of being permanently visible (and permanently scrolled-past) in every
// row at once.
// ─────────────────────────────────────────────────────────────────────────
import { X } from 'lucide-vue-next'

defineProps({
  open: { type: Boolean, default: false },
  title: { type: String, default: '' }
})

const emit = defineEmits(['close'])
</script>

<template>
  <Teleport to="body">
    <Transition name="drawer-fade">
      <div v-if="open" class="drawer-overlay" @click.self="emit('close')">
        <Transition name="drawer-slide">
          <aside v-if="open" class="drawer-panel">
            <div class="drawer-header">
              <h2>{{ title }}</h2>
              <button type="button" class="drawer-close" @click="emit('close')" aria-label="Close">
                <X :size="16" />
              </button>
            </div>
            <div class="drawer-body">
              <slot />
            </div>
          </aside>
        </Transition>
      </div>
    </Transition>
  </Teleport>
</template>

<style scoped>
.drawer-overlay {
  position: fixed;
  inset: 0;
  background: rgba(15, 23, 42, 0.35);
  display: flex;
  justify-content: flex-end;
  z-index: 200;
}

.drawer-panel {
  width: 380px;
  max-width: 92vw;
  height: 100%;
  background: #fff;
  display: flex;
  flex-direction: column;
  box-shadow: -8px 0 24px rgba(15, 23, 42, 0.12);
}

.drawer-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 16px 18px;
  border-bottom: 1px solid #e2e8f0;
  flex-shrink: 0;
}

.drawer-header h2 {
  margin: 0;
  font-size: 0.98rem;
  font-weight: 800;
  color: #0f172a;
}

.drawer-close {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 28px;
  height: 28px;
  border-radius: 7px;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #64748b;
  cursor: pointer;
}

.drawer-close:hover {
  background: #f8fafc;
}

.drawer-body {
  flex: 1;
  overflow-y: auto;
  padding: 18px;
  display: flex;
  flex-direction: column;
  gap: 14px;
}

.drawer-fade-enter-active,
.drawer-fade-leave-active {
  transition: opacity 0.18s ease;
}
.drawer-fade-enter-from,
.drawer-fade-leave-to {
  opacity: 0;
}

.drawer-slide-enter-active,
.drawer-slide-leave-active {
  transition: transform 0.22s ease;
}
.drawer-slide-enter-from,
.drawer-slide-leave-to {
  transform: translateX(100%);
}
</style>
