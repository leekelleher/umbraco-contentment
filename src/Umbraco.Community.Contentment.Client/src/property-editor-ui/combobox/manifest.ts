// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Combobox',
	name: '[Contentment] Combobox Property Editor UI',
	element: () => import('./combobox.element.js'),
	meta: {
		label: 'Combobox',
		icon: 'icon-box-alt',
		group: 'contentment',
	},
};
