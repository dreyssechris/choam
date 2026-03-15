<script setup lang="ts">
import { format, parseISO } from 'date-fns'
import { useCategories } from '../composables/useCategories'
import type { Transaction } from '../contracts/transactions'
import '../styles/components/_transaction-details.scss'

const props = defineProps<{
  tx: Transaction
}>()

const { selectById } = useCategories()
const category = selectById(props.tx.categoryId)
</script>

<template>
  <q-list class="transaction-details">
    <q-item>
      <q-item-section side>Date</q-item-section>
      <q-item-section>{{ format(parseISO(tx.date), 'yyyy-MM-dd') }}</q-item-section>
    </q-item>
    <q-separator />
    <q-item>
      <q-item-section side>Title</q-item-section>
      <q-item-section>{{ tx.title }}</q-item-section>
    </q-item>
    <q-separator />
    <q-item>
      <q-item-section side>Type</q-item-section>
      <q-item-section>{{ tx.type }}</q-item-section>
    </q-item>
    <q-separator />
    <q-item>
      <q-item-section side>Amount</q-item-section>
      <q-item-section>{{ tx.amount.toFixed(2) }} EUR</q-item-section>
    </q-item>
    <q-separator />
    <q-item>
      <q-item-section side>Category</q-item-section>
      <q-item-section>{{ category?.name ?? tx.categoryId }}</q-item-section>
    </q-item>
    <q-separator v-if="tx.description" />
    <q-item v-if="tx.description">
      <q-item-section side>Description</q-item-section>
      <q-item-section>{{ tx.description }}</q-item-section>
    </q-item>
  </q-list>
</template>
