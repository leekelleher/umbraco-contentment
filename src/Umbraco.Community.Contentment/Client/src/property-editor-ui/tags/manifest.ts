// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Tags',
	name: '[Contentment] Tags Property Editor UI',
	element: () => import('./tags.element.js'),
	meta: {
		label: '[Contentment] Tags',
		icon: 'icon-tags',
		group: 'lists',
	},
};
