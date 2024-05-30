import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
	name: '[Contentment] Dropdown List Property Editor UI',
	element: () => import('./dropdown-list.element.js'),
	meta: {
		label: '[Contentment] Dropdown List',
		icon: 'icon-target',
		group: 'lists',
	},
};
