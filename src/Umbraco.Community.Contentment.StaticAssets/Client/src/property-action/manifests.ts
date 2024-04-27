// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as editJson } from './edit-json/manifest.js';
import { ManifestPropertyActions } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestPropertyActions> = [editJson];
