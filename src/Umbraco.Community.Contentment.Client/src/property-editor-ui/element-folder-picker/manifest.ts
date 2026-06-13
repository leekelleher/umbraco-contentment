// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ElementFolderPicker',
	name: '[Contentment] Element Folder Picker Property Editor UI',
	element: () => import('./element-folder-picker.element.js'),
	meta: {
		label: 'Element Folder Picker',
		icon: 'icon-folder',
		group: 'contentment',
	},
};
