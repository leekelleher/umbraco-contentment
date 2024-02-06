/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property, state } from "@umbraco-cms/backoffice/external/lit";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { UMB_DOCUMENT_WORKSPACE_CONTEXT } from "@umbraco-cms/backoffice/document";
import { UMB_PROPERTY_CONTEXT } from "@umbraco-cms/backoffice/property";
import type { ManifestPropertyEditorUi, UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-data-list")
export class ContentmentPropertyEditorUIDataListElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    @property()
    public value?: string;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        if (config) {
            console.log("data-list.config", config);
        }
    }

    #dataTypeId?: string;

    #propertyAlias?: string;

    #propertyContext?: typeof UMB_PROPERTY_CONTEXT.TYPE;

    #workspaceContext?: typeof UMB_DOCUMENT_WORKSPACE_CONTEXT.TYPE;

    @state()
    private _items: Array<{ value: string, sortOrder: number }> = [];

    constructor() {
        super();

        this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
            this.#propertyContext = propertyContext;

            this.observe(propertyContext.alias, (propertyAlias) => {
                this.#propertyAlias = propertyAlias;
            });

            this.observe(propertyContext.value, (propertyValue) => {
                console.log("data-list.propertyContext.value", propertyValue);
            });
        });

        this.consumeContext(UMB_DOCUMENT_WORKSPACE_CONTEXT, (workspaceContext) => {
            this.#workspaceContext = workspaceContext;
        });
    }

    connectedCallback() {
        super.connectedCallback();
        console.log("data-list.connectedCallback");
        this.#init();
    }

    async #init() {

        // Gets the Data Type ID for the current property.
        if (this.#propertyAlias && this.#workspaceContext) {
            const property = await this.#workspaceContext.structure.getPropertyStructureByAlias(this.#propertyAlias);

            if (property) {
                this.#dataTypeId = property.dataTypeId; // property.dataType.unique;
                console.log("data-list.dataTypeId", this.#dataTypeId);

                // TODO: [LK] Fetch the Data List items.
                const items = [
                    { value: 'one', sortOrder: 1 },
                    { value: 'two', sortOrder: 2 },
                    { value: 'three', sortOrder: 3 },
                ];

                this._items = items.map((item) => (item.value === this.value ? { ...item, selected: true } : item));
            }
        }

    }

    render() {

        // Umb.PropertyEditorUi.CheckboxList
        // Umb.PropertyEditorUi.Dropdown
        // Umb.PropertyEditorUi.RadioButtonList
        const listEditorAlias = "Umb.PropertyEditorUi.RadioButtonList";

        const config = new UmbPropertyEditorConfigCollection(
            [{ alias: "items", value: this._items }]
        );

        return html`
            <umb-extension-slot
                type="propertyEditorUi"
                default-element="div"
                .filter=${(ext: ManifestPropertyEditorUi) => ext.alias === listEditorAlias}
                .props=${{ value: this.value, config }}>
            </umb-extension-slot>
        `;
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
