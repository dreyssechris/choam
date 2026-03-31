import { createApp } from 'vue'
import { Quasar, Dark, Notify } from 'quasar'
import { VueQueryPlugin, QueryClient } from '@tanstack/vue-query'

import '@quasar/extras/material-symbols-outlined/material-symbols-outlined.css'
import '@quasar/extras/material-icons/material-icons.css'
import 'quasar/src/css/index.sass'

import App from './App.vue'
import router from './router'
import './styles/globals.scss'

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, refetchOnWindowFocus: false },
  },
})

const app = createApp(App)

app.use(Quasar, {
  plugins: { Dark, Notify },
  config: {
    dark: true,
  },
})

app.use(router)
app.use(VueQueryPlugin, { queryClient })

app.mount('#app')
