// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.TemplatedList',
	name: '[Contentment] Templated List Property Editor UI',
	element: () => import('./templated-list.element.js'),
	meta: {
		label: '[Contentment] Templated List',
		icon: 'icon-fa-codepen',
		group: 'lists',
	},
};
