// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] List Items Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.ListItems',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ListItems',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ListItems',
	name: '[Contentment] List Items Property Editor UI',
	element: () => import('../list-items/list-items.element.js'),
	meta: {
		label: '[Contentment] List Items',
		icon: 'icon-hearts',
		group: 'lists',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.ListItems',
		settings: {
			properties: [
				{
					alias: 'hideIcon',
					label: 'Hide icon field?',
					description: 'Select to hide the icon picker.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'hideDescription',
					label: 'Hide description field?',
					description: 'Select to hide the description text field.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'confirmRemoval',
					label: 'Confirm removals?',
					description: 'Select to enable a confirmation prompt when removing an item.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'maxItems',
					label: 'Maximum items',
					description: "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [{ alias: 'maxItems', value: 0 }],
		},
	},
};

export const manifests = [schema, editorUi];
