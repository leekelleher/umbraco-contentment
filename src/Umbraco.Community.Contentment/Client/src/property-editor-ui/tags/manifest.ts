// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.Tags',
	name: '[Contentment] Tags Property Editor UI',
	element: () => import('./tags.element.js'),
	meta: {
		label: 'Tags',
		icon: 'icon-fa-tags',
		group: 'contentment',
	},
};
