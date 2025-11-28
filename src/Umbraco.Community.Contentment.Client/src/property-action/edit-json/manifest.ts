// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION } from '../../condition/property-config-flag/constants.js';

export const manifest: UmbExtensionManifest = {
	type: 'propertyAction',
	kind: 'default',
	alias: 'Umb.Contentment.PropertyAction.EditJson',
	name: '[Contentment] Edit JSON Property Action',
	api: () => import('./edit-json.controller.js'),
	forPropertyEditorUis: [
		'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
		'Umb.Contentment.PropertyEditorUi.CodeEditor',
		'Umb.Contentment.PropertyEditorUi.ContentBlocks',
		'Umb.Contentment.PropertyEditorUi.ContentBlockTypeConfiguration',
		'Umb.Contentment.PropertyEditorUi.DataList',
		'Umb.Contentment.PropertyEditorUi.DataPicker',
		'Umb.Contentment.PropertyEditorUi.EditorNotes',
		'Umb.Contentment.PropertyEditorUi.ListItems',
		'Umb.Contentment.PropertyEditorUi.Notes',
		'Umb.Contentment.PropertyEditorUi.ReadOnly',
		'Umb.Contentment.PropertyEditorUi.SocialLinks',
		'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
		'Umb.Contentment.PropertyEditorUi.TextboxList',
	],
	meta: {
		icon: 'icon-brackets',
		label: '#contentment_editJson',
	},
	conditions: [{ alias: CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION, propertyConfigAlias: 'enableDevMode' }],
};
