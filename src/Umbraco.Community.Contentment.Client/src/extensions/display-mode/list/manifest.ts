// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'contentmentDisplayMode',
	alias: 'Umb.Contentment.DisplayMode.List',
	name: '[Contentment] List Display Mode UI',
	element: () => import('./list.element.js'),
	meta: {
		key: 'Umbraco.Community.Contentment.DataEditors.ListDataPickerDisplayMode, Umbraco.Community.Contentment',
		name: 'List',
		icon: 'icon-fa-list-ul',
		description: 'Items will be displayed in a list similar to a content picker.',
	},
};
