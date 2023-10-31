﻿/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestTypes, UmbBackofficeManifestKind } from "@umbraco-cms/backoffice/extension-registry";
import type { UmbEntryPointOnInit } from "@umbraco-cms/backoffice/extension-api";

import { manifests as propertyEditorManifests } from "./DataEditors/manifests.js";

const manifests: Array<ManifestTypes | UmbBackofficeManifestKind> = [
    ...propertyEditorManifests,
];

export const onInit: UmbEntryPointOnInit = (_, extensionRegistry) => {

    extensionRegistry.registerMany(manifests);

    console.log(`%c
     (((
    (o o)
ooO--(_)--Ooo-    #FIWNBPFT

 ██████╗ ██████╗ ███╗   ██╗████████╗███████╗███╗   ██╗████████╗███╗   ███╗███████╗███╗   ██╗████████╗
██╔════╝██╔═══██╗████╗  ██║╚══██╔══╝██╔════╝████╗  ██║╚══██╔══╝████╗ ████║██╔════╝████╗  ██║╚══██╔══╝
██║     ██║   ██║██╔██╗ ██║   ██║   █████╗  ██╔██╗ ██║   ██║   ██╔████╔██║█████╗  ██╔██╗ ██║   ██║
██║     ██║   ██║██║╚██╗██║   ██║   ██╔══╝  ██║╚██╗██║   ██║   ██║╚██╔╝██║██╔══╝  ██║╚██╗██║   ██║
╚██████╗╚██████╔╝██║ ╚████║   ██║   ███████╗██║ ╚████║   ██║   ██║ ╚═╝ ██║███████╗██║ ╚████║   ██║
 ╚═════╝ ╚═════╝ ╚═╝  ╚═══╝   ╚═╝   ╚══════╝╚═╝  ╚═══╝   ╚═╝   ╚═╝     ╚═╝╚══════╝╚═╝  ╚═══╝   ╚═╝
                                                                                                     `, `font-family: monospace`);

};