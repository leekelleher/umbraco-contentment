// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

const schema: UmbExtensionManifest = {
	type: 'propertyEditorSchema',
	name: '[Contentment] Input List Property Editor Schema',
	alias: 'Umbraco.Community.Contentment.InputList',
	meta: {
		defaultPropertyEditorUiAlias: 'Umb.Contentment.PropertyEditorUi.InputList',
		settings: {
			properties: [],
		},
	},
};

const editorUi: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.InputList',
	name: '[Contentment] Input List Property Editor UI',
	element: () => import('./input-list.element.js'),
	meta: {
		label: 'Input List',
		icon: 'icon-list',
		group: 'contentment',
		propertyEditorSchemaAlias: 'Umbraco.Community.Contentment.InputList',
		settings: {
			properties: [],
		},
	},
};

export const manifests = [schema, editorUi];
