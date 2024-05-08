// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.EditorNotes',
	name: '[Contentment] Editor Notes',
	element: () => import('./editor-notes.element.js'),
	meta: {
		label: '[Contentment] Editor Notes',
		icon: 'icon-alert-alt',
		group: 'display',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.Notes',
		settings: {
			properties: [
				{
					alias: 'alertType',
					label: 'Alert type',
					description: '',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Default', value: 'default', icon: 'icon-circle-dotted color-blue' },
								{ name: 'Positive', value: 'positive', icon: 'icon-circle-dotted-active color-green' },
								{ name: 'Warning', value: 'warning', icon: 'icon-alert color-orange' },
								{ name: 'Danger', value: 'danger', icon: 'icon-application-error color-red' },
							],
						},
						{ alias: 'showIcons', value: true },
					],
				},
				{
					alias: 'icon',
					label: 'Icon',
					description: 'Select an icon to be displayed next to the message.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.IconPicker',
				},
				{
					alias: 'heading',
					label: 'Heading',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
				},
				{
					alias: 'message',
					label: 'Message',
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
			defaultData: [
				{
					alias: 'alertType',
					value: 'warning',
				},
			],
		},
	},
};
