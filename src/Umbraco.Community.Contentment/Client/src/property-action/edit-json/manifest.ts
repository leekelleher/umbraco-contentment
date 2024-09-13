// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION } from '../../condition/property-config-flag/constants.js';
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
		label: '#contentment_editJson',
	},
	// @ts-ignore: Unable to extend core's `ConditionTypes` type [LK]
	conditions: [{ alias: CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION, propertyConfigAlias: 'enableDevMode' }],
};
