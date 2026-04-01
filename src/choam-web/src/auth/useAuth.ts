import { ref, readonly } from 'vue'
import type { User } from 'oidc-client-ts'
import { userManager } from './authService'

const user = ref<User | null>(null)
const isLoading = ref(true)

// Try to load existing session on app start
userManager.getUser().then((u) => {
  user.value = u && !u.expired ? u : null
  isLoading.value = false
})

userManager.events.addUserLoaded((u) => {
  user.value = u
})

userManager.events.addUserUnloaded(() => {
  user.value = null
})

userManager.events.addAccessTokenExpired(() => {
  user.value = null
})

export function useAuth() {
  const isAuthenticated = () => !!user.value && !user.value.expired

  const login = () => userManager.signinRedirect()

  const logout = () =>
    userManager.signoutRedirect({ post_logout_redirect_uri: window.location.origin })

  const getAccessToken = () => user.value?.access_token ?? null

  const userName = () =>
    user.value?.profile?.preferred_username ?? user.value?.profile?.name ?? ''

  const roles = () => {
    const r = user.value?.profile?.roles
    return Array.isArray(r) ? (r as string[]) : []
  }

  const isDirector = () => roles().includes('director')

  return {
    user: readonly(user),
    isLoading: readonly(isLoading),
    isAuthenticated,
    login,
    logout,
    getAccessToken,
    userName,
    roles,
    isDirector,
  }
}
