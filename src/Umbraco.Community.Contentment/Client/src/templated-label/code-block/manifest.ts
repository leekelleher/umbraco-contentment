import { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export const manifest: ManifestElement = {
	type: 'contentmentTemplatedLabel',
	alias: 'Umb.Contentment.TemplatedLabel.CodeBlock',
	name: '[Contentment] Code Block Templated Label',
	element: () => import('./code-block.element.js'),
};
