// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ManifestCondition } from '@umbraco-cms/backoffice/extension-api';
import type { ManifestPropertyAction, ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';
import { CONTENTMENT_DEVELOPER_MODE_CONDITION } from './edit-json.condition.js';

const condition: ManifestCondition = {
	type: 'condition',
	name: '[Contentment] Developer Mode Condition',
	alias: CONTENTMENT_DEVELOPER_MODE_CONDITION,
	api: () => import('./edit-json.condition.js'),
};

const propertyAction: ManifestPropertyAction = {
	type: 'propertyAction',
	kind: 'default',
	alias: 'Umb.Contentment.PropertyAction.EditJson',
	name: '[Contentment] Edit JSON Property Action',
	api: () => import('./edit-json.controller.js'),
	forPropertyEditorUis: [
		'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
		'Umb.Contentment.PropertyEditorUi.CodeEditor',
		'Umb.Contentment.PropertyEditorUi.ContentBlocks',
		'Umb.Contentment.PropertyEditorUi.DataList',
		'Umb.Contentment.PropertyEditorUi.DataPicker',
		'Umb.Contentment.PropertyEditorUi.EditorNotes',
		'Umb.Contentment.PropertyEditorUi.ListItems',
		'Umb.Contentment.PropertyEditorUi.Notes',
		'Umb.Contentment.PropertyEditorUi.SocialLinks',
		'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
	],
	meta: {
		icon: 'icon-brackets',
		label: 'Edit raw value',
	},
	conditions: [{ alias: CONTENTMENT_DEVELOPER_MODE_CONDITION }],
};

export const manifests: Array<ManifestTypes> = [condition, propertyAction];
