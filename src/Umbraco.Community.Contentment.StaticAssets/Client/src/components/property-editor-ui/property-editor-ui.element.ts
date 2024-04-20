// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

// TODO: [LK] Update the license to credit Umbraco core.
// https://github.com/umbraco/Umbraco.CMS.Backoffice/blob/v14.0.0-beta002/src/packages/core/property/property/property.element.ts

import { createExtensionElement } from "@umbraco-cms/backoffice/extension-api";
import {
  customElement,
  nothing,
  property,
  state,
} from "@umbraco-cms/backoffice/external/lit";
import {
  ManifestPropertyEditorUi,
  umbExtensionsRegistry,
} from "@umbraco-cms/backoffice/extension-registry";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";
import { UmbPropertyEditorConfigCollection } from "@umbraco-cms/backoffice/property-editor";

@customElement("contentment-property-editor-ui")
export class ContentmentPropertyEditorUiElement extends UmbLitElement {
  @property({ attribute: false })
  config?: UmbPropertyEditorConfigCollection | undefined;

  @property({ type: String, attribute: "property-editor-ui-alias" })
  public set propertyEditorUiAlias(value: string | undefined) {
    this.#propertyEditorUiAlias = value;
    this.#observePropertyEditorUI();
  }
  public get propertyEditorUiAlias(): string | undefined {
    return this.#propertyEditorUiAlias;
  }
  #propertyEditorUiAlias?: string;

  @property()
  value?: any;

  @state()
  private _element?: ManifestPropertyEditorUi["ELEMENT_TYPE"];

  #observePropertyEditorUI(): void {
    if (this.#propertyEditorUiAlias) {
      this.observe(
        umbExtensionsRegistry.byTypeAndAlias(
          "propertyEditorUi",
          this.#propertyEditorUiAlias
        ),
        (manifest) => this.#gotEditorUI(manifest),
        "observePropertyEditorUI"
      );
    }
  }

  async #gotEditorUI(
    manifest?: ManifestPropertyEditorUi | null
  ): Promise<void> {
    if (!manifest) return;

    const el = await createExtensionElement(manifest);

    if (el) {
      const oldElement = this._element;

      this._element = el as ManifestPropertyEditorUi["ELEMENT_TYPE"];
      if (this._element) {
        this._element.value = this.value;

        if (this.config) {
          this._element.config = this.config;
        }
      }

      this.requestUpdate("_element", oldElement);
    }
  }

  // Disable the Shadow DOM for this element. The event propagation needs to pass-through.
  createRenderRoot() {
    return this;
  }

  render() {
    return this._element ?? nothing;
  }
}

export default ContentmentPropertyEditorUiElement;

declare global {
  interface HTMLElementTagNameMap {
    "contentment-property-editor-ui": ContentmentPropertyEditorUiElement;
  }
}
