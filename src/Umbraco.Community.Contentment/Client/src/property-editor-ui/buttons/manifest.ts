// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Buttons',
	name: '[Contentment] Buttons Property Editor UI',
	element: () => import('./buttons.element.js'),
	meta: {
		label: '[Contentment] Buttons',
		icon: 'icon-tab',
		group: 'lists',
	},
};
