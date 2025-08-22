// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CodeEditor',
	name: '[Contentment] Code Editor Property Editor UI',
	element: () => import('./code-editor.element.js'),
	meta: {
		label: 'Code Editor',
		icon: 'icon-code',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Plain.String',
		settings: {
			properties: [
				{
					alias: 'mode',
					label: 'Language mode',
					description: 'Select the programming language mode. The default mode is "Razor (CSHTML)".',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'CSS', value: 'css' },
								{ name: 'HTML', value: 'html' },
								{ name: 'JavaScript', value: 'javascript' },
								{ name: 'JSON', value: 'json' },
								{ name: 'Markdown', value: 'markdown' },
								{ name: 'Razor (CSHTML)', value: 'razor' },
								{ name: 'TypeScript', value: 'typescript' },
							],
						},
					],
				},
			],
			defaultData: [{ alias: 'mode', value: 'razor' }],
		},
	},
};
