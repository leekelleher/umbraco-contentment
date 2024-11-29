// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'localization',
		name: '[Contentment] English',
		alias: 'Umb.Contentment.Localization.En',
		js: () => import('./en.js'),
		meta: {
			culture: 'en',
		},
	},
];
