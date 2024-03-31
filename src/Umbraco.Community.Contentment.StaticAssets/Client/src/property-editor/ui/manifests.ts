/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { manifest as codeEditor } from './code-editor/manifest.js';
import { manifest as configurationEditor } from './configuration-editor/manifest.js';
import { manifest as dataList } from './data-list/manifest.js';
import { manifest as editorNotes } from './editor-notes/manifest.js';
import { manifest as notes } from './notes/manifest.js';
import type { ManifestPropertyEditorUi } from '@umbraco-cms/backoffice/extension-registry';

export const manifests: Array<ManifestPropertyEditorUi> = [
  codeEditor,
  configurationEditor,
  dataList,
  editorNotes,
  notes
];
