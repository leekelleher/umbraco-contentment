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

const editorUi: UmbExtensionManifest = {
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
					description: 'Select and configure how to display the blocks in the editor.',
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
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ReadOnly',
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
};

const selectionModal: UmbExtensionManifest = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.ContentBlock.Selection',
	name: '[Contentment] Content Block Selection Modal',
	element: () => import('./content-block-selection-modal.element.js'),
};

const workspaceModal: UmbExtensionManifest = {
	type: 'modal',
	alias: 'Umb.Contentment.Modal.ContentBlock.Workspace',
	name: '[Contentment] Content Block Workspace Modal',
	element: () => import('./content-block-workspace-modal.element.js'),
};

export const manifests = [schema, editorUi, selectionModal, workspaceModal];
