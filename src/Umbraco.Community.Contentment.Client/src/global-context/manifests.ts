// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'globalContext',
		alias: 'Contentment.GlobalContext.Liquid',
		name: '[Contentment] Liquid Context',
		api: () => import('./liquid.context.js'),
	},
];
