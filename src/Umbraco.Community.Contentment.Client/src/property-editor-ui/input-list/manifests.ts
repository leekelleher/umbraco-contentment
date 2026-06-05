// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

const dataTypesEditorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.InputListColumnsConfiguration',
	name: '[Contentment] Input List: Columns Configuration Property Editor UI',
	element: () => import('./input-list-columns-configuration.element.js'),
	meta: {
		label: 'Input List Columns Configuration',
		icon: 'icon-list',
		group: 'contentment',
	},
};

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Input List Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.InputList',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.InputList',
		settings: {
			properties: [
				{
					alias: 'columns',
					label: 'Columns',
					description: 'Configure the data type for each column in the list.',
					propertyEditorUiAlias: dataTypesEditorUi.alias,
					config: [{ alias: 'enableDevMode', value: true }],
				},
			],
		},
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: schema.meta.defaultPropertyEditorUiAlias,
	name: '[Contentment] Input List Property Editor UI',
	element: () => import('./input-list.element.js'),
	meta: {
		label: 'Input List',
		icon: 'icon-list',
		group: 'contentment',
		propertyEditorSchemaAlias: schema.alias,
		settings: {
			properties: [
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
		},
	},
};

export const manifests = [schema, dataTypesEditorUi, editorUi];
