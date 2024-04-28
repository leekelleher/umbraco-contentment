// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyEditorUi = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CodeEditor',
	name: '[Contentment] Code Editor',
	element: () => import('./code-editor.element.js'),
	meta: {
		label: '[Contentment] Code Editor',
		icon: 'icon-code',
		group: 'code',
		propertyEditorSchemaAlias: 'Umbraco.Plain.String',
		settings: {
			properties: [
				{
					alias: 'mode',
					label: 'Language mode',
					description: 'WIP: Use one of the following: `razor`, `javascript`.',
					propertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.DropdownList',
					config: [
						{
							alias: 'items',
							value: [
								{ name: 'JavaScript', value: 'javascript' },
								{ name: 'Razor (CSHTML)', value: 'razor' },
							],
						},
					],
				},
			],
			defaultData: [{ alias: 'mode', value: 'razor' }],
		},
	},
};
