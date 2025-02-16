// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'contentmentDisplayMode',
	alias: 'Umb.Contentment.DisplayMode.Cards',
	name: '[Contentment] Cards Display Mode UI',
	element: () => import('./cards.element.js'),
	meta: {
		key: 'Umb.Contentment.DisplayMode.Cards',
		name: 'Cards',
		icon: 'icon-playing-cards',
		description: 'Items will be displayed as cards.',
	},
};
