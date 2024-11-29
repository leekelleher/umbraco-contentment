// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ReadOnly',
	name: '[Contentment] Read Only Property Editor UI',
	element: () => import('./read-only.element.js'),
	meta: {
		label: 'Read Only',
		icon: 'icon-hearts',
		group: 'contentment',
	},
};
