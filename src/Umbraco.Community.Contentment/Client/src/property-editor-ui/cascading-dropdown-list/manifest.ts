import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CascadingDropdownList',
	name: '[Contentment] Cascading Dropdown List Property Editor UI',
	element: () => import('./cascading-dropdown-list.element.js'),
	meta: {
		label: '[Contentment] Cascading Dropdown List',
		icon: 'icon-target',
		group: 'lists',
	},
};
