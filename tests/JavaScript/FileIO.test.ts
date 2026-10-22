import { mkdtemp, rm } from 'node:fs/promises';
import { tmpdir } from 'node:os';
import { join } from 'node:path';
import { spawnSync } from 'node:child_process';
import { describe, expect, it } from 'vitest';
import { ensureDirectoryAsync, readTextAsync, writeTextAsync } from 'polyglottemplate';

describe('public npm file helpers', () => {
  it('imports the model without Node globals and rejects browser file I/O', () => {
    const script = `
      import assert from 'node:assert/strict';
      import { readFileSync } from 'node:fs';
      import { SourceTextModule, createContext } from 'node:vm';
      const module = new SourceTextModule(readFileSync('dist/ts/index.js', 'utf8'), {
        context: createContext({ console }),
      });
      await module.link(() => { throw new Error('Unexpected static import'); });
      await module.evaluate();
      const model = new module.namespace.ExampleModel('browser', 1);
      model.Increment(2);
      assert.equal(model.Value, 3);
      await assert.rejects(module.namespace.readTextAsync('example.txt'), /unavailable in the browser/);
    `;
    const result = spawnSync(process.execPath, ['--experimental-vm-modules', '--input-type=module', '--eval', script], {
      encoding: 'utf8',
    });
    expect(result.status, result.stderr).toBe(0);
  });

  it('roundtrips UTF-8, overwrites, and propagates errors', async () => {
    const directory = await mkdtemp(join(tmpdir(), 'polyglot-template-native-'));
    try {
      const nested = join(directory, 'nested', 'child');
      await ensureDirectoryAsync(nested);
      const path = join(nested, 'example.txt');
      const text = 'Hello, 世界!\nGrüße 👋\n';
      await writeTextAsync(path, text);
      expect(await readTextAsync(path)).toBe(text);
      await writeTextAsync(path, 'replacement');
      expect(await readTextAsync(path)).toBe('replacement');
      await writeTextAsync(path, '');
      expect(await readTextAsync(path)).toBe('');
      await expect(readTextAsync(join(directory, 'missing.txt'))).rejects.toThrow();
      await expect(writeTextAsync(join(directory, 'missing', 'file.txt'), 'text')).rejects.toThrow();
    } finally {
      await rm(directory, { recursive: true, force: true });
    }
  });
});
