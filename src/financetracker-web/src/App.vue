<script setup lang="ts">
import { VueQueryDevtools } from '@tanstack/vue-query-devtools'
import { usePwaUpdate } from './composables/usePwaUpdate'

usePwaUpdate()
</script>

<template>
  <router-view />
  <VueQueryDevtools />
</template>
