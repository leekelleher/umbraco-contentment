/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import type { ManifestPropertyEditorUi } from "@umbraco-cms/backoffice/extension-registry";

export const manifest: ManifestPropertyEditorUi = {
    type: "propertyEditorUi",
    alias: "Umbraco.Community.Contentment.DataList.UI",
    name: "[Contentment] Data List UI",
    element: () => import("./data-list.element.js"),
    meta: {
        label: "[Contentment] Data List",
        icon: "icon-list",
        group: "lists",
        propertyEditorSchemaAlias: "Umbraco.Community.Contentment.DataList",
        settings: {
            properties: [
                {
                    alias: "dataSource",
                    label: "Data source",
                    description: "Select and configure a data source.",
                    propertyEditorUiAlias: "Umbraco.Community.Contentment.ConfigurationEditor.UI"
                },
                {
                    alias: "listEditor",
                    label: "List editor",
                    description: "Select and configure a list editor.",
                    propertyEditorUiAlias: "Umbraco.Community.Contentment.ConfigurationEditor.UI"
                },
            ],
        }
    },
};
