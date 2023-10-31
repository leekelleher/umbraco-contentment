/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, ifDefined, property, state } from "@umbraco-cms/backoffice/external/lit";
import { unsafeHTML } from "lit/directives/unsafe-html.js";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorExtensionElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-editor-notes")
export class ContentmentPropertyEditorUIEditorNotesElement
    extends LitElement
    implements UmbPropertyEditorExtensionElement {

    @state()
    private _alertType?: string;

    @state()
    private _icon?: string;

    @state()
    private _heading?: string;

    @state()
    private _message?: string;

    @property()
    public value: string | undefined;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this._alertType = "positive"; // config.getValueByAlias("alertType");
        this._icon = "umb:alert-alt"; // config.getValueByAlias("icon");
        this._heading = config.getValueByAlias("heading");
        this._message = config.getValueByAlias("message");
    }

    render() {
        return html`
<uui-toast-notification open="true" color="${this._alertType}">
    <uui-toast-notification-layout id="layout" headline="${ifDefined(this._heading)}" class="uui-text">
        <uui-icon name="${this._icon}"></uui-icon>
        <div id="message">${unsafeHTML(this._message)}</div>
    </uui-toast-notification-layout>
</uui-toast-notification>
`;
    }

    static styles = [
        UmbTextStyles,
        css``
    ];
}

export default ContentmentPropertyEditorUIEditorNotesElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-editor-notes": ContentmentPropertyEditorUIEditorNotesElement;
    }
}
