<script setup lang="ts">
import { computed, ref } from 'vue'
import { useTransactions } from '../composables/useTransactions'
import {
  useMonth,
  filterTransactionByMonth,
  totalsByType,
} from '../composables/useMonth'
import TransactionList from '../components/TransactionList.vue'
import TransactionForm from '../components/TransactionForm.vue'
import TransactionDetails from '../components/TransactionDetails.vue'
import MonthPicker from '../components/MonthPicker.vue'
import type {
  Transaction,
  TransactionCreate,
  TransactionUpdate,
} from '../contracts/transactions'
import '../styles/pages/_transactions-page.scss'

function toDateInputValue(iso: string) {
  const d = new Date(iso)
  const y = d.getFullYear()
  const m = String(d.getMonth() + 1).padStart(2, '0')
  const day = String(d.getDate()).padStart(2, '0')
  return `${y}-${m}-${day}`
}

type ModalState =
  | { kind: 'closed' }
  | { kind: 'view'; tx: Transaction }
  | { kind: 'edit'; tx: Transaction }
  | { kind: 'create' }

const { list, create, update, remove } = useTransactions()

const modal = ref<ModalState>({ kind: 'closed' })

const now = new Date()
const { year, month, previousMonth, nextMonth } = useMonth(
  now.getFullYear(),
  now.getMonth(),
)

const allTransactions = computed(() => list.data.value ?? [])

const monthTransactions = computed(() =>
  filterTransactionByMonth(allTransactions.value, year.value, month.value),
)

const totals = computed(() => totalsByType(monthTransactions.value))

const eur = new Intl.NumberFormat('en-EN', {
  style: 'currency',
  currency: 'EUR',
})

const dialogOpen = computed({
  get: () => modal.value.kind !== 'closed',
  set: (val: boolean) => {
    if (!val) modal.value = { kind: 'closed' }
  },
})

const modalTitle = computed(() => {
  switch (modal.value.kind) {
    case 'view': return 'Transaction details'
    case 'edit': return 'Edit transaction'
    case 'create': return 'New transaction'
    default: return undefined
  }
})

function handleCreate(values: TransactionCreate) {
  create.mutate(values, {
    onSuccess: () => { modal.value = { kind: 'closed' } },
  })
}

function handleUpdate(values: TransactionUpdate) {
  if (modal.value.kind !== 'edit') return
  update.mutate(
    { id: modal.value.tx.id, payload: values },
    { onSuccess: () => { modal.value = { kind: 'closed' } } },
  )
}
</script>

<template>
  <q-page padding>
    <div class="q-mx-auto transactions-page">
      <div v-if="list.isLoading.value" class="text-center q-pa-xl">
        <q-spinner-dots size="40px" color="primary" />
        <p class="q-mt-md">Loading...</p>
      </div>

      <div v-else-if="list.isError.value" class="text-center q-pa-xl text-negative">
        <q-icon name="error" size="48px" />
        <p class="q-mt-md">Failed to load Ledger</p>
      </div>

      <template v-else>
        <h1 class="text-h4 q-mb-md text-center">Ledger</h1>

        <MonthPicker
          :year="year"
          :month="month"
          @previous="previousMonth"
          @next="nextMonth"
          @update:month="(m: number) => (month = m)"
        />

        <div class="q-my-md text-center">
          <q-btn
            round
            icon="add"
            color="primary"
            aria-label="Add new transaction"
            @click="modal = { kind: 'create' }"
          />
        </div>

        <div class="row q-col-gutter-sm q-mb-lg">
          <div class="col-6 col-md-3">
            <q-card flat bordered dark class="summary-card">
              <q-card-section class="column items-center justify-center q-pa-md">
                <q-icon name="trending_up" class="text-income q-mb-xs" size="28px" />
                <span class="summary-card__value text-income">{{ eur.format(totals.income) }}</span>
              </q-card-section>
            </q-card>
          </div>
          <div class="col-6 col-md-3">
            <q-card flat bordered dark class="summary-card">
              <q-card-section class="column items-center justify-center q-pa-md">
                <q-icon name="trending_down" class="text-expense q-mb-xs" size="28px" />
                <span class="summary-card__value text-expense">{{ eur.format(totals.expense) }}</span>
              </q-card-section>
            </q-card>
          </div>
          <div class="col-6 col-md-3">
            <q-card flat bordered dark class="summary-card">
              <q-card-section class="column items-center justify-center q-pa-md">
                <q-icon name="savings" class="text-investment q-mb-xs" size="28px" />
                <span class="summary-card__value text-investment">{{ eur.format(totals.investment) }}</span>
              </q-card-section>
            </q-card>
          </div>
          <div class="col-6 col-md-3">
            <q-card flat bordered dark class="summary-card">
              <q-card-section class="column items-center justify-center q-pa-md">
                <q-icon name="swap_vert" size="28px" class="q-mb-xs" />
                <span
                  class="summary-card__value"
                  :class="totals.netCashFlow >= 0 ? 'text-income' : 'text-expense'"
                >
                  {{ eur.format(totals.netCashFlow) }}
                </span>
              </q-card-section>
            </q-card>
          </div>
        </div>

        <TransactionList
          :items="monthTransactions"
          @view="(t: Transaction) => (modal = { kind: 'view', tx: t })"
          @edit="(t: Transaction) => (modal = { kind: 'edit', tx: t })"
          @delete="(id: number) => remove.mutate(id)"
        />

        <q-dialog v-model="dialogOpen">
          <q-card dark class="dialog-card">
            <q-card-section class="row items-center q-pb-none">
              <div class="text-h6">{{ modalTitle }}</div>
              <q-space />
              <q-btn flat round dense icon="close" @click="modal = { kind: 'closed' }" />
            </q-card-section>

            <q-separator />

            <q-card-section>
              <TransactionDetails
                v-if="modal.kind === 'view'"
                :tx="modal.tx"
              />

              <TransactionForm
                v-if="modal.kind === 'edit'"
                :default-values="{
                  title: modal.tx.title,
                  amount: modal.tx.amount,
                  description: modal.tx.description ?? '',
                  date: toDateInputValue(modal.tx.date),
                  type: modal.tx.type,
                  categoryId: modal.tx.categoryId,
                }"
                :submitting="update.isPending.value"
                submit-label="Save changes"
                @submit="handleUpdate"
                @cancel="modal = { kind: 'closed' }"
              />

              <TransactionForm
                v-if="modal.kind === 'create'"
                :submitting="create.isPending.value"
                submit-label="Create"
                @submit="handleCreate"
              />
            </q-card-section>
          </q-card>
        </q-dialog>
      </template>
    </div>
  </q-page>
</template>
