// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Text Input Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.TextInput',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.TextInput',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.TextInput',
	name: '[Contentment] Text Input Property Editor UI',
	element: () => import('./text-input.element.js'),
	meta: {
		label: '[Contentment] Text Input',
		icon: 'icon-autofill',
		group: 'common',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.TextInput',
		settings: {
			properties: [
				{
					alias: 'inputType',
					label: 'Input type',
					description: 'Select the text-based HTML input type.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Email', value: 'email' },
								{ name: 'Telephone', value: 'tel' },
								{ name: 'Text', value: 'text' },
								{ name: 'URL', value: 'url' },
							],
						},
					],
				},
				{
					alias: 'placeholderText',
					label: 'Placeholder text',
					description:
						'Add placeholder text for the text input.<br>This is to be used as instructional information, not as a default value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
				},
				{
					alias: 'dataSource',
					label: 'Data source',
					description:
						'_(optional)_ Select and configure a data source to provide a HTML5 `<datalist>` for this text input.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ConfigurationEditor',
					config: [
						{ alias: 'addButtonLabelKey', value: 'contentment_configureDataSource' },
						{ alias: 'configurationType', value: 'dataSource' },
						{ alias: 'maxItems', value: 1 },
					],
				},
				{
					alias: 'maxChars',
					label: 'Maximum allowed characters',
					description:
						'Enter the maximum number of characters allowed for the text input.<br>The default limit is 500 characters.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
				},
				{
					alias: 'autocomplete',
					label: 'Enable autocomplete?',
					description: "Select to enable your web-browser's autocomplete functionality on the text input.",
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'spellcheck',
					label: 'Enable spellcheck?',
					description: "Select to enable your web-browser's spellcheck functionality on the text input.",
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'prepend',
					label: 'Prepend icon',
					description: '<em>(optional)</em> Select an icon to prepend to (before) the text input.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
				},
				{
					alias: 'append',
					label: 'Append icon',
					description: '<em>(optional)</em> Select an icon to append to (after) the text input.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
				},
			],
			defaultData: [{ alias: 'inputType', value: 'text' }],
		},
	},
};

export const manifests = [schema, editorUi];
