// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.EditorNotes',
	name: '[Contentment] Editor Notes Property Editor UI',
	element: () => import('./editor-notes.element.js'),
	meta: {
		label: 'Editor Notes',
		icon: 'icon-alert',
		group: 'contentment',
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
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
					config: [{ alias: 'hideColors', value: true }],
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
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Tiptap',
					config: [
						{
							alias: 'extensions',
							value: [
								'Umb.Tiptap.Embed',
								'Umb.Tiptap.Link',
								'Umb.Tiptap.Figure',
								'Umb.Tiptap.Image',
								'Umb.Tiptap.Subscript',
								'Umb.Tiptap.Superscript',
								'Umb.Tiptap.Table',
								'Umb.Tiptap.Underline',
								'Umb.Tiptap.TextAlign',
								'Umb.Tiptap.MediaUpload',
							],
						},
						{
							alias: 'toolbar',
							value: [
								[
									['Umb.Tiptap.Toolbar.SourceEditor'],
									['Umb.Tiptap.Toolbar.Bold', 'Umb.Tiptap.Toolbar.Italic', 'Umb.Tiptap.Toolbar.Underline'],
									[
										'Umb.Tiptap.Toolbar.TextAlignLeft',
										'Umb.Tiptap.Toolbar.TextAlignCenter',
										'Umb.Tiptap.Toolbar.TextAlignRight',
									],
									['Umb.Tiptap.Toolbar.BulletList', 'Umb.Tiptap.Toolbar.OrderedList'],
									['Umb.Tiptap.Toolbar.Blockquote', 'Umb.Tiptap.Toolbar.HorizontalRule'],
									['Umb.Tiptap.Toolbar.Link', 'Umb.Tiptap.Toolbar.Unlink'],
									['Umb.Tiptap.Toolbar.MediaPicker', 'Umb.Tiptap.Toolbar.EmbeddedMedia'],
								],
							],
						},
						{ alias: 'maxImageSize', value: 500 },
					],
				},
				{
					alias: 'hideLabel',
					label: 'Hide label?',
					description: 'Select to hide the label and have the editor take up the full width of the panel.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'hidePropertyGroup',
					label: 'Move above property group container?',
					description: '<em>(experimental)</em> Select to move the note above/outside the property group.',
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
