// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

const schema: UmbExtensionManifest = {
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
		label: 'Notes',
		icon: 'icon-autofill',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.Notes',
		settings: {
			properties: [
				{
					alias: 'notes',
					label: 'Notes',
					description: 'Enter the notes to be displayed for the content editor.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Tiptap',
					config: [
						{
							alias: 'extensions',
							value: [
								'Umb.Tiptap.RichTextEssentials',
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
		},
	},
};

export const manifests = [schema, editorUi];
