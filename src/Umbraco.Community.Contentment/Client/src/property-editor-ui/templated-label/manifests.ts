// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type {
	ManifestPropertyEditorSchema,
	ManifestPropertyEditorUi,
} from '@umbraco-cms/backoffice/extension-registry';

const schema: ManifestPropertyEditorSchema = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Templated Label Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.TemplatedLabel',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
		settings: {
			properties: [
				{
					alias: 'umbracoDataValueType',
					label: 'Value type',
					description: "Select the value's type. This defines how the underlying value is stored in the database.",
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'Bigint', value: 'BIGINT' },
								{ name: 'Date', value: 'DATE' },
								{ name: 'DateTime', value: 'DATETIME' },
								{ name: 'Decimal', value: 'DECIMAL' },
								{ name: 'Integer', value: 'INT' },
								{ name: 'Json', value: 'JSON' },
								{ name: 'String', value: 'STRING' },
								{ name: 'Text', value: 'TEXT' },
								{ name: 'Time', value: 'TIME' },
								{ name: 'Xml', value: 'XML' },
							],
						},
					],
				},
			],
			defaultData: [{ alias: 'umbracoDataValueType', value: 'STRING' }],
		},
	},
};

const editorUi: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.TemplatedLabel',
	name: '[Contentment] Templated Label Property Editor UI',
	element: () => import('./templated-label.element.js'),
	meta: {
		label: '[Contentment] Templated Label',
		icon: 'icon-fa-codepen',
		group: 'lists',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.TemplatedLabel',
		settings: {
			properties: [
				{
					alias: 'component',
					label: 'Component',
					description: 'Select the templated component to used for the label.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.ManifestPicker',
					config: [
						{ alias: 'extensionType', value: 'contentmentTemplatedLabel' },
						{ alias: 'maxItems', value: 1 },
					],
				},
				{
					alias: 'hideLabel',
					label: 'Hide label?',
					description: 'Select to hide the label and have the editor take up the full width of the panel.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
				{
					alias: 'enableDevMode',
					label: 'Developer mode?',
					description: 'Enable a property action to edit the raw data for the editor value.',
					propertyEditorUiAlias: 'Umb.PropertyEditorUi.Toggle',
				},
			],
		},
	},
};

export const manifests = [schema, editorUi];
