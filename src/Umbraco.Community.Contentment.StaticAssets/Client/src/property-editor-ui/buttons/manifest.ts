import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Buttons',
	name: '[Contentment] Buttons Property Editor UI',
	element: () => import('./buttons.element.js'),
	meta: {
		label: '[Contentment] Buttons',
		icon: 'icon-tab',
		group: 'lists',
	},
};
