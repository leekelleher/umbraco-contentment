// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as blocks } from './blocks/manifest.js';
import { manifest as cards } from './cards/manifest.js';
import { manifest as list } from './list/manifest.js';

export const manifests: Array<UmbExtensionManifest> = [blocks, cards, list];
