// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

export const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Notes',
	alias: 'Umbraco.Community.Contentment.Notes',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Notes',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Notes',
	name: '[Contentment] Notes',
	element: () => import('./notes.element.js'),
	meta: {
		label: '[Contentment] Notes',
		icon: 'icon-autofill',
		group: 'display',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.Notes',
		settings: {
			properties: [
				{
					alias: 'notes',
					label: 'Notes',
					description: 'Enter the notes to be displayed for the content editor.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.TinyMCE',
				},
				{
					alias: 'hideLabel',
					label: 'Hide label?',
					description: 'Select to hide the label and have the editor take up the full width of the panel.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
		},
	},
};

export const manifests = [schema, editorUi];
