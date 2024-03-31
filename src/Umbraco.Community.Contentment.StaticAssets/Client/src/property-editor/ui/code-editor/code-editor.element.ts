/* Copyright © 2023 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { LitElement, css, customElement, html, property, query } from '@umbraco-cms/backoffice/external/lit';
import { loadCodeEditor, type UmbCodeEditorElement } from '@umbraco-cms/backoffice/code-editor';
import { UmbBooleanState } from '@umbraco-cms/backoffice/observable-api';
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';
import { UmbPropertyValueChangeEvent, type UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbInputEvent } from '@umbraco-cms/backoffice/event';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

@customElement('contentment-property-editor-ui-code-editor')
export class ContentmentPropertyEditorUICodeEditorElement
  extends UmbElementMixin(LitElement)
  implements UmbPropertyEditorUiElement {

  #isCodeEditorReady = new UmbBooleanState(false);
  #loaded = this.#isCodeEditorReady.asObservable();

  @query('umb-code-editor')
  private _codeEditor?: UmbCodeEditorElement;

  constructor() {
    super();
    this.#loadCodeEditor();
  }

  async #loadCodeEditor() {
    try {
      await loadCodeEditor();
      this.#isCodeEditorReady.setValue(true);
    } catch (error) {
      console.error(error);
    }
  }

  #language?: string;

  @property()
  public value?: string;

  @property({ attribute: false })
  public set config(config: UmbPropertyEditorConfigCollection) {
    this.#language = config.getValueByAlias('mode') ?? 'razor';
  }

  #onChange(event: UmbInputEvent) {
    event.stopPropagation();
    this.value = this._codeEditor?.code;
    this.dispatchEvent(new UmbPropertyValueChangeEvent());
  }

  render() {
    return this.#loaded ? this.#renderCodeEditor() : this.#renderLoading();
  }

  #renderCodeEditor() {
    return html`
        <div id='code-editor'>
            <umb-code-editor language='${this.#language}'
                              .code='${this.value ?? ''}'
                              @input='${this.#onChange}'>
            </umb-code-editor>
        </div>
    `;
  }

  #renderLoading() {
    return html`<uui-loader></uui-loader>`;
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
    'contentment-property-editor-ui-code-editor': ContentmentPropertyEditorUICodeEditorElement;
  }
}
