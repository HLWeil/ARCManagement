import { ExampleModel, getRuntimeName } from 'arcmanagement';

const model = new ExampleModel('browser', 1);
model.Increment(1);
document.querySelector('#app')!.textContent = `${model.Name}: ${model.Value} (${getRuntimeName()})`;
