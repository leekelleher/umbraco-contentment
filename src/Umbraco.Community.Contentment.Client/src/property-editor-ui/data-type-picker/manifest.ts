// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DataTypePicker',
	name: '[Contentment] Data Type Picker Property Editor UI',
	element: () => import('./data-type-picker.element.js'),
	meta: {
		label: 'Data Type Picker',
		icon: 'icon-autofill',
		group: 'contentment',
	},
};
