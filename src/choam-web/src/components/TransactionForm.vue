<script setup lang="ts">
import { ref, computed, watch } from 'vue'
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

// Amount stored as cents — digits shift from right like banking apps
const amountCents = ref(
  props.defaultValues?.amount != null
    ? Math.round(props.defaultValues.amount * 100)
    : 0,
)

const amountDisplay = computed(() => {
  const euros = Math.floor(amountCents.value / 100)
  const cents = amountCents.value % 100
  return `${euros.toLocaleString('de-DE')},${String(cents).padStart(2, '0')} €`
})

function onAmountKeydown(e: KeyboardEvent) {
  if (e.key >= '0' && e.key <= '9') {
    e.preventDefault()
    amountCents.value = amountCents.value * 10 + Number(e.key)
  } else if (e.key === 'Backspace') {
    e.preventDefault()
    amountCents.value = Math.floor(amountCents.value / 10)
  }
}

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
    if (dv.amount !== undefined) amountCents.value = Math.round(dv.amount * 100)
    if (dv.type !== undefined) type.value = dv.type
    if (dv.date !== undefined) date.value = dv.date
    if (dv.categoryId !== undefined) categoryId.value = dv.categoryId
    if (dv.description !== undefined) description.value = dv.description
  },
)

const amountRules = [
  () => amountCents.value > 0 || 'Fill in an amount',
]

function handleSubmit() {
  const payload: TransactionCreate = {
    title: title.value,
    amount: amountCents.value / 100,
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
  <q-form
    class="transaction-form"
    @submit.prevent="handleSubmit"
  >
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
          :model-value="amountDisplay"
          label="Amount"
          outlined
          dense
          dark
          :disable="submitting"
          :rules="amountRules"
          lazy-rules
          @keydown="onAmountKeydown"
          @beforeinput.prevent
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
