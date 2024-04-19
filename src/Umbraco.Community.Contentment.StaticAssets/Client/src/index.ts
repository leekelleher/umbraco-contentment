// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as localizations } from "./localization/manifests.js";
import { manifests as propertyEditors } from './property-editor/manifests.js';
import type { ManifestTypes, UmbBackofficeManifestKind } from '@umbraco-cms/backoffice/extension-registry';

export * from './components/index.js';
export * from './utils/index.js';

export const manifests: Array<ManifestTypes | UmbBackofficeManifestKind> = [
  ...localizations,
  ...propertyEditors,
];


