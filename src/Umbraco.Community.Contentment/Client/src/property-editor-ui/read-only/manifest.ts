// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ReadOnly',
	name: '[Contentment] Read Only Property Editor UI',
	element: () => import('./read-only.element.js'),
	meta: {
		label: '[Contentment] Read Only',
		icon: 'icon-hearts',
		group: 'display',
	},
};
