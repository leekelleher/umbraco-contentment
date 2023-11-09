/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property, query, state } from "@umbraco-cms/backoffice/external/lit";
import { loadCodeEditor, type UmbCodeEditorElement } from "@umbraco-cms/backoffice/code-editor";
import { UmbInputEvent } from "@umbraco-cms/backoffice/events";
import { UmbBooleanState } from "@umbraco-cms/backoffice/observable-api";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorExtensionElement } from "@umbraco-cms/backoffice/extension-registry";


@customElement("contentment-property-editor-ui-code-editor")
export class ContentmentPropertyEditorUICodeEditorElement
    extends LitElement
    implements UmbPropertyEditorExtensionElement {

    #isCodeEditorReady = new UmbBooleanState(false);
    isCodeEditorReady = this.#isCodeEditorReady.asObservable();

    @query("umb-code-editor")
    _codeEditor?: UmbCodeEditorElement;

    constructor() {
        super();
        this.#loadCodeEditor();
    }

    async #loadCodeEditor() {
        try {
            await loadCodeEditor();
            this.#isCodeEditorReady.next(true);
        } catch (error) {
            console.error(error);
        }
    }

    @state()
    private _language?: string;

    @property()
    public value: string | undefined;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this._language = config.getValueByAlias("mode") ?? "razor";
    }

    #onChange(e: UmbInputEvent) {
        e.stopPropagation();
        this.value = this._codeEditor?.code;
        this.dispatchEvent(new CustomEvent("property-value-change"));
    }

    #renderCodeEditor() {
        return html`
<div id="code-editor">
    <umb-code-editor language="${this._language}"
                     .code="${this.value ?? ''}"
                     @input="${this.#onChange}">
    </umb-code-editor>
</div>
`;
    }

    #renderLoading() {
        return html`<div id="loader"><uui-loader></uui-loader></div>`;
    }

    render() {
        return this.isCodeEditorReady ? this.#renderCodeEditor() : this.#renderLoading();
    }

    static styles = [
        css`
#code-editor {
    display: flex;
    height: 200px;
}
    #code-editor > umb-code-editor {
        width: 100%;
    }
`
    ];
}

export default ContentmentPropertyEditorUICodeEditorElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-code-editor": ContentmentPropertyEditorUICodeEditorElement;
    }
}
