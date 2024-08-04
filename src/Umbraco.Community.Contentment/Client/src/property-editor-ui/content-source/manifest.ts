// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ContentSource',
	name: '[Contentment] Content Source Property Editor UI',
	element: () => import('./content-source.element.js'),
	meta: {
		label: '[Contentment] Content Source',
		icon: 'icon-page-add',
		group: 'pickers',
	},
};
