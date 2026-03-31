<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue'
import { format, parseISO } from 'date-fns'
import { useCategories } from '../composables/useCategories'
import type { Transaction } from '../contracts/transactions'
import '../styles/components/_transaction-list.scss'

defineProps<{
  items: Transaction[]
}>()

const emit = defineEmits<{
  view: [tx: Transaction]
  edit: [tx: Transaction]
  delete: [id: number]
}>()

const { list: categoryList } = useCategories()

const isMobile = ref(window.innerWidth < 600)

function onResize() {
  isMobile.value = window.innerWidth < 600
}

onMounted(() => window.addEventListener('resize', onResize))
onUnmounted(() => window.removeEventListener('resize', onResize))

function typeIcon(type: string) {
  switch (type) {
    case 'Income': return { icon: 'trending_up', modifier: 'income' }
    case 'Expense': return { icon: 'trending_down', modifier: 'expense' }
    case 'Investment': return { icon: 'bar_chart', modifier: 'investment' }
    default: return { icon: 'help', modifier: '' }
  }
}

function getCategoryName(categoryId: number): string {
  const cat = categoryList.data.value?.find((c) => c.id === categoryId)
  return cat?.name ?? String(categoryId)
}

function formatAmount(row: Transaction): string {
  const prefix = row.type === 'Income' ? '+' : '-'
  return `${prefix}${Math.abs(row.amount).toLocaleString('de-DE', { minimumFractionDigits: 2 })} €`
}

function formatDate(iso: string): string {
  return format(parseISO(iso), 'dd MMM yyyy')
}
</script>

<template>
  <section class="ledger-section">
    <div
      v-if="items.length === 0"
      class="ledger-empty"
    >
      No transactions yet.
    </div>

    <!-- Desktop table -->
    <div
      v-else-if="!isMobile"
      style="overflow-x: auto"
    >
      <table class="ledger-table">
        <thead>
          <tr>
            <th class="ledger-table__icon-cell font-label" />
            <th class="font-label">
              Date
            </th>
            <th class="font-label">
              Title
            </th>
            <th class="font-label">
              Type
            </th>
            <th class="font-label">
              Amount
            </th>
            <th class="font-label">
              Category
            </th>
            <th class="font-label">
              Actions
            </th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="row in items"
            :key="row.id"
          >
            <td class="ledger-table__icon-cell">
              <q-icon
                :name="typeIcon(row.type).icon"
                class="ledger-table__icon"
                :class="`ledger-table__icon--${typeIcon(row.type).modifier}`"
                size="23px"
              />
            </td>
            <td class="ledger-table__date">
              {{ formatDate(row.date) }}
            </td>
            <td class="ledger-table__title">
              {{ row.title }}
            </td>
            <td class="ledger-table__type">
              {{ row.type }}
            </td>
            <td class="ledger-table__amount font-headline">
              {{ formatAmount(row) }}
            </td>
            <td class="ledger-table__category">
              <span class="category-badge">
                {{ getCategoryName(row.categoryId) }}
              </span>
            </td>
            <td class="ledger-table__actions">
              <button class="actions-btn">
                <q-icon
                  name="sym_o_more_vert"
                  size="20px"
                />
                <q-menu>
                  <q-list dense>
                    <q-item
                      clickable
                      @click="emit('view', row)"
                    >
                      <q-item-section avatar>
                        <q-icon name="sym_o_visibility" />
                      </q-item-section>
                      <q-item-section>View</q-item-section>
                    </q-item>
                    <q-item
                      clickable
                      @click="emit('edit', row)"
                    >
                      <q-item-section avatar>
                        <q-icon name="sym_o_edit" />
                      </q-item-section>
                      <q-item-section>Edit</q-item-section>
                    </q-item>
                    <q-item
                      clickable
                      @click="emit('delete', row.id)"
                    >
                      <q-item-section avatar>
                        <q-icon
                          name="sym_o_delete"
                          color="negative"
                        />
                      </q-item-section>
                      <q-item-section class="text-negative">
                        Delete
                      </q-item-section>
                    </q-item>
                  </q-list>
                </q-menu>
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Mobile card layout -->
    <div
      v-else
      class="ledger-cards"
    >
      <div
        v-for="row in items"
        :key="row.id"
        class="ledger-card"
      >
        <q-icon
          :name="typeIcon(row.type).icon"
          class="ledger-table__icon"
          :class="`ledger-table__icon--${typeIcon(row.type).modifier}`"
          size="20px"
        />
        <div class="ledger-card__body">
          <div class="ledger-card__title">
            {{ row.title }}
          </div>
          <div class="ledger-card__meta">
            {{ formatDate(row.date) }} · {{ row.type }} · {{ getCategoryName(row.categoryId) }}
          </div>
        </div>
        <span class="ledger-card__amount font-headline">
          {{ formatAmount(row) }}
        </span>
        <button class="actions-btn">
          <q-icon
            name="sym_o_more_vert"
            size="20px"
          />
          <q-menu>
            <q-list dense>
              <q-item
                clickable
                @click="emit('view', row)"
              >
                <q-item-section avatar>
                  <q-icon name="sym_o_visibility" />
                </q-item-section>
                <q-item-section>View</q-item-section>
              </q-item>
              <q-item
                clickable
                @click="emit('edit', row)"
              >
                <q-item-section avatar>
                  <q-icon name="sym_o_edit" />
                </q-item-section>
                <q-item-section>Edit</q-item-section>
              </q-item>
              <q-item
                clickable
                @click="emit('delete', row.id)"
              >
                <q-item-section avatar>
                  <q-icon
                    name="sym_o_delete"
                    color="negative"
                  />
                </q-item-section>
                <q-item-section class="text-negative">
                  Delete
                </q-item-section>
              </q-item>
            </q-list>
          </q-menu>
        </button>
      </div>
    </div>
  </section>
</template>
