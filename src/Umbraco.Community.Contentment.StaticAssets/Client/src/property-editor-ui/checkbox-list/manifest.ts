import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CheckBoxList',
	name: '[Contentment] Checkbox List Property Editor UI',
	element: () => import('./checkbox-list.element.js'),
	meta: {
		label: '[Contentment] Checkbox List',
		icon: 'icon-checkbox',
		group: 'lists',
	},
};
