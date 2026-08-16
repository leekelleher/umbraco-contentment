// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const CONTENTMENT_ICON_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS = 'Umbraco.Community.Contentment.IconPicker';

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Icon Picker Property Editor Schema',
	alias: CONTENTMENT_ICON_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
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
		propertyEditorSchemaAlias: CONTENTMENT_ICON_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
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
				{
					alias: 'hideClearButton',
					label: 'Hide clear button?',
					description: `If you wish to hide the "clear" button, you can unset the icon picker via the modal selection.`,
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
			defaultData: [{ alias: 'size', value: 'large' }],
		},
	},
};

const valueSummary: UmbExtensionManifest = {
	type: 'valueSummary',
	kind: 'default',
	alias: 'Umb.Contentment.ValueSummary.PropertyEditor.IconPicker',
	name: '[Contentment] Icon Picker Property Editor Value Summary',
	forValueType: CONTENTMENT_ICON_PICKER_PROPERTY_EDITOR_SCHEMA_ALIAS,
	element: () => import('./icon-picker.value-summary.element.js'),
};

export const manifests = [schema, editorUi, valueSummary];
