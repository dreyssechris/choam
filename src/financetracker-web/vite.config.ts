import { fileURLToPath } from 'node:url'
import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'
import { quasar, transformAssetUrls } from '@quasar/vite-plugin'
import { VitePWA } from 'vite-plugin-pwa'

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
    VitePWA({
      registerType: 'prompt',
      scope: '/financetracker/',
      manifest: {
        name: 'Financetracker',
        short_name: 'Financetracker',
        description: 'Personal finance tracker',
        theme_color: '#0b0c10',
        background_color: '#0b0c10',
        display: 'standalone',
        scope: '/financetracker/',
        start_url: '/financetracker/',
        icons: [
          {
            src: 'icons/pwa-192x192.png',
            sizes: '192x192',
            type: 'image/png',
          },
          {
            src: 'icons/pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
          },
          {
            src: 'icons/pwa-512x512.png',
            sizes: '512x512',
            type: 'image/png',
            purpose: 'maskable',
          },
        ],
      },
      workbox: {
        navigateFallback: '/financetracker/index.html',
        navigateFallbackAllowlist: [/^\/financetracker\//],
        runtimeCaching: [
          {
            urlPattern: /\/financetracker\/api\/.*/,
            handler: 'NetworkFirst',
            options: {
              cacheName: 'api-cache',
              expiration: { maxEntries: 50, maxAgeSeconds: 60 * 60 * 24 },
              networkTimeoutSeconds: 10,
            },
          },
          {
            urlPattern: /\.(?:js|css|woff2?)$/,
            handler: 'CacheFirst',
            options: {
              cacheName: 'static-assets',
              expiration: {
                maxEntries: 100,
                maxAgeSeconds: 60 * 60 * 24 * 365,
              },
            },
          },
          {
            urlPattern: /\.(?:png|jpg|jpeg|gif|svg|ico)$/,
            handler: 'CacheFirst',
            options: {
              cacheName: 'image-assets',
              expiration: { maxEntries: 50, maxAgeSeconds: 60 * 60 * 24 * 30 },
            },
          },
        ],
      },
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
