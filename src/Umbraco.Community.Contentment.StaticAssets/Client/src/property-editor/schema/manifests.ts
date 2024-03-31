/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { manifest as dataList } from './Umbraco.Community.Contentment.DataList.js';
import { manifest as notes } from './Umbraco.Community.Contentment.Notes.js';
import type { ManifestPropertyEditorSchema } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestPropertyEditorSchema> = [
  dataList,
  notes,
];
