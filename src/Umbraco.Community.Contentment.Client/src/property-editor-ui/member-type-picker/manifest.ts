// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.MemberTypePicker',
	name: '[Contentment] Member Type Picker Property Editor UI',
	element: () => import('./member-type-picker.element.js'),
	meta: {
		label: 'Member Type Picker',
		icon: 'icon-checkbox',
		group: 'contentment',
	},
};
