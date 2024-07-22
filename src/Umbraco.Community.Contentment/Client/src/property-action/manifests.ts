// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as editJson } from './edit-json/manifests.js';
import { ManifestTypes } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestTypes> = [...editJson];
