// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

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
