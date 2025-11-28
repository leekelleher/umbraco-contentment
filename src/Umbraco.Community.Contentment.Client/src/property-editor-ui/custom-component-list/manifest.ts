// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.CustomComponentList',
	name: '[Contentment] Custom Component List Property Editor UI',
	element: () => import('./custom-component-list.element.js'),
	meta: {
		label: 'Custom Component List',
		icon: 'icon-shipping-box',
		group: 'contentment',
	},
};
