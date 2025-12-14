// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'globalContext',
		alias: 'Umb.Contentment.GlobalContext.Liquid',
		name: '[Contentment] Liquid Global Context',
		api: () => import('./liquid.context.js'),
	},
];
