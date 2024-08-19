// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as conditions } from './condition/manifests.js';
import { manifests as dashboards } from './dashboard/manifests.js';
import { manifests as icons } from './icons/manifests.js';
import { manifests as localizations } from './localization/manifests.js';
import { manifests as propertyActions } from './property-action/manifests.js';
import { manifests as propertyEditorUis } from './property-editor-ui/manifests.js';
import type { ManifestTypes, UmbBackofficeManifestKind } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestTypes | UmbBackofficeManifestKind> = [
	...conditions,
	...dashboards,
	...icons,
	...localizations,
	...propertyActions,
	...propertyEditorUis,
];
