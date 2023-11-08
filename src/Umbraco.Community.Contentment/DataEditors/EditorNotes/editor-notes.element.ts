/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property, state, when } from "@umbraco-cms/backoffice/external/lit";
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
        this._alertType = config.getValueByAlias("alertType");
        this._icon = config.getValueByAlias("icon");
        this._heading = config.getValueByAlias("heading");
        this._message = config.getValueByAlias("message");
    }

    render() {
        return html`
<div id="note" class="uui-text ${this._alertType}">
    ${when(this._icon, () => html`<uui-icon name=${this._icon}></uui-icon>`)}
    <div>
        ${when(this._heading, () => html`<h5>${this._heading}</h5>`)}
        ${when(this._message, () => html`<div>${unsafeHTML(this._message)}</div>`)}
    </div>
</div>
`;
    }

    static styles = [
        UmbTextStyles,
        css`
#note {
    display: flex;
    align-items: flex-start;
    justify-content: flex-start;
    gap: 1rem;

    background-color: var(--uui-color-surface);
    color: var(--uui-color-text);

    border-color: var(--uui-color-surface);
    border-radius: calc(var(--uui-border-radius) * 2);

    box-shadow: var(--uui-shadow-depth-1);

    padding: 1rem;
}
    #note.default {
        background-color: var(--uui-color-default);
        color: var(--uui-color-default-contrast);
        border-color: var(--uui-color-default-standalone);
    }
        #note.positive uui-icon {
            color: var(--uui-color-default-contrast);
        }
    #note.positive {
        background-color: var(--uui-color-positive);
        color: var(--uui-color-positive-contrast);
        border-color: var(--uui-color-positive-standalone);
    }
        #note.positive uui-icon {
            color: var(--uui-color-positive-contrast);
        }
    #note.warning {
        background-color: var(--uui-color-warning);
        color: var(--uui-color-warning-contrast);
        border-color: var(--uui-color-warning-standalone);
    }
        #note.warning uui-icon {
            color: var(--uui-color-warning-contrast);
        }
    #note.danger {
        background-color: var(--uui-color-danger);
        color: var(--uui-color-danger-contrast);
        border-color: var(--uui-color-danger-standalone);
    }
        #note.danger uui-icon {
            color: var(--uui-color-danger-contrast);
        }

uui-icon {
    min-height: 3rem;
    min-width: 3rem;
}

.uui-text p {
    margin: 0.5rem 0;
}
    .uui-text p:last-child {
        margin-bottom: 0.25rem;
    }
`
    ];
}

export default ContentmentPropertyEditorUIEditorNotesElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-editor-notes": ContentmentPropertyEditorUIEditorNotesElement;
    }
}
