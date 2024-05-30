// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestIcons } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestIcons> = [
	{
		type: 'icons',
		alias: 'Umb.Contentment.Icons',
		name: '[Contentment] Icons',
		js: () => import('./icons.js'),
	},
];
