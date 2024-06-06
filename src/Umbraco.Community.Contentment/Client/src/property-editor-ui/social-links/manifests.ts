// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Social Links Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.SocialLinks',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.SocialLinks',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.SocialLinks',
	name: '[Contentment] Social Links Property Editor UI',
	element: () => import('../read-only/read-only.element.js'),
	meta: {
		label: '[Contentment] Social Links',
		icon: 'icon-hearts',
		group: 'lists',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.SocialLinks',
		settings: {
			properties: [
				{
					alias: 'networks',
					label: 'Social networks',
					description: 'Define the icon set for the available social networks.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
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
		},
	},
};

export const manifests = [schema, editorUi];
