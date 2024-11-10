// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ManifestPicker',
	name: '[Contentment] Manifest Picker Property Editor UI',
	element: () => import('./manifest-picker.element.js'),
	meta: {
		label: '[Contentment] Manifest Picker',
		icon: 'icon-fa-codepen',
		group: 'pickers',
		settings: {
			properties: [
				{
					alias: 'extensionType',
					label: 'Extension type',
					description: 'Select the extension type to pick from.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
				},
			],
		},
	},
};