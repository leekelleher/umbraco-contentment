/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestTypes, UmbBackofficeManifestKind } from "@umbraco-cms/backoffice/extension-registry";
import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";

//import { manifests as propertyActionManifests } from "./DataEditors/_/property-action.manifests.js";
import { manifests as propertyEditorManifests } from "./DataEditors/property-editor-manifests.js";
import { manifests as localizationManifests } from "./Localizations/manifests.js";

export * from "./DataEditors/_/hide-label.function.js";

const manifests: Array<ManifestTypes | UmbBackofficeManifestKind> = [
    //...propertyActionManifests,
    ...propertyEditorManifests,
    ...localizationManifests,
];

export const onInit: UmbEntryPointOnInit = (_, extensionRegistry) => {
    extensionRegistry.registerMany(manifests);
};
