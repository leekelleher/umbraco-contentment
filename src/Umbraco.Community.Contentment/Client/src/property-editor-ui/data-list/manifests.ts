// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Data List  Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.DataList',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataList',
		settings: {
			properties: [
				{
					alias: 'dataSource',
					label: 'Data source',
					description: 'Select and configure a data source.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
						{ alias: 'configurationType', value: 'dataSource' },
						{ alias: 'maxItems', value: 1 },
						{ alias: 'enableDevMode', value: true },
					],
				},
				{
					alias: 'listEditor',
					label: 'List editor',
					description: 'Select and configure a list editor.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureListEditor' },
						{ alias: 'configurationType', value: 'listEditor' },
						{ alias: 'maxItems', value: 1 },
						{ alias: 'enableDevMode', value: true },
					],
				},
			],
		},
	},
};

const dataList: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DataList',
	name: '[Contentment] Data List Property Editor UI',
	element: () => import('./data-list.element.js'),
	meta: {
		label: 'Data List',
		icon: 'icon-fa-list-ul',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.DataList',
		settings: {
			properties: [
				{
					alias: 'preview',
					label: 'Preview',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataListPreview',
				},
			],
		},
	},
};

const dataListPreview: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.DataListPreview',
	name: '[Contentment] Data List Preview Property Editor UI',
	element: () => import('./data-list-preview.element.js'),
	meta: {
		label: 'Data List Preview',
		icon: 'icon-fa-list-ul',
		group: 'contentment',
	},
};

export const manifests = [schema, dataList, dataListPreview];
