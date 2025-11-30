// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { manifests as blockEditorCustomViews } from './block-editor-custom-view/manifests.js';
import { manifests as dataListItemUis } from './data-list-item-ui/manifests.js';
import { manifests as dataSources } from './data-source/manifests.js';
import { manifests as displayModes } from './display-mode/manifests.js';
import { manifests as listEditors } from './list-editor/manifests.js';
import type { UmbExtensionManifestKind } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<UmbExtensionManifest | UmbExtensionManifestKind> = [
	...blockEditorCustomViews,
	...dataListItemUis,
	...dataSources,
	...displayModes,
	...listEditors,
];
