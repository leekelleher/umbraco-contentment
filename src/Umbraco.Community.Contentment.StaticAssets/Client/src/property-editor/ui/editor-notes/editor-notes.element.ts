import { css, customElement, html, property, when, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { tryHideLabel } from '../../../utils/hide-label.function.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

@customElement('contentment-property-editor-ui-editor-notes')
export class ContentmentPropertyEditorUIEditorNotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {

// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

  #hideLabel: boolean = false;

  #alertType?: string;

  #icon?: string;

  #heading?: string;

  #message?: string;

  @property()
  public value?: string;

  @property({ attribute: false })
  public set config(config: UmbPropertyEditorConfigCollection) {
    this.#alertType = config.getValueByAlias('alertType');
    this.#icon = config.getValueByAlias('icon');
    this.#heading = config.getValueByAlias('heading');
    this.#message = config.getValueByAlias('message');
    this.#hideLabel = config.getValueByAlias<boolean>('hideLabel') ?? false;

    if (this.#icon) {
      // HACK: To workaround the `color-text` part of the value. [LK]
      this.#icon = this.#icon.split(' ')[0];
    }
  }

  connectedCallback() {
    super.connectedCallback();
    tryHideLabel(this, this.#hideLabel);
  }

  render() {
    const inlineStyles = `
        background-color: var(--uui-color-${this.#alertType});
        color: var(--uui-color-${this.#alertType}-contrast);
        border-color: var(--uui-color-${this.#alertType}-standalone);
    `;

    return html`
        <div id='note' class='uui-text ${this.#alertType}' style='${inlineStyles}'>
            ${when(this.#icon, () => html`<uui-icon name=${this.#icon} style='color: var(--uui-color-${this.#alertType}-contrast);'></uui-icon>`)}
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
    'contentment-property-editor-ui-editor-notes': ContentmentPropertyEditorUIEditorNotesElement;
  }
}
