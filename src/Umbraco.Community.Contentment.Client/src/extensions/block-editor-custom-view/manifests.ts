// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { ManifestBlockEditorCustomView } from '@umbraco-cms/backoffice/block-custom-view';
import type { ManifestKind } from '@umbraco-cms/backoffice/extension-api';
import type { UmbExtensionManifestKind } from '@umbraco-cms/backoffice/extension-registry';

const kind: ManifestKind<ManifestBlockEditorCustomView> = {
	type: 'kind',
	alias: 'Umb.Contentment.Kind.BlockEditorCustomView.Liquid',
	matchKind: 'liquid',
	matchType: 'blockEditorCustomView',
	manifest: {
		type: 'blockEditorCustomView',
		kind: 'liquid',
		element: () => import('./liquid.element.js'),
	},
};

export const manifests: Array<UmbExtensionManifest | UmbExtensionManifestKind> = [kind];
