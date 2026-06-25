// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as backofficeColors } from './backoffice-colors/manifest.js';
import { manifest as backofficeSections } from './backoffice-sections/manifest.js';

export const manifests: Array<UmbExtensionManifest> = [backofficeColors, backofficeSections];
