import { ExampleModel, getRuntimeName } from 'polyglottemplate';

const model = new ExampleModel('browser', 1);
model.Increment(1);
document.querySelector('#app')!.textContent = `${model.Name}: ${model.Value} (${getRuntimeName()})`;
