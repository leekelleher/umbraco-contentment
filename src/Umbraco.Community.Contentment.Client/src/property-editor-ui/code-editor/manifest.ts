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
				{
					alias: 'fontSize',
					label: 'Font size',
					description: 'Set the font size for the code editor. The default size is "small".',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Extra extra small', value: 'xx-small' },
								{ name: 'Extra small', value: 'x-small' },
								{ name: 'Small', value: 'small' },
								{ name: 'Medium', value: 'medium' },
								{ name: 'Large', value: 'large' },
								{ name: 'Extra large', value: 'x-large' },
								{ name: 'Extra extra large', value: 'xx-large' },
								{ name: 'Extra extra extra large', value: 'xxx-large' },
							],
						},
					],
				},
				{
					alias: 'lineNumbers',
					label: 'Show line numbers',
					description: 'Select to show line numbers in the code editor.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'wordWrap',
					label: 'Word wrapping',
					description: 'Select to enable word wrapping in the code editor.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [
				{ alias: 'mode', value: 'razor' },
				{ alias: 'fontSize', value: 'small' },
				{ alias: 'lineNumbers', value: true },
				{ alias: 'wordWrap', value: false },
			],
		},
	},
};
