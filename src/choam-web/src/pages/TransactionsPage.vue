<template>
  <q-page padding>
    <div class="q-mx-auto transactions-page">
      <div v-if="list.isLoading.value" class="page-status">
        <q-spinner-dots size="40px" color="primary" />
        <p class="page-status__text">Loading...</p>
      </div>

      <div v-else-if="list.isError.value" class="page-status text-negative">
        <q-icon name="error" size="48px" />
        <p class="page-status__text">Failed to load Ledger</p>
      </div>

      <template v-else>
        <!-- Header -->
        <header class="ledger-header">
          <div>
            <p class="ledger-header__subtitle font-label">
              Ankert/Dreyße Financials
            </p>
            <h1 class="ledger-header__title font-headline">
              Ledger
            </h1>
          </div>
          <MonthPicker
            :year="year"
            :month="month"
            @previous="previousMonth"
            @next="nextMonth"
            @update:month="(m: number) => (month = m)"
          />
          <div class="ledger-header__actions">
            <button class="btn-ghost">
              <q-icon name="sym_o_file_download" size="18px" />
              Export Statement
            </button>
            <button
              class="btn-primary"
              @click="modal = { kind: 'create' }"
            >
              <q-icon name="sym_o_add" size="20px" />
              New Entry
            </button>
          </div>
        </header>

        <!-- KPI Cards -->
        <div class="kpi-grid">
          <div class="kpi-card">
            <div class="kpi-card__header">
              <span class="kpi-card__label font-label">
                Total Income
                <q-icon name="trending_up" class="kpi-card__icon kpi-card__icon--income" />
              </span>
            </div>
            <div class="kpi-card__body">
              <span class="kpi-card__value font-headline">{{ eur.format(totals.income) }}</span>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-card__header">
              <span class="kpi-card__label font-label">
                Total Expenses
                <q-icon name="trending_down" class="kpi-card__icon kpi-card__icon--expense" />
              </span>
            </div>
            <div class="kpi-card__body">
              <span class="kpi-card__value font-headline">{{ eur.format(totals.expense) }}</span>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-card__header">
              <span class="kpi-card__label font-label">
                Total Investments
                <q-icon name="sym_o_bar_chart" class="kpi-card__icon kpi-card__icon--investment" />
              </span>
            </div>
            <div class="kpi-card__body">
              <span class="kpi-card__value font-headline">{{ eur.format(totals.investment) }}</span>
            </div>
          </div>

          <div class="kpi-card">
            <div class="kpi-card__header">
              <span class="kpi-card__label font-label">
                Netto-Balance
                <q-icon name="sym_o_account_balance" class="kpi-card__icon kpi-card__icon--balance" />
              </span>
            </div>
            <div class="kpi-card__body">
              <span
                class="kpi-card__value font-headline"
                :class="totals.netCashFlow >= 0 ? 'kpi-card__value--positive' : 'kpi-card__value--negative'"
              >
                {{ eur.format(totals.netCashFlow) }}
              </span>
            </div>
          </div>
        </div>

        <!-- Transaction List -->
        <TransactionList
          :items="monthTransactions"
          @view="(t: Transaction) => (modal = { kind: 'view', tx: t })"
          @edit="(t: Transaction) => (modal = { kind: 'edit', tx: t })"
          @delete="(id: number) => remove.mutate(id)"
        />

        <!-- Dialog -->
        <q-dialog v-model="dialogOpen">
          <q-card dark class="dialog-card">
            <q-card-section class="row items-center q-pb-none">
              <div class="text-h6 font-headline">{{ modalTitle }}</div>
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

const eur = new Intl.NumberFormat('de-DE', {
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
