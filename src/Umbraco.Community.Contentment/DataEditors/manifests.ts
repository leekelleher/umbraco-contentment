/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { editorNotesUiManifest } from "./EditorNotes/editor-notes.manifest.js";
import { notesSchemaManifest, notesUiManifest } from "./Notes/notes.manifest.js";

export const manifests = [
    editorNotesUiManifest,
    notesSchemaManifest,
    notesUiManifest
];
