// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

const editorNotes: UmbExtensionManifest = {
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
									[
										'Umb.Tiptap.Toolbar.StyleSelect',
										'Umb.Tiptap.Toolbar.Bold',
										'Umb.Tiptap.Toolbar.Italic',
										'Umb.Tiptap.Toolbar.Underline',
									],
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
					alias: 'alertType',
					label: 'Style',
					description: 'Select the style of the note.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.EditorNoteStyles',
				},
				{
					alias: 'hideLabel',
					label: 'Hide label?',
					description: `<uui-tag look="placeholder">experimental</uui-tag>
Select to hide the label and have the editor take up the full width of the panel.`,
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'hidePropertyGroup',
					label: 'Move above property group container?',
					description: `<uui-tag look="placeholder">experimental</uui-tag>
Select to move the note above/outside the property group.`,
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

const editorNoteStyles: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.EditorNoteStyles',
	name: '[Contentment] Editor Note Styles Property Editor UI',
	element: () => import('./editor-note-styles.element.js'),
	meta: {
		label: 'Editor Note Styles',
		icon: 'icon-contentment',
		group: 'contentment',
	},
};

export const manifests = [editorNotes, editorNoteStyles];
