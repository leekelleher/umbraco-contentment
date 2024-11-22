// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DictionaryPicker',
	name: '[Contentment] Dictionary Picker Property Editor UI',
	element: () => import('./dictionary-picker.element.js'),
	meta: {
		label: 'Dictionary Picker',
		icon: 'icon-book-alt',
		group: 'contentment',
	},
};
