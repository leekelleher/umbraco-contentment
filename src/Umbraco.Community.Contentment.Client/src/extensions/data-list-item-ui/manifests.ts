// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as codeBlock } from './code-block/manifest.js';
import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export const manifests: Array<ManifestElement> = [codeBlock];
