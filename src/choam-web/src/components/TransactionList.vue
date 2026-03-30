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

const { selectById } = useCategories()

// Responsive: switch to grid (card) mode on small screens
const isGridMode = ref(window.innerWidth < 600)

function onResize() {
  isGridMode.value = window.innerWidth < 600
}

onMounted(() => window.addEventListener('resize', onResize))
onUnmounted(() => window.removeEventListener('resize', onResize))

const columns = [
  {
    name: 'type-icon',
    label: '',
    field: 'type',
    align: 'center' as const,
    style: 'width: 48px',
    headerStyle: 'width: 48px',
  },
  {
    name: 'date',
    label: 'Date',
    field: (row: Transaction) => format(parseISO(row.date), 'yyyy-MM-dd'),
    align: 'left' as const,
    sortable: true,
  },
  {
    name: 'title',
    label: 'Title',
    field: 'title',
    align: 'left' as const,
  },
  {
    name: 'type',
    label: 'Type',
    field: 'type',
    align: 'left' as const,
  },
  {
    name: 'amount',
    label: 'Amount',
    field: 'amount',
    align: 'left' as const,
    format: (val: number) => `${val.toFixed(2)} EUR`,
  },
  {
    name: 'category',
    label: 'Category',
    field: (row: Transaction) => selectById(row.categoryId)?.name ?? row.categoryId,
    align: 'left' as const,
  },
  {
    name: 'actions',
    label: '',
    field: 'id',
    align: 'right' as const,
    style: 'width: 140px',
    headerStyle: 'width: 140px',
  },
]

function typeIcon(type: string) {
  switch (type) {
    case 'Income': return { icon: 'trending_up', colorClass: 'text-income' }
    case 'Expense': return { icon: 'trending_down', colorClass: 'text-expense' }
    case 'Investment': return { icon: 'savings', colorClass: 'text-investment' }
    default: return { icon: 'help', colorClass: 'text-grey' }
  }
}

function getCategoryName(categoryId: number): string {
  return selectById(categoryId)?.name ?? String(categoryId)
}
</script>

<template>
  <div v-if="items.length === 0" class="text-center q-pa-lg text-grey">
    No transactions yet.
  </div>
  <q-table
    v-else
    :rows="items"
    :columns="columns"
    row-key="id"
    flat
    bordered
    dark
    hide-pagination
    :rows-per-page-options="[0]"
    :grid="isGridMode"
    class="transaction-table"
  >
    <!-- Desktop: custom cells -->
    <template #body-cell-type-icon="slotProps">
      <q-td :props="slotProps">
        <q-icon
          :name="typeIcon(slotProps.row.type).icon"
          :class="typeIcon(slotProps.row.type).colorClass"
          size="sm"
        />
      </q-td>
    </template>

    <template #body-cell-actions="slotProps">
      <q-td :props="slotProps">
        <div class="transaction-table__actions">
          <q-btn flat round dense icon="visibility" @click="emit('view', slotProps.row)" />
          <q-btn flat round dense icon="edit" @click="emit('edit', slotProps.row)" />
          <q-btn flat round dense icon="delete" color="negative" @click="emit('delete', slotProps.row.id)" />
        </div>
      </q-td>
    </template>

    <!-- Mobile: card layout per row -->
    <template #item="slotProps">
      <div class="q-pa-xs col-12">
        <q-card flat bordered dark class="transaction-card">
          <q-card-section class="row items-center no-wrap q-pa-sm">
            <q-icon
              :name="typeIcon(slotProps.row.type).icon"
              :class="[typeIcon(slotProps.row.type).colorClass, 'q-mr-sm']"
              size="sm"
            />
            <div class="col">
              <div class="text-weight-medium">{{ slotProps.row.title }}</div>
              <div class="text-caption text-grey">
                {{ format(parseISO(slotProps.row.date), 'yyyy-MM-dd') }}
                · {{ slotProps.row.type }}
                · {{ getCategoryName(slotProps.row.categoryId) }}
              </div>
            </div>
            <div class="text-weight-bold q-mr-sm">
              {{ slotProps.row.amount.toFixed(2) }} €
            </div>
            <div class="transaction-table__actions">
              <q-btn flat round dense icon="visibility" @click="emit('view', slotProps.row)" />
              <q-btn flat round dense icon="edit" @click="emit('edit', slotProps.row)" />
              <q-btn flat round dense icon="delete" color="negative" @click="emit('delete', slotProps.row.id)" />
            </div>
          </q-card-section>
        </q-card>
      </div>
    </template>
  </q-table>
</template>
