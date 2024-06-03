// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

export const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Bytes Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.Bytes',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.Bytes',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Bytes',
	name: '[Contentment] Bytes Property Editor UI',
	element: () => import('./bytes.element.js'),
	meta: {
		label: '[Contentment] Bytes',
		icon: 'icon-binarycode',
		group: 'display',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.Bytes',
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

export const manifests = [schema, editorUi];
