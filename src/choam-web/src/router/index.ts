import { createRouter, createWebHistory } from 'vue-router'
import MainLayout from '../layouts/MainLayout.vue'
import { useAuth } from '../auth/useAuth'

const router = createRouter({
  history: createWebHistory('/'),
  routes: [
    {
      path: '/auth/callback',
      name: 'auth-callback',
      component: () => import('../pages/AuthCallbackPage.vue'),
    },
    {
      path: '/',
      component: MainLayout,
      children: [
        {
          path: '',
          name: 'ledger',
          component: () => import('../pages/TransactionsPage.vue'),
        },
        {
          path: 'chat',
          name: 'chat',
          component: () => import('../pages/ChatPage.vue'),
        },
        {
          path: 'analytics',
          name: 'analytics',
          component: () => import('../pages/AnalyticsPage.vue'),
        },
        {
          path: 'settings',
          name: 'settings',
          component: () => import('../pages/SettingsPage.vue'),
        },
      ],
    },
    {
      path: '/:pathMatch(.*)*',
      redirect: '/',
    },
  ],
})

router.beforeEach((to) => {
  if (to.name === 'auth-callback') return true

  const { isAuthenticated, isLoading, login } = useAuth()

  if (isLoading.value) return true

  if (!isAuthenticated()) {
    login()
    return false
  }

  return true
})

export default router
