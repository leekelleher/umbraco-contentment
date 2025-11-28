// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'icons',
		alias: 'Umb.Contentment.Icons',
		name: '[Contentment] Icons',
		js: () => import('./icons.js'),
	},
];
