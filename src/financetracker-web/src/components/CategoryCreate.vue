<script setup lang="ts">
import { ref } from 'vue'
import { useCategories } from '../composables/useCategories'
import '../styles/components/_category-create.scss'

const emit = defineEmits<{
  created: [id: number]
}>()

const { list, create } = useCategories()
const name = ref('')
const error = ref<string | null>(null)

async function add() {
  error.value = null
  const trimmed = name.value.trim()
  if (!trimmed) return

  const exists = (list.data.value ?? []).some(
    (c) => c.name.trim().toLowerCase() === trimmed.toLowerCase(),
  )
  if (exists) {
    error.value = 'Category already exists.'
    return
  }

  try {
    const created = await create.mutateAsync(trimmed)
    if (created && typeof (created as any).id === 'number') {
      emit('created', (created as any).id)
      name.value = ''
    }
  } catch (err: any) {
    error.value = err?.message || 'Failed to create category.'
  }
}

function onKeydown(e: KeyboardEvent) {
  if (e.key === 'Enter') {
    e.preventDefault()
    void add()
  }
}
</script>

<template>
  <div class="category-create">
    <q-input
      v-model="name"
      label="New Category"
      outlined
      dense
      dark
      :disable="create.isPending.value"
      @keydown="onKeydown"
      @update:model-value="error = null"
    >
      <template #append>
        <q-btn
          flat
          dense
          label="Create"
          color="primary"
          :disable="!name.trim() || create.isPending.value"
          @click="add"
        />
      </template>
    </q-input>
    <div v-if="error" class="text-negative q-mt-xs" role="alert">
      {{ error }}
    </div>
  </div>
</template>
