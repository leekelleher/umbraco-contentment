// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CascadingDropdownList',
	name: '[Contentment] Cascading Dropdown List Property Editor UI',
	element: () => import('./cascading-dropdown-list.element.js'),
	meta: {
		label: 'Cascading Dropdown List',
		icon: 'icon-target',
		group: 'contentment',
	},
};
