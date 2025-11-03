// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Content Blocks Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.ContentBlocks',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ContentBlocks',
	},
};

const propertyEditorUi: Array<UmbExtensionManifest> = [
	{
		type: 'propertyEditorUi',
		alias: 'Umb.Contentment.PropertyEditorUi.ContentBlocks',
		name: '[Contentment] Content Blocks Property Editor UI',
		element: () => import('./content-blocks.element.js'),
		meta: {
			label: 'Content Blocks',
			icon: 'icon-fa-server',
			group: 'contentment',
			propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.ContentBlocks',
			settings: {
				properties: [
					{
						alias: 'displayMode',
						label: '#contentment_labelDisplayMode',
						description: `<uui-tag color="danger" look="primary">Not yet implemented</uui-tag>
Select and configure how to display the blocks in the editor.`,
						propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
						config: [
							{ alias: 'addButtonLabelKey', value: 'contentment_configureDisplayMode' },
							{ alias: 'configurationType', value: 'contentmentDisplayMode' },
							{ alias: 'maxItems', value: 1 },
							{ alias: 'enableDevMode', value: true },
						],
					},
					{
						alias: 'contentBlockTypes',
						label: 'Block types',
						description: 'Configure the element types to be used as blocks.',
						propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ContentBlockTypeConfiguration',
						config: [
							{ alias: 'addButtonLabelKey', value: 'contentment_configureElementType' },
							{ alias: 'enableFilter', value: true },
							{ alias: 'enableDevMode', value: true },
							{ alias: 'uiAlias', value: 'Umb.Contentment.DisplayMode.List' },
						],
					},
					{
						alias: 'enableFilter',
						label: 'Enable filter?',
						description: 'Select to enable the search filter in the overlay selection panel.',
						propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
					},
					{
						alias: 'maxItems',
						label: 'Maximum items',
						description: "Enter the number for the maximum items allowed.<br>Use '0' for an unlimited amount.",
						propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
					},
					{
						alias: 'disableSorting',
						label: 'Disable sorting?',
						description: 'Select to disable sorting of the items.',
						propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
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
	},
	{
		type: 'propertyEditorUi',
		alias: 'Umb.Contentment.PropertyEditorUi.ContentBlockTypeConfiguration',
		name: '[Contentment] Content Block Type Configuration Property Editor UI',
		element: () => import('./content-block-type-configuration.element.js'),
		meta: {
			label: 'Content Block Type Configuration',
			icon: 'icon-fa-server',
			group: 'contentment',
		},
	},
];

export const manifests = [schema, ...propertyEditorUi];
