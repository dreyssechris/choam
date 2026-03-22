import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { quasar, transformAssetUrls } from '@quasar/vite-plugin'

const isRemoteDev = process.env.VITE_HMR_HOST != null

export default defineConfig({
  base: '/financetracker/',

  plugins: [
    vue({
      template: { transformAssetUrls },
    }),
    quasar({
      sassVariables: fileURLToPath(
        new URL('./src/styles/quasar-variables.scss', import.meta.url),
      ),
    }),
  ],

  server: {
    host: true,
    strictPort: true,
    watch: {
      usePolling: true,
      interval: 1000,
    },
    allowedHosts: [
      'localhost',
      '127.0.0.1',
      '192.168.0.168',
      'financetracker-web',
      'financetracker-web-dev',
      'chrispicloud.dev',
      'dev.chrispicloud.dev',
    ],
    hmr: isRemoteDev
      ? {
          protocol: 'wss',
          host: process.env.VITE_HMR_HOST,
          port: 5173,
          clientPort: 443,
          path: '/financetracker',
        }
      : true,
    proxy: {
      '/financetracker/api': {
        target: 'http://api:8080',
        changeOrigin: true,
        rewrite: (p: string) => p.replace(/^\/financetracker\/api/, ''),
      },
    },
  },
})
