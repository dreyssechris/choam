<script setup lang="ts">
import type { TransactionCreate } from '../contracts/transactions'
import type { Category } from '../contracts/categories'
import { eur, formatDate, resolveTransactionType } from '../utils/formatters'
import '../styles/components/_proposal-card.scss'

defineProps<{
  proposal: TransactionCreate
  categories: Category[]
  disabled?: boolean
}>()

const emit = defineEmits<{
  confirm: [proposal: TransactionCreate]
  edit: []
  discard: []
}>()

function categoryName(id: number, categories: Category[]) {
  return categories.find(c => c.id === id)?.name ?? 'Unknown'
}
</script>

<template>
  <div class="proposal-card">
    <p class="proposal-card__label font-label">Transaction Proposal</p>
    <div class="proposal-card__fields">
      <div class="proposal-card__field">
        <span class="proposal-card__key font-label">Title</span>
        <span class="proposal-card__value">{{ proposal.title }}</span>
      </div>
      <div class="proposal-card__field">
        <span class="proposal-card__key font-label">Amount</span>
        <span class="proposal-card__value">{{ eur.format(proposal.amount) }}</span>
      </div>
      <div class="proposal-card__field">
        <span class="proposal-card__key font-label">Type</span>
        <span class="proposal-card__value">{{ resolveTransactionType(proposal.type) }}</span>
      </div>
      <div class="proposal-card__field">
        <span class="proposal-card__key font-label">Date</span>
        <span class="proposal-card__value">{{ formatDate(proposal.date) }}</span>
      </div>
      <div class="proposal-card__field">
        <span class="proposal-card__key font-label">Category</span>
        <span class="proposal-card__value">{{ categoryName(proposal.categoryId, categories) }}</span>
      </div>
    </div>
    <div class="proposal-card__actions">
      <button class="btn-primary" :disabled="disabled" @click="emit('confirm', proposal)">
        <q-icon name="sym_o_check" size="16px" />
        Confirm
      </button>
      <button class="btn-ghost" @click="emit('edit')">
        <q-icon name="sym_o_edit" size="16px" />
        Edit
      </button>
      <button class="btn-ghost" @click="emit('discard')">
        <q-icon name="sym_o_close" size="16px" />
        Discard
      </button>
    </div>
  </div>
</template>
