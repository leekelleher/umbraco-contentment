// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Icon Picker Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.IconPicker',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
	name: '[Contentment] Icon Picker Property Editor UI',
	element: () => import('./icon-picker.element.js'),
	meta: {
		label: 'Icon Picker',
		icon: 'icon-palette',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.IconPicker',
		settings: {
			properties: [
				{
					alias: 'defaultIcon',
					label: 'Default Icon',
					description: 'Select an icon to be displayed as the default icon, (for when no icon has been selected).',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.IconPicker',
				},
				{
					alias: 'size',
					label: 'Size',
					description: 'Select the size of icon picker. The default is "large".',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Small', value: 'small' },
								{ name: 'Large', value: 'large' },
							],
						},
						{ alias: 'defaultValue', value: 'large' },
					],
				},
			],
			defaultData: [{ alias: 'size', value: 'large' }],
		},
	},
};

export const manifests = [schema, editorUi];
