<script setup>
import { computed } from 'vue'
import { Sparkles, AlertCircle } from 'lucide-vue-next'

const MAX_LENGTH = 20

const props = defineProps({
  modelValue: { type: String, required: true },
  suggestedName: { type: String, required: true },
  errorText: { type: String, default: '' }
})

const emit = defineEmits(['update:modelValue'])

const charCount = computed(() => props.modelValue.length)

function handleInput(event) {
  emit('update:modelValue', event.target.value.slice(0, MAX_LENGTH))
}

function applySuggestion() {
  emit('update:modelValue', props.suggestedName)
}
</script>

<template>
  <div class="field" :class="{ 'has-error': errorText }">
    <div class="field-label-row">
      <label for="group-name">Group Name</label>
      <span class="char-count">{{ charCount }}/{{ MAX_LENGTH }}</span>
    </div>

    <input
      id="group-name"
      type="text"
      :value="modelValue"
      @input="handleInput"
      :maxlength="MAX_LENGTH"
      placeholder="e.g. Smith Group"
    />

    <button type="button" class="suggestion-chip" @click="applySuggestion">
      <Sparkles :size="12" />
      Use suggestion: <strong>{{ suggestedName }}</strong>
    </button>

    <p v-if="errorText" class="error-text">
      <AlertCircle :size="12" />
      {{ errorText }}
    </p>
  </div>
</template>

<style scoped>
.field {
  margin-bottom: 12px;
}

.field-label-row {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  margin-bottom: 6px;
}

.field label {
  font-size: 0.7rem;
  font-weight: 700;
  color: #334155;
}

.char-count {
  font-size: 0.62rem;
  color: #94a3b8;
}

.field input {
  width: 100%;
  padding: 9px 11px;
  border-radius: 9px;
  border: 1px solid #e2e8f0;
  background: #fff;
  color: #0f172a;
  font-family: inherit;
  font-size: 0.82rem;
  outline: none;
  transition: border-color 0.15s;
}

.field input::placeholder {
  color: #94a3b8;
}

.field input:focus {
  border-color: #1855b9;
}

.has-error input {
  border-color: #dc2626;
}

.suggestion-chip {
  display: inline-flex;
  align-items: center;
  gap: 5px;
  margin-top: 8px;
  padding: 5px 10px;
  border-radius: 999px;
  border: 1px solid rgba(37, 99, 235, 0.18);
  background: rgba(37, 99, 235, 0.05);
  color: #1855b9;
  font-family: inherit;
  font-size: 0.66rem;
  cursor: pointer;
  transition: background 0.15s, border-color 0.15s;
}

.suggestion-chip:hover {
  background: rgba(37, 99, 235, 0.1);
  border-color: rgba(37, 99, 235, 0.3);
}

.suggestion-chip strong {
  font-weight: 700;
}

.error-text {
  display: flex;
  align-items: center;
  gap: 5px;
  font-size: 0.68rem;
  color: #dc2626;
  margin: 8px 0 0;
}

@media (max-width: 960px) {
  .field label {
    font-size: 0.78rem;
  }

  .field input {
    padding: 11px 13px;
    font-size: 0.9rem;
  }
}
</style>
