// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as propertyEditorSchemaManifests } from './schema/manifests.js';
import { manifests as propertyEditorUIManifests } from './ui/manifests.js';

export const manifests = [...propertyEditorSchemaManifests, ...propertyEditorUIManifests];
