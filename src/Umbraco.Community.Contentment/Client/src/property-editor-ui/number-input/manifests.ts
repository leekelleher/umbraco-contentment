// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Number Input Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.NumberInput',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.NumberInput',
	name: '[Contentment] Number Input Property Editor UI',
	element: () => import('../number-input/number-input.element.js'),
	meta: {
		label: '[Contentment] Number Input',
		icon: 'icon-ordered-list',
		group: 'common',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.NumberInput',
		settings: {
			properties: [
				{
					alias: 'size',
					label: 'Numeric size',
					description: 'How big will the number get?',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Small', value: 's', description: 'Ideal for numbers under 100, comfortably fits 3 digits.' },
								{ name: 'Medium', value: 'm', description: 'Better when dealing with hundreds and thousands, comfortably fits 6 digits.' },
								{ name: 'Large', value: 'l', description: 'Did you know a 18 digit number is called a quintillion!' },
								{ name: 'Extra Large', value: 'xl', description: "Useful when aligning with full width text inputs. Fits 88 digits <em>- that's over an octovigintillion!</em>" },
							],
						},
						{ alias: 'showDescriptions', value: true },
					],
				},
				{
					alias: 'placeholderText',
					label: 'Placeholder text',
					description: 'Add placeholder text for the number input.<br>This is to be used as instructional information, not as a default value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.TextBox',
				},
			],
			defaultData: [{ alias: 'size', value: 's' }],
		},
	},
};

export const manifests = [schema, editorUi];
