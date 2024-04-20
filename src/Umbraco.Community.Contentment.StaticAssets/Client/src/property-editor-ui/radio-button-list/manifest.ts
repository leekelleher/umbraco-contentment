import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
	name: '[Contentment] Radio Button List',
	element: () => import('./radio-button-list.element.js'),
	meta: {
		label: '[Contentment] Radio Button List',
		icon: 'icon-target',
		group: 'lists',
	},
};
