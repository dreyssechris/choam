<script setup lang="ts">
import { ref, watch } from 'vue'
import CategorySelect from './CategorySelect.vue'
import CategoryCreate from './CategoryCreate.vue'
import type { TransactionCreate, TransactionType } from '../contracts/transactions'
import '../styles/components/_transaction-form.scss'

const props = withDefaults(
  defineProps<{
    defaultValues?: Partial<TransactionCreate>
    submitting?: boolean
    submitLabel?: string
    defaultCategoryId?: number
  }>(),
  {
    submitting: false,
    submitLabel: 'Create',
    defaultCategoryId: 1,
  },
)

const emit = defineEmits<{
  submit: [values: TransactionCreate]
  cancel: []
}>()

const title = ref(props.defaultValues?.title ?? '')
const amount = ref(
  props.defaultValues?.amount != null ? String(props.defaultValues.amount) : '',
)
const type = ref<TransactionType>(props.defaultValues?.type ?? 'Income')
const date = ref(
  props.defaultValues?.date ?? new Date().toISOString().slice(0, 10),
)
const categoryId = ref(props.defaultValues?.categoryId ?? props.defaultCategoryId)
const description = ref(props.defaultValues?.description ?? '')

const typeOptions: { label: string; value: TransactionType }[] = [
  { label: 'Income', value: 'Income' },
  { label: 'Expense', value: 'Expense' },
  { label: 'Investment', value: 'Investment' },
]

watch(
  () => props.defaultValues,
  (dv) => {
    if (!dv) return
    if (dv.title !== undefined) title.value = dv.title
    if (dv.amount !== undefined) amount.value = String(dv.amount)
    if (dv.type !== undefined) type.value = dv.type
    if (dv.date !== undefined) date.value = dv.date
    if (dv.categoryId !== undefined) categoryId.value = dv.categoryId
    if (dv.description !== undefined) description.value = dv.description
  },
)

function handleSubmit() {
  const payload: TransactionCreate = {
    title: title.value,
    amount: Number(amount.value),
    description: description.value.trim() || undefined,
    date: new Date(date.value).toISOString(),
    type: type.value,
    categoryId: categoryId.value,
  }
  emit('submit', payload)
}

const showCancel = !!props.defaultValues
</script>

<template>
  <q-form class="transaction-form" @submit.prevent="handleSubmit">
    <div class="row q-col-gutter-md">
      <div class="col-12 col-md-6">
        <q-input
          v-model="title"
          label="Title"
          outlined
          dense
          dark
          :disable="submitting"
          :rules="[(v: string) => !!v || 'Fill in a title']"
          lazy-rules
        />
      </div>
      <div class="col-12 col-md-6">
        <q-input
          v-model="amount"
          label="Amount"
          outlined
          dense
          dark
          prefix="EUR "
          type="number"
          step="0.01"
          min="0"
          :disable="submitting"
          :rules="[(v: string) => !!v || 'Fill in an amount']"
          lazy-rules
        />
      </div>
      <div class="col-12 col-md-6">
        <q-select
          v-model="type"
          :options="typeOptions"
          emit-value
          map-options
          label="Type"
          outlined
          dense
          dark
          :disable="submitting"
        />
      </div>
      <div class="col-12 col-md-6">
        <q-input
          v-model="date"
          label="Date"
          type="date"
          outlined
          dense
          dark
          :disable="submitting"
        />
      </div>
      <div class="col-12 col-md-6">
        <CategorySelect
          v-model="categoryId"
          :disable="submitting"
        />
        <div class="transaction-form__category-row">
          <CategoryCreate @created="(id: number) => (categoryId = id)" />
        </div>
      </div>
      <div class="col-12">
        <q-input
          v-model="description"
          label="Description (optional)"
          outlined
          dense
          dark
          :disable="submitting"
        />
      </div>
    </div>
    <div class="row justify-center q-gutter-sm q-mt-lg">
      <q-btn
        :label="submitLabel"
        type="submit"
        color="primary"
        :loading="submitting"
        :disable="submitting"
      />
      <q-btn
        v-if="showCancel"
        label="Cancel"
        flat
        :disable="submitting"
        @click="emit('cancel')"
      />
    </div>
  </q-form>
</template>
