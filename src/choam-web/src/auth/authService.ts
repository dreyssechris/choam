import { UserManager, WebStorageStateStore } from 'oidc-client-ts'

const authority = import.meta.env.VITE_KEYCLOAK_AUTHORITY ?? 'https://auth.chrispicloud.dev/realms/choam'
const clientId = import.meta.env.VITE_KEYCLOAK_CLIENT_ID ?? 'choam-web'
const redirectUri = `${window.location.origin}/auth/callback`
const postLogoutRedirectUri = window.location.origin

export const userManager = new UserManager({
  authority,
  client_id: clientId,
  redirect_uri: redirectUri,
  post_logout_redirect_uri: postLogoutRedirectUri,
  response_type: 'code',
  scope: 'openid profile email',
  automaticSilentRenew: true,
  userStore: new WebStorageStateStore({ store: localStorage }),
})
