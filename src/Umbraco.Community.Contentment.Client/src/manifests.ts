// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { manifests as conditions } from './condition/manifests.js';
import { manifests as extensions } from './extensions/manifests.js';
import { manifests as icons } from './icons/manifests.js';
import { manifests as localizations } from './localization/manifests.js';
import { manifests as propertyActions } from './property-action/manifests.js';
import { manifests as propertyEditorUis } from './property-editor-ui/manifests.js';
import { manifests as workspaces } from './workspace/manifests.js';
import type { UmbExtensionManifestKind } from '@umbraco-cms/backoffice/extension-registry';

import './components/index.js';

export const manifests: Array<UmbExtensionManifest | UmbExtensionManifestKind> = [
	...conditions,
	...extensions,
	...icons,
	...localizations,
	...propertyActions,
	...propertyEditorUis,
	...workspaces,
];
