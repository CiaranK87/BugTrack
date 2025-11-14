import { defineConfig } from 'vitest/config';

export default defineConfig({
  plugins: [],
  test: {
    environment: 'jsdom',
    globals: true,
    setupFiles: './src/test/setup.ts',
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      include: [
        'src/**/*.{ts,tsx}',
        '!src/**/*.d.ts',
        '!src/**/*.config.*',
        '!src/vite-env.d.ts'
      ],
      exclude: [
        'node_modules/',
        'src/test/**',
      ],
    },
  },
  resolve: {
    alias: {
      '@': './src',
    },
  },
});