// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Textbox List Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.TextboxList',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.TextboxList',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.TextboxList',
	name: '[Contentment] Textbox List Property Editor UI',
	element: () => import('../read-only/read-only.element.js'),
	meta: {
		label: '[Contentment] Textbox List',
		icon: 'icon-hearts',
		group: 'common',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.TextboxList',
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
					],
				},
				{
					alias: 'defaultIcon',
					label: 'Default icon',
					description:
						'Select an icon to be displayed as the default icon,<br><em>(for when no icon is available)</em>.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
				},
				{
					alias: 'labelStyle',
					label: 'Label style',
					description: "Select the style of the item's label.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Icon and Text', value: 'both', description: "Displays both the item's icon and name." },
								{ name: 'Icon only', value: 'icon', description: "Hides the item's name and only displays the icon." },
								{ name: 'Text only', value: 'text', description: "Hides the item's icon and only displays the name." },
							],
						},
						{ alias: 'showDescriptions', value: true },
					],
				},

				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [
				{ alias: 'labelStyle', value: 'both' },
				{ alias: 'defaultIcon', value: 'icon-document' },
			],
		},
	},
};

export const manifests = [schema, editorUi];
