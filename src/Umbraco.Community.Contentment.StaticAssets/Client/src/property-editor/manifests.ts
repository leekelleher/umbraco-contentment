/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { manifests as propertyEditorSchemaManifests } from './schema/manifests.js';
import { manifests as propertyEditorUIManifests } from './ui/manifests.js';

export const manifests = [...propertyEditorSchemaManifests, ...propertyEditorUIManifests];
