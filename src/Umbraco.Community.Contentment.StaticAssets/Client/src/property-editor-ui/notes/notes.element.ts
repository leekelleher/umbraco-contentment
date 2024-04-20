// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, property, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { tryHideLabel } from '../../utils/hide-label.function.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

@customElement('contentment-property-editor-ui-notes')
export class ContentmentPropertyEditorUINotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {

  #hideLabel: boolean = false;

  #notes?: string;

  @property()
  public value?: string;

  @property({ attribute: false })
  public set config(config: UmbPropertyEditorConfigCollection) {
    this.#notes = config.getValueByAlias('notes');
    this.#hideLabel = config.getValueByAlias<boolean>('hideLabel') ?? false;
  }

  connectedCallback() {
    super.connectedCallback();
    tryHideLabel(this, this.#hideLabel);
  }

  render() {
    return unsafeHTML(this.#notes);
  }

  static styles = [UmbTextStyles];
}

export default ContentmentPropertyEditorUINotesElement;

declare global {
  interface HTMLElementTagNameMap {
    'contentment-property-editor-ui-notes': ContentmentPropertyEditorUINotesElement;
  }
}
