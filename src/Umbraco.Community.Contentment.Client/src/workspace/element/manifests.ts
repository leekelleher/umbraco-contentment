// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export const manifests: Array<UmbExtensionManifest> = [
	{
		type: 'modal',
		alias: 'Umb.Contentment.Modal.ElementWorkspace',
		name: '[Contentment] Element Workspace Modal',
		element: () => import('./element-workspace-modal.element.js'),
	},
];
