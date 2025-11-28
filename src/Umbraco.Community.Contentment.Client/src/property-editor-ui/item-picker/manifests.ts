// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const modal: UmbExtensionManifest = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.ItemPicker',
	name: '[Contentment] Item Picker Modal',
	element: () => import('./item-picker-modal.element.js'),
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ItemPicker',
	name: '[Contentment] Item Picker Property Editor UI',
	element: () => import('./item-picker.element.js'),
	meta: {
		label: 'Item Picker',
		icon: 'icon-fa-arrow-pointer',
		group: 'contentment',
	},
};

export const manifests = [modal, editorUi];
