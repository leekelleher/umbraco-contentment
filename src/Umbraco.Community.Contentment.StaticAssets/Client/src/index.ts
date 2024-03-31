/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { manifests as localizations } from "./localization/manifests.js";
import { manifests as propertyEditors } from './property-editor/manifests.js';
import type { ManifestTypes, UmbBackofficeManifestKind } from '@umbraco-cms/backoffice/extension-registry';

export * from './components/index.js';
export * from './utils/index.js';

export const manifests: Array<ManifestTypes | UmbBackofficeManifestKind> = [
  ...localizations,
  ...propertyEditors,
];


