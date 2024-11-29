// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as dataListItems } from './data-list-item-ui/manifests.js';
import { manifests as templatedLabels } from './templated-label-ui/manifests.js';
import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export const manifests: Array<ManifestElement> = [...dataListItems, ...templatedLabels];
