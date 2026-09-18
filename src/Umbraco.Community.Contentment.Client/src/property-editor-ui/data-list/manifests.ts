// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const CONTENTMENT_DATA_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS = 'Umbraco.Community.Contentment.DataList';

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Data List Property Editor Schema',
	alias: CONTENTMENT_DATA_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS,
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DataList',
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
					alias: 'listEditor',
					label: '#contentment_labelListEditor',
					description: '{#contentment_configureListEditor}.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureListEditor' },
						{ alias: 'configurationType', value: 'contentmentListEditor' },
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
		propertyEditorSchemaAlias: CONTENTMENT_DATA_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS,
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

export const repository: UmbExtensionManifest = {
	type: 'repository',
	alias: 'Umb.Contentment.Repository.DataList',
	name: '[Contentment] Data List Repository',
	api: () => import('./data-list.repository.js'),
};

const valueSummary: UmbExtensionManifest = {
	type: 'valueSummary',
	kind: 'default',
	alias: 'Umb.Contentment.ValueSummary.PropertyEditor.DataList',
	name: '[Contentment] Data List Property Editor Value Summary',
	forValueType: CONTENTMENT_DATA_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS,
	element: () => import('./data-list.value-summary.element.js'),
};

export const manifests = [schema, dataList, dataListPreview, repository, valueSummary];
