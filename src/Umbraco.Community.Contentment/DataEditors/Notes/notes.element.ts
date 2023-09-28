import { LitElement, html } from "lit";
import { customElement, property, state } from "lit/decorators.js";
import { unsafeHTML } from "lit/directives/unsafe-html.js";
import { type UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";
import { type UmbPropertyEditorExtensionElement } from "@umbraco-cms/backoffice/extension-registry";

@customElement("contentment-notes")
export class ContentmentNotesPropertyEditorUIElement
    extends LitElement
    implements UmbPropertyEditorExtensionElement {

    @state()
    private _notes?: string;

    @property({ type: String })
    public value = "";

    @property({ attribute: false })
    public set config(config: UmbPropertyEditorConfigCollection) {
        this._notes = config.getValueByAlias("notes");
    }

    render() {
        return html`${unsafeHTML(this._notes)}`;
    }
}

declare global {
    interface HTMLElementTagNameMap {
        "contentment-notes": ContentmentNotesPropertyEditorUIElement;
    }
}
