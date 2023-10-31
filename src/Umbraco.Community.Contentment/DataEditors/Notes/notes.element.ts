/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, customElement, property, state } from "@umbraco-cms/backoffice/external/lit";
import { unsafeHTML } from "lit/directives/unsafe-html.js";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorExtensionElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-notes")
export class ContentmentPropertyEditorUINotesElement
    extends LitElement
    implements UmbPropertyEditorExtensionElement {

    @state()
    private _notes?: string;

    @property()
    public value: string | undefined;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this._notes = config.getValueByAlias("notes");
    }

    render() {
        return unsafeHTML(this._notes);
    }
}

export default ContentmentPropertyEditorUINotesElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-notes": ContentmentPropertyEditorUINotesElement;
    }
}
