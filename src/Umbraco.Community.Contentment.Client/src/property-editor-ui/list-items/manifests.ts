// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const CONTENTMENT_LIST_ITEMS_PROPERTY_EDITOR_SCHEMA_ALIAS = 'Umbraco.Community.Contentment.ListItems';

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] List Items Property Editor Schema',
	alias: CONTENTMENT_LIST_ITEMS_PROPERTY_EDITOR_SCHEMA_ALIAS,
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ListItems',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ListItems',
	name: '[Contentment] List Items Property Editor UI',
	element: () => import('../list-items/list-items.element.js'),
	meta: {
		label: 'List Items',
		icon: 'icon-fa-table-list',
		group: 'contentment',
		propertyEditorSchemaAlias: CONTENTMENT_LIST_ITEMS_PROPERTY_EDITOR_SCHEMA_ALIAS,
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

const valueSummary: UmbExtensionManifest = {
	type: 'valueSummary',
	kind: 'default',
	alias: 'Umb.Contentment.ValueSummary.PropertyEditor.ListItems',
	name: '[Contentment] List Items Property Editor Value Summary',
	forValueType: CONTENTMENT_LIST_ITEMS_PROPERTY_EDITOR_SCHEMA_ALIAS,
	element: () => import('./list-items.value-summary.element.js'),
};

export const manifests = [schema, editorUi, valueSummary];
