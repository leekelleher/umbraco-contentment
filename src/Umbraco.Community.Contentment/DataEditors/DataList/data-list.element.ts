/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";


@customElement("contentment-property-editor-ui-data-list")
export class ContentmentPropertyEditorUIDataListElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    @property()
    public value?: string;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        if (config) {

        }
    }

    render() {
        return html`&lt;contentment-property-editor-ui-data-list&gt;&lt;/contentment-property-editor-ui-data-list&gt;`;
    }

    static styles = [
        css``
    ];
}

export default ContentmentPropertyEditorUIDataListElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-data-list": ContentmentPropertyEditorUIDataListElement;
    }
}
