import { describe, expect, it } from 'vitest';
import { ExampleModel, getRuntimeName } from 'polyglottemplate';

describe('public npm model', () => {
  it('exposes attached properties and methods', () => {
    const model = new ExampleModel('example', 2);
    expect(model.Name).toBe('example');
    const copy = model.Copy();
    model.Name = 'changed';
    model.Increment(3);
    expect(model.Value).toBe(5);
    expect(copy.Name).toBe('example');
    expect(copy.Value).toBe(2);
    model.Value = 0;
    expect(model.Value).toBe(0);
  });

  it('uses the JavaScript compiler branch', () => {
    expect(getRuntimeName()).toBe('JavaScript');
  });
});
