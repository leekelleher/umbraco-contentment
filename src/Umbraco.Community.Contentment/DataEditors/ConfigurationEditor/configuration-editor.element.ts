/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UMB_PROPERTY_CONTEXT } from "@umbraco-cms/backoffice/property";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-configuration-editor")
export class ContentmentPropertyEditorUIConfigurationEditorElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    @property()
    public value?: object;

    #addButtonLabelKey: string = "general_add";

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {

        this.#addButtonLabelKey = config.getValueByAlias("addButtonLabelKey") ?? "general_add";

    }

    #propertyAlias?: string;

    constructor() {
        super();

        this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
            this.observe(propertyContext.alias, (alias) => {
                this.#propertyAlias = alias;
            });
        });
    }

    #openConfigurationEditorModal() {
        alert(`Open configuration editor modal for ${this.#propertyAlias}`);
    }

    render() {
        return html`
            <pre><code>${JSON.stringify(this.value, null, 4)}</code></pre>
            ${this.#renderChooseButton()}
        `;
    }

    #renderChooseButton() {
        return html`
            <uui-button
                @click=${this.#openConfigurationEditorModal}
                label=${this.localize.term(this.#addButtonLabelKey)}
                look="placeholder"></uui-button>
        `;
    }

    static styles = [
        css`
            uui-button {
                width: 100%;
            }
        `
    ];
}

export default ContentmentPropertyEditorUIConfigurationEditorElement;

declare global {
    interface HTMLElementTagNameMap {
        "contentment-property-editor-ui-configuration-editor": ContentmentPropertyEditorUIConfigurationEditorElement;
    }
}
