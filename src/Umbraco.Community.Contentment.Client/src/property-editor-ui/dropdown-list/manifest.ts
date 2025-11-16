// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
	name: '[Contentment] Dropdown List Property Editor UI',
	element: () => import('./dropdown-list.element.js'),
	meta: {
		label: 'Dropdown List',
		icon: 'icon-fa-square-caret-down',
		group: 'contentment',
	},
};
