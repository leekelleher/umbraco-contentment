// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestModal, ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

const modal: ManifestModal = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.ConfigurationEditor',
  name: '[Contentment] Configuration Editor Modal',
	element: () => import('./configuration-editor-modal.element.js'),
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
	name: '[Contentment] Configuration Editor Property Editor UI',
	element: () => import('./configuration-editor.element.js'),
	meta: {
		label: '[Contentment] Configuration Editor',
		icon: 'icon-settings-alt',
		group: 'data',
	},
};

export const manifests = [modal, editorUi];
