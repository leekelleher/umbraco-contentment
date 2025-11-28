// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.RadioButtonList',
	name: '[Contentment] Radio Button List Property Editor UI',
	element: () => import('./radio-button-list.element.js'),
	meta: {
		label: 'Radio Button List',
		icon: 'icon-target',
		group: 'contentment',
	},
};
