// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CheckBoxList',
	name: '[Contentment] Checkbox List Property Editor UI',
	element: () => import('./checkbox-list.element.js'),
	meta: {
		label: '[Contentment] Checkbox List',
		icon: 'icon-checkbox',
		group: 'lists',
	},
};
