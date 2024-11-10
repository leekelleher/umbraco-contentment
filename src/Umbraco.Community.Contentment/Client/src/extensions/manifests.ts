// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ManifestElement } from '@umbraco-cms/backoffice/extension-api';
import { manifests as dataListItems } from './data-list-item-ui/manifests.js';
import { manifests as templatedLabels } from './templated-label-ui/manifests.js';

export const manifests: Array<ManifestElement> = [...templatedLabels];
