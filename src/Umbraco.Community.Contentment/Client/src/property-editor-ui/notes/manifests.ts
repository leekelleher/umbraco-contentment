// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Notes Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.Notes',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Notes',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Notes',
	name: '[Contentment] Notes Property Editor UI',
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
					config: [
						{
							alias: 'toolbar',
							value: [
								'styles',
								'bold',
								'italic',
								'alignleft',
								'aligncenter',
								'alignright',
								'bullist',
								'numlist',
								'outdent',
								'indent',
								'sourcecode',
								'link',
								'umbmediapicker',
								'umbembeddialog',
							],
						},
						{ alias: 'mode', value: 'Classic' },
						{ alias: 'maxImageSize', value: 500 },
					],
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
