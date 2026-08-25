import { resolve } from 'node:path'
import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'
import dts from 'vite-plugin-dts'

// Library mode: design-system is a shared component package consumed by
// employee-portal / agent-workspace / admin-console, not a deployable app
// itself — see docs/architecture/repository-structure.md.
export default defineConfig({
  plugins: [react(), dts({ tsconfigPath: 'tsconfig.app.json' })],
  build: {
    lib: {
      entry: resolve(import.meta.dirname, 'src/index.ts'),
      name: 'ItsmDesignSystem',
      fileName: 'index',
    },
    rollupOptions: {
      external: [/^react(\/.*)?$/, /^react-dom(\/.*)?$/],
    },
  },
})
