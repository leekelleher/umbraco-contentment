// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const CONTENTMENT_DATA_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS = 'Umbraco.Community.Contentment.DataPicker';

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Data Picker Property Editor Schema',
	alias: CONTENTMENT_DATA_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataPicker',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DataPicker',
	name: '[Contentment] Data Picker Property Editor UI',
	element: () => import('./data-picker.element.js'),
	meta: {
		label: 'Data Picker',
		icon: 'icon-fa-arrow-pointer',
		group: 'contentment',
		propertyEditorSchemaAlias: CONTENTMENT_DATA_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
		settings: {
			properties: [
				{
					alias: 'dataSource',
					label: '#contentment_labelDataSource',
					description: '{#contentment_configureDataSource}.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
						{ alias: 'configurationType', value: 'contentmentDataSource' },
						{ alias: 'maxItems', value: 1 },
						{ alias: 'enableDevMode', value: true },
					],
				},
				{
					alias: 'displayMode',
					label: '#contentment_labelDisplayMode',
					description: 'Select display mode for the picker editor.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureDisplayMode' },
						{ alias: 'configurationType', value: 'contentmentDisplayMode' },
						{ alias: 'maxItems', value: 1 },
						{ alias: 'enableDevMode', value: true },
					],
				},
				{
					alias: 'pageSize',
					label: 'Page size',
					description: 'How many items to display per page? The default value is 12.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'overlaySize',
					label: 'Editor overlay size',
					description: "Select the size of the overlay panel. The default is 'medium'.",
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.OverlaySize',
				},
				{
					alias: 'hideSearch',
					label: 'Hide search box?',
					description: 'Hide the search box in the overlay panel.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'maxItems',
					label: 'Maximum items',
					description: "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'allowDuplicates',
					label: 'Allow duplicates?',
					description: 'Select to allow the editor to select duplicate items.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [
				{ alias: 'overlaySize', value: 'medium' },
				{ alias: 'pageSize', value: 12 },
			],
		},
	},
};

const modal: UmbExtensionManifest = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.DataPicker',
	name: '[Contentment] Data Picker Modal',
	element: () => import('./data-picker-modal.element.js'),
};

const valueSummary: UmbExtensionManifest = {
	type: 'valueSummary',
	kind: 'default',
	alias: 'Umb.Contentment.ValueSummary.PropertyEditor.DataPicker',
	name: '[Contentment] Data Picker Property Editor Value Summary',
	forValueType: CONTENTMENT_DATA_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
	element: () => import('./data-picker.value-summary.element.js'),
};

export const manifests = [schema, editorUi, modal, valueSummary];
