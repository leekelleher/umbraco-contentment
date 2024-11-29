// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Render Macro',
	alias: 'Umbraco.Community.Contentment.RenderMacro',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.RenderMacro',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.RenderMacro',
	name: '[Contentment] Render Macro Property Editor UI',
	element: () => import('./render-macro.element.js'),
	meta: {
		label: 'Render Macro',
		icon: 'icon-box',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.RenderMacro',
		settings: {
			properties: [
				{
					alias: 'notes',
					label: 'Notes',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.EditorNotes',
					config: [
						{ alias: 'alertType', value: 'warning' },
						{ alias: 'icon', value: 'icon-alert' },
						{ alias: 'heading', value: 'Render Macro has been deprecated' },
						{
							alias: 'message',
							value:
								'<p><em>Support for Macros were deprecated in Umbraco 14. Please consider alternative functionality.</em></p>',
						},
						{ alias: 'hideLabel', value: true },
					],
				},
			],
		},
	},
};

export const manifests = [schema, editorUi];
