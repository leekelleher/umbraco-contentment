// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as allowClear } from './allow-clear/manifest.js';
import { manifest as editJson } from './edit-json/manifest.js';
import { ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestTypes> = [allowClear, editJson];
