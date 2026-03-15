<script setup lang="ts">
import { useCategories } from '../composables/useCategories'
import '../styles/components/_category-select.scss'

const model = defineModel<number>({ required: true })

defineProps<{
  disable?: boolean
}>()

const { list, remove } = useCategories()

function handleDelete() {
  if (!model.value) return
  if (!window.confirm('Remove the Category permanently?')) return

  const nextId = (list.data.value ?? []).find(
    (c) => c.id !== model.value,
  )?.id

  remove.mutate(model.value, {
    onSuccess: () => {
      if (nextId != null) model.value = nextId
    },
  })
}
</script>

<template>
  <div v-if="list.isLoading.value" class="text-grey">Loading...</div>
  <div v-else-if="list.isError.value" class="text-negative">
    Failed to load categories
  </div>
  <q-select
    v-else
    v-model="model"
    :options="(list.data.value ?? []).map((c) => ({ label: c.name, value: c.id }))"
    emit-value
    map-options
    label="Category"
    outlined
    dense
    dark
    :disable="disable"
    class="category-select"
  >
    <template #append>
      <q-btn
        flat
        dense
        round
        icon="close"
        color="negative"
        size="sm"
        :disable="disable || remove.isPending.value"
        @click.stop="handleDelete"
      />
    </template>
  </q-select>
</template>
