// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifest: UmbExtensionManifest = {
	type: 'contentmentDisplayMode',
	alias: 'Umb.Contentment.DisplayMode.Cards',
	name: '[Contentment] Cards Display Mode UI',
	element: () => import('./cards.element.js'),
	meta: {
		key: 'Umbraco.Community.Contentment.DataEditors.CardsDataPickerDisplayMode, Umbraco.Community.Contentment',
		name: 'Cards',
		icon: 'icon-playing-cards',
	},
};
