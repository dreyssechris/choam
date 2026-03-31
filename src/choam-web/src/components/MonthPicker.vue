<template>
  <div class="month-picker">
    <div class="month-picker__container">
      <button
        class="month-picker__arrow"
        aria-label="previous month"
        @click="emit('previous')"
      >
        <q-icon
          name="sym_o_chevron_left"
          size="24px"
        />
      </button>

      <div class="month-picker__display">
        <div class="month-picker__label">
          <span class="month-picker__text font-headline">{{ MONTHS[month] }} {{ year }}</span>
          <q-icon
            name="sym_o_expand_more"
            class="month-picker__chevron"
          />
        </div>
        <div class="month-picker__underline" />

        <q-menu
          anchor="bottom middle"
          self="top middle"
        >
          <q-list dense>
            <q-item
              v-for="(m, i) in MONTHS"
              :key="i"
              v-close-popup
              clickable
              :active="i === month"
              active-class="text-sand-gold"
              @click="emit('update:month', i)"
            >
              <q-item-section>{{ m }} {{ year }}</q-item-section>
            </q-item>
          </q-list>
        </q-menu>
      </div>

      <button
        class="month-picker__arrow"
        aria-label="next month"
        @click="emit('next')"
      >
        <q-icon
          name="sym_o_chevron_right"
          size="24px"
        />
      </button>
    </div>
  </div>
</template>

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
