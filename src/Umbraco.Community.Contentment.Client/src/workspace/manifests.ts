// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

const workspace: UmbExtensionManifest = {
	type: 'workspace',
	alias: 'Umb.Contentment.Workspace.Contentment',
	name: '[Contentment] Workspace',
	element: () => import('./workspace.element.js'),
	meta: {
		entityType: 'contentment',
	},
};

export const manifests = [workspace];
