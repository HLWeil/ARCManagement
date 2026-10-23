import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vitest/config';
import dts from 'vite-plugin-dts';

export default defineConfig({
  plugins: [
    dts({
      entryRoot: 'src/ARCManagement',
      include: ['src/ARCManagement/**/*.ts'],
      bundleTypes: false,
      afterDiagnostic(diagnostics) {
        if (diagnostics.some(diagnostic => diagnostic.category === 1)) {
          throw new Error('Declaration generation failed.');
        }
      },
    }),
  ],
  build: {
    outDir: 'dist/ts',
    target: 'es2022',
    minify: false,
    lib: {
      entry: fileURLToPath(new URL('./src/ARCManagement/index.ts', import.meta.url)),
      name: 'ARCManagement',
      formats: ['es'],
      fileName: () => 'index.js',
    },
    rolldownOptions: {
      external: [/^node:/],
    },
  },
  test: {
    include: ['tests/ARCManagement/ts/Main.fs.ts', 'tests/JavaScript/**/*.test.ts'],
    environment: 'node',
    allowOnly: process.env.FAIL_ON_FOCUSED_TESTS !== '1',
    testTimeout: 30_000,
  },
});
