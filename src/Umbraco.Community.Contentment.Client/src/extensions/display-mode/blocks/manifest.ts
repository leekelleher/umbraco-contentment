// Umbraco.Community.Contentment.DataEditors.BlocksDisplayMode, Umbraco.Community.Contentment

// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'contentmentDisplayMode',
	alias: 'Umb.Contentment.DisplayMode.Blocks',
	name: '[Contentment] Blocks Display Mode UI',
	element: () => import('./blocks.element.js'),
	meta: {
		key: 'Umb.Contentment.DisplayMode.Blocks',
		name: 'Blocks',
		icon: 'icon-thumbnail-list',
		description: 'Blocks will be displayed in a list similar to the Block List editor.',
	},
};
