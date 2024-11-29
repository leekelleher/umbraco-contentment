// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Data Picker Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.DataPicker',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataPicker',
	},
};

const editorUis: Array<UmbExtensionManifest> = [
	{
		type: 'propertyEditorUi',
		alias: 'Umb.Contentment.PropertyEditorUi.DataPicker',
		name: '[Contentment] Data Picker Property Editor UI',
		element: () => import('./data-picker.element.js'),
		meta: {
			label: 'Data Picker',
			icon: 'icon-fa-arrow-pointer',
			group: 'contentment',
			propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.DataPicker',
			settings: {
				properties: [
					{
						alias: 'dataSource',
						label: 'Data source',
						description: 'Select and configure a data source.',
						propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
						config: [
							{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
							{ alias: 'configurationType', value: 'dataPickerSource' },
							{ alias: 'maxItems', value: 1 },
							{ alias: 'enableDevMode', value: true },
						],
					},
					{
						alias: 'displayMode',
						label: 'Display mode',
						description: 'Select display mode for the picker editor.',
						propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
						config: [
							{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
							{ alias: 'configurationType', value: 'dataPickerDisplayMode' },
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
					{ alias: 'allowDuplicates', value: true },
				],
			},
		},
	},
	{
		type: 'propertyEditorUi',
		alias: 'Umb.Contentment.PropertyEditorUi.DataPicker.Cards',
		name: '[Contentment] Data Picker (Cards) Property Editor UI',
		element: () => import('./data-picker-cards.element.js'),
		meta: {
			label: 'Data Picker (Cards)',
			icon: 'icon-playing-cards',
			group: 'contentment',
		},
	},
	{
		type: 'propertyEditorUi',
		alias: 'Umb.Contentment.PropertyEditorUi.DataPicker.List',
		name: '[Contentment] Data Picker (List) Property Editor UI',
		element: () => import('./data-picker-list.element.js'),
		meta: {
			label: 'Data Picker (List)',
			icon: 'icon-fa-list-ul',
			group: 'contentment',
		},
	},
];

const modal: UmbExtensionManifest = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.DataPicker',
	name: '[Contentment] Data Picker Modal',
	element: () => import('./data-picker-modal.element.js'),
};

export const manifests = [schema, ...editorUis, modal];
