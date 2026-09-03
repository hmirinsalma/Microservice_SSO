import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    host: '0.0.0.0', // ✅ Écouter sur toutes les interfaces (IPv4 + IPv6)
    port: 5173,
    strictPort: true,
    proxy: {
      '/api': {
        target: 'http://localhost:5137',
        changeOrigin: true,
        secure: false,
      },
    },
  },
})
