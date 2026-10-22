import { fileURLToPath } from 'node:url';
import { defineConfig } from 'vitest/config';
import dts from 'vite-plugin-dts';

export default defineConfig({
  plugins: [
    dts({
      entryRoot: 'src/PolyglotTemplate',
      include: ['src/PolyglotTemplate/**/*.ts'],
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
      entry: fileURLToPath(new URL('./src/PolyglotTemplate/index.ts', import.meta.url)),
      name: 'PolyglotTemplate',
      formats: ['es'],
      fileName: () => 'index.js',
    },
    rolldownOptions: {
      external: [/^node:/],
    },
  },
  test: {
    include: ['tests/PolyglotTemplate/ts/Main.fs.ts', 'tests/JavaScript/**/*.test.ts'],
    environment: 'node',
    allowOnly: process.env.FAIL_ON_FOCUSED_TESTS !== '1',
    testTimeout: 30_000,
  },
});
