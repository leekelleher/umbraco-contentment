// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ManifestWorkspace } from '@umbraco-cms/backoffice/extension-registry';

const workspace: ManifestWorkspace = {
	type: 'workspace',
	alias: 'Umb.Contentment.Workspace.Contentment',
	name: '[Contentment] Workspace',
	element: () => import('./workspace.element.js'),
	meta: {
		entityType: 'contentment',
	},
};

export const manifests = [workspace];
