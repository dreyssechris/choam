<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import '../styles/layouts/_main-layout.scss'

const router = useRouter()

const navItems = [
  { label: 'Ledger', route: '/' },
  { label: 'Chat', route: null },
  { label: 'Analytics', route: null },
  { label: 'Settings', route: null },
]

const activeNav = ref('Ledger')

function onNav(item: { label: string; route: string | null }) {
  activeNav.value = item.label
  if (item.route) router.push(item.route)
}
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header class="glass-nav">
      <div class="nav-inner">
        <div class="nav-left">
          <span class="nav-logo font-headline">CHOAM</span>
          <nav class="nav-links">
            <a
              v-for="item in navItems"
              :key="item.label"
              :class="['nav-link font-headline', { 'nav-link--active': activeNav === item.label }]"
              href="#"
              @click.prevent="onNav(item)"
            >
              {{ item.label }}
            </a>
          </nav>
        </div>
        <div class="nav-right">
          <div class="nav-user">
            <q-icon 
              name="sym_o_account_circle"
              size="20px"
            />
            <span class="nav-user__name">Administrator</span>
          </div>
        </div>
      </div>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>
  </q-layout>
</template>
