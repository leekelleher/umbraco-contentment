/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, customElement, property, unsafeHTML } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-notes")
export class ContentmentPropertyEditorUINotesElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    #notes?: string;

    @property()
    public value?: string;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this.#notes = config.getValueByAlias("notes");
    }

    render() {
        return unsafeHTML(this.#notes);
    }
}

export default ContentmentPropertyEditorUINotesElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-notes": ContentmentPropertyEditorUINotesElement;
    }
}
