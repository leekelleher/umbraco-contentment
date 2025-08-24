// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as codeBlock } from './code-block/manifest.js';
import { manifest as infoBox } from './info-box/manifest.js';

export const manifests: Array<UmbExtensionManifest> = [codeBlock, infoBox];
