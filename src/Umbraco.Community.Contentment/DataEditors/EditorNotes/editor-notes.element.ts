/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property, when } from "@umbraco-cms/backoffice/external/lit";
import { unsafeHTML } from "lit/directives/unsafe-html.js";
import { UmbTextStyles } from "@umbraco-cms/backoffice/style";
import { UmbElementMixin } from "@umbraco-cms/backoffice/element-api";
import type { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import type { UmbPropertyEditorUiElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-property-editor-ui-editor-notes")
export class ContentmentPropertyEditorUIEditorNotesElement
    extends UmbElementMixin(LitElement)
    implements UmbPropertyEditorUiElement {

    #alertType?: string;

    #icon?: string;

    #heading?: string;

    #message?: string;

    @property()
    public value?: string;

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this.#alertType = config.getValueByAlias("alertType");
        this.#icon = config.getValueByAlias("icon");
        this.#heading = config.getValueByAlias("heading");
        this.#message = config.getValueByAlias("message");

        if (this.#icon) {
            // HACK: To workaround the `color-text` part of the value. [LK]
            this.#icon = this.#icon.split(' ')[0];
        }
    }

    render() {
        return html`
            <div id="note" class="uui-text ${this.#alertType}">
                ${when(this.#icon, () => html`<uui-icon name=${this.#icon}></uui-icon>`)}
                <div>
                    ${when(this.#heading, () => html`<h5>${this.#heading}</h5>`)}
                    ${when(this.#message, () => html`<div>${unsafeHTML(this.#message)}</div>`)}
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
