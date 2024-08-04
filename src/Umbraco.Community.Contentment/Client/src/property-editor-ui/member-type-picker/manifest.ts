// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.MemberTypePicker',
	name: '[Contentment] Member Type Picker Property Editor UI',
	element: () => import('./member-type-picker.element.js'),
	meta: {
		label: '[Contentment] Member Type Picker',
		icon: 'icon-checkbox',
		group: 'pickers',
	},
};
