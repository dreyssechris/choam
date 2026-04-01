<script setup lang="ts">
import { ref } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import { useAuth } from '../auth/useAuth'
import '../styles/layouts/_main-layout.scss'

const router = useRouter()
const route = useRoute()
const { userName, logout } = useAuth()

const avatars = ['/avatars/cloak.png', '/avatars/fremen-mask.png', '/avatars/tleilaxu.png']

function getRandomAvatar(): string {
  const stored = sessionStorage.getItem('choam-avatar')
  if (stored) return stored
  const pick = avatars[Math.floor(Math.random() * avatars.length)]
  sessionStorage.setItem('choam-avatar', pick)
  return pick
}

const userAvatar = ref(getRandomAvatar())

const navItems = [
  { label: 'Ledger', icon: 'sym_o_receipt_long', route: '/' },
  { label: 'Chat', icon: 'sym_o_chat', route: '/chat' },
  { label: 'Analytics', icon: 'sym_o_bar_chart', route: '/analytics' },
  { label: 'Settings', icon: 'sym_o_settings', route: '/settings' },
]
</script>

<template>
  <q-layout view="hHh lpR fFf">
    <q-header class="glass-nav">
      <div class="nav-inner">
        <div class="nav-left">
          <a
            class="nav-logo font-headline"
            href="#"
            @click.prevent="router.push('/')"
          >CHOAM</a>
          <nav class="nav-links">
            <a
              v-for="item in navItems.slice(1)"
              :key="item.label"
              :class="['nav-link font-headline', { 'nav-link--active': route.path === item.route }]"
              href="#"
              @click.prevent="router.push(item.route)"
            >
              {{ item.label }}
            </a>
          </nav>
        </div>
        <div class="nav-right">
          <div class="nav-user">
            <img
              :src="userAvatar"
              alt="Avatar"
              class="nav-user__avatar"
              style="cursor: pointer"
              @click="logout"
            />
            <span class="nav-user__name">{{ userName() }}</span>
          </div>
        </div>
      </div>
    </q-header>

    <q-page-container>
      <router-view />
    </q-page-container>

    <nav class="bottom-nav">
      <a
        v-for="item in navItems"
        :key="item.label"
        :class="['bottom-nav__item', { 'bottom-nav__item--active': route.path === item.route }]"
        href="#"
        @click.prevent="router.push(item.route)"
      >
        <q-icon :name="item.icon" size="20px" />
        <span class="bottom-nav__label">{{ item.label }}</span>
      </a>
    </nav>
  </q-layout>
</template>
