// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const CONTENTMENT_BYTES_PROPERTY_EDITOR_SCHEMA_ALIAS = 'Umbraco.Community.Contentment.Bytes';

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Bytes Property Editor Schema',
	alias: CONTENTMENT_BYTES_PROPERTY_EDITOR_SCHEMA_ALIAS,
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Bytes',
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Bytes',
	name: '[Contentment] Bytes Property Editor UI',
	element: () => import('./bytes.element.js'),
	meta: {
		label: 'Bytes',
		icon: 'icon-binarycode',
		group: 'contentment',
		propertyEditorSchemaAlias: CONTENTMENT_BYTES_PROPERTY_EDITOR_SCHEMA_ALIAS,
		settings: {
			properties: [
				{
					alias: 'kilo',
					label: 'Kilobytes?',
					description: 'How many bytes do you prefer in your kilobyte?',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
					config: [
						{
							alias: 'items',
							value: [
								{
									name: '1000 bytes',
									value: '1000',
									description: 'The modern standard for a kilobyte is <strong>1000 bytes</strong> (decimal).',
								},
								{
									name: '1024 bytes',
									value: '1024',
									description:
										'Computationally, there are <strong>1024 bytes</strong> (binary). Today, this is known as a kibibyte.',
								},
							],
						},
						{ alias: 'showDescriptions', value: 'true' },
					],
				},
				{
					alias: 'decimals',
					label: 'Decimal places',
					description: 'How many decimal places would you like?',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Slider',
					config: [
						{ alias: 'initVal1', value: 2 },
						{ alias: 'minVal', value: 0 },
						{ alias: 'maxVal', value: 10 },
						{ alias: 'step', value: 1 },
					],
				},
			],
			defaultData: [
				{ alias: 'kilo', value: '1024' },
				{ alias: 'decimals', value: 2 },
			],
		},
	},
};

const valueSummary: UmbExtensionManifest = {
	type: 'valueSummary',
	kind: 'default',
	alias: 'Umb.Contentment.ValueSummary.PropertyEditor.Bytes',
	name: '[Contentment] Bytes Property Editor Value Summary',
	forValueType: CONTENTMENT_BYTES_PROPERTY_EDITOR_SCHEMA_ALIAS,
	element: () => import('./bytes.value-summary.element.js'),
};

export const manifests = [schema, editorUi, valueSummary];
