<script setup lang="ts">
import '../styles/components/_month-picker.scss'

defineProps<{
  year: number
  month: number
}>()

const emit = defineEmits<{
  previous: []
  next: []
  'update:month': [value: number]
}>()

const MONTHS = [
  'Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
  'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec',
]
</script>

<template>
  <div class="row items-center justify-center q-gutter-sm month-picker">
    <q-btn
      flat
      round
      icon="chevron_left"
      aria-label="previous month"
      @click="emit('previous')"
    />
    <q-select
      :model-value="month"
      :options="MONTHS.map((m, i) => ({ label: `${m} ${year}`, value: i }))"
      emit-value
      map-options
      dense
      borderless
      class="month-picker__select"
      @update:model-value="(v: number) => emit('update:month', v)"
    />
    <q-btn
      flat
      round
      icon="chevron_right"
      aria-label="next month"
      @click="emit('next')"
    />
  </div>
</template>
