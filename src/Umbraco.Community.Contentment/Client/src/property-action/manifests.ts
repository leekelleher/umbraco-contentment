// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as allowClear } from './allow-clear/manifest.js';
import { manifest as editJson } from './edit-json/manifest.js';

export const manifests: Array<UmbExtensionManifest> = [allowClear, editJson];
