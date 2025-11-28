// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'propertyEditorUi',
	alias: 'Umb.Contentment.PropertyEditorUi.ContentSource',
	name: '[Contentment] Content Source Property Editor UI',
	element: () => import('./content-source.element.js'),
	meta: {
		label: 'Content Source',
		icon: 'icon-page-add',
		group: 'contentment',
	},
};
