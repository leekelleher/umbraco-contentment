/* Copyright � 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestPropertyEditorSchema, ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const notesSchemaManifest: ManifestPropertyEditorSchema = {
    type: "propertyEditorSchema",
    name: "[Contentment] Notes",
    alias: "Umbraco.Community.Contentment.Notes",
    meta: {
        defaultPropertyEditorUiAlias: "Umbraco.Community.Contentment.Notes.UI",
        settings: {
            properties: [
                {
                    alias: "notes",
                    label: "Notes",
                    description: "Enter the notes to be displayed for the content editor.",
                    propertyEditorUiAlias: "Umb.PropertyEditorUi.TinyMCE"
                },
            ]
        }
    }
};

export const notesUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Umbraco.Community.Contentment.Notes.UI",
    name: "[Contentment] Notes UI",
    js: () => import("./notes.element.js"),
    meta: {
        label: "[Contentment] Notes",
        icon: "umb:autofill",
        group: "display",
        propertyEditorSchemaAlias: "Umbraco.Community.Contentment.Notes",
    },
};
