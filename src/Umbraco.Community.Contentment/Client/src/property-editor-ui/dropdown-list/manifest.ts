// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
	name: '[Contentment] Dropdown List Property Editor UI',
	element: () => import('./dropdown-list.element.js'),
	meta: {
		label: 'Dropdown List',
		icon: 'icon-target',
		group: 'contentment',
	},
};
