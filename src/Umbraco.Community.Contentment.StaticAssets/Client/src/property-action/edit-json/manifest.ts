// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestPropertyAction } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyAction = {
	type: 'propertyAction',
	kind: 'default',
	alias: 'Umb.Contentment.PropertyAction.EditJson',
	name: '[Contentment] Edit JSON Property Action',
	api: () => import('./edit-json.controller.js'),
	forPropertyEditorUis: [
		'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
		'Umb.Contentment.PropertyEditorUi.CodeEditor',
		'Umb.Contentment.PropertyEditorUi.DataList',
		'Umb.Contentment.PropertyEditorUi.EditorNotes',
		'Umb.Contentment.PropertyEditorUi.Notes',
	],
	meta: {
		icon: 'icon-brackets',
		label: 'Edit raw value',
	},
};
