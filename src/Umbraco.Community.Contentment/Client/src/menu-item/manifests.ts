// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ManifestMenuItem } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestMenuItem> = [
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
