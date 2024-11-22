// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'menuItem',
		alias: 'Umb.Contentment.MenuItem.Contentment',
		name: '[Contentment] Menu Item',
		meta: {
			label: 'Contentment',
			icon: 'icon-contentment',
			entityType: 'contentment',
			menus: ['Umb.Menu.AdvancedSettings'],
		},
	},
];
