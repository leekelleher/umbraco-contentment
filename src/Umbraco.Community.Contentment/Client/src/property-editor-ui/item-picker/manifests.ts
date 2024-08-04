// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestModal, ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

const modal: ManifestModal = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.ItemPicker',
	name: '[Contentment] Item Picker Modal',
	element: () => import('./item-picker-modal.element.js'),
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ItemPicker',
	name: '[Contentment] Item Picker Property Editor UI',
	element: () => import('./item-picker.element.js'),
	meta: {
		label: '[Contentment] Item Picker',
		icon: 'icon-fa fa-mouse-pointer',
		group: 'pickers',
	},
};

export const manifests = [modal, editorUi];
