/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const editorNotesUiManifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Umbraco.Community.Contentment.EditorNotes.UI",
    name: "[Contentment] Editor Notes UI",
    loader: () => import("./editor-notes.element.js"),
    meta: {
        label: "[Contentment] Editor Notes",
        icon: "umb:alert-alt",
        group: "display",
        propertyEditorSchemaAlias: "Umbraco.Community.Contentment.EditorNotes",
        settings: {
            properties: [
                {
                    alias: "alertType",
                    label: "Alert type",
                    description: "WIP: Use one of the following: `default`, `positive`, `warning`, `danger`.",
                    propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
                },
                {
                    alias: "icon",
                    label: "Icon",
                    description: "Select an icon to be displayed next to the message.",
                    propertyEditorUiAlias: "Umb.PropertyEditorUi.IconPicker"
                },
                {
                    alias: "heading",
                    label: "Heading",
                    propertyEditorUiAlias: "Umb.PropertyEditorUi.TextBox"
                },
                {
                    alias: "message",
                    label: "Message",
                    description: "Enter the notes to be displayed for the content editor.",
                    propertyEditorUiAlias: "Umb.PropertyEditorUi.TinyMCE"
                },
            ],
            defaultData: [{
                alias: "alertType", value: "alert alert-warning"
            }]
        }
    },
};
