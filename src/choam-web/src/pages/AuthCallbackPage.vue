<script setup lang="ts">
import { onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { userManager } from '../auth/authService'

const router = useRouter()

onMounted(async () => {
  await userManager.signinRedirectCallback()
  router.replace('/')
})
</script>

<template>
  <div class="flex flex-center" style="height: 100vh">
    <q-spinner-dots size="40px" color="primary" />
  </div>
</template>
