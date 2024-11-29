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
				{
					alias: 'theme',
					label: 'Theme',
					description: "Set the theme for the code editor. The default theme is 'Chrome'.",
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Label',
				},
				{
					alias: 'fontSize',
					label: 'Font size',
					description:
						'Set the font size. The value must be a valid CSS <a href="https://developer.mozilla.org/en-US/docs/Web/CSS/font-size" target="_blank"  rel="noopener"><strong>font-size</strong></a> value. The default size is \'small\'.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Label',
				},
				{
					alias: 'useWrapMode',
					label: 'Word wrapping',
					description: 'Select to enable word wrapping.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'minLines',
					label: 'Minimum lines',
					description: 'Set the minimum number of lines that the editor will be. The default is 12 lines.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'maxLines',
					label: 'Maximum lines',
					description:
						'Set the maximum number of lines that the editor can be. If left empty, the editor will not auto-scale.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
			],
			defaultData: [
				{ alias: 'mode', value: 'razor' },
				{ alias: 'theme', value: 'chrome' },
				{ alias: 'fontSize', value: 'small' },
				{ alias: 'minLines', value: 12 },
				{ alias: 'maxLines', value: 30 },
			],
		},
	},
};
