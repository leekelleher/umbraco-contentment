// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifest as dataList } from './Umbraco.Community.Contentment.DataList.js';
import { manifest as notes } from './Umbraco.Community.Contentment.Notes.js';
import type { ManifestPropertyEditorSchema } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestPropertyEditorSchema> = [
  dataList,
  notes,
];
