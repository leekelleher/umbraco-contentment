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
					label: 'Language',
					description: 'Select the programming language. The default language is "Razor (CSHTML)".',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'C#', value: 'csharp' },
								{ name: 'CSS', value: 'css' },
								{ name: 'HTML', value: 'html' },
								{ name: 'JavaScript', value: 'javascript' },
								{ name: 'JSON', value: 'json' },
								{ name: 'Liquid', value: 'liquid' },
								{ name: 'Markdown', value: 'markdown' },
								{ name: 'Razor (CSHTML)', value: 'razor' },
								{ name: 'SQL', value: 'sql' },
								{ name: 'TypeScript', value: 'typescript' },
								{ name: 'XML', value: 'xml' },
								{ name: 'YAML', value: 'yaml' },
							],
						},
					],
				},
			],
			defaultData: [{ alias: 'mode', value: 'razor' }],
		},
	},
};
