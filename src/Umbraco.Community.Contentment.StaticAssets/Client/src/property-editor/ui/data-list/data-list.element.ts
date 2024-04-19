// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection, UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import { umbExtensionsRegistry, type UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

type ContentmentDataListItem = { name: string, value: string, description?: string, icon?: string };
type ContentmentDataListEditor = { propertyEditorUiAlias?: string, config?: UmbPropertyEditorConfigCollection };

@customElement('contentment-property-editor-ui-data-list')
export class ContentmentPropertyEditorUIDataListElement extends UmbLitElement implements UmbPropertyEditorUiElement {

  #listEditor?: ContentmentDataListEditor;

  @state()
  private _initialized = false;

  @property()
  public value?: string | string[];

  @property({ attribute: false })
  public config?: UmbPropertyEditorConfigCollection;

  constructor() {
    super();

    this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
      this.observe(propertyContext.value, (propertyValue) => {
        console.log('data-list.propertyContext.value', propertyValue);
      });
    });
  }

  protected async firstUpdated() {
    await Promise.all([
      (await this.#init()),
    ]);
    this._initialized = true;
  }

  async #init() {
    this.#listEditor = await new Promise<ContentmentDataListEditor>(async (resolve, reject) => {

      if (!this.config) return reject();

      //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//
      // TODO: [LK] Make a call to the server to get the list of items. //
      //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!//

      // TODO: [LK] Other next steps.
      // Fresh VS projects: Cms.Web.Api.Management; Cms.Web.StaticAssets;
      // maybe keep 'Umbraco.Community.Contentment' as the core library?
      // Then I can generate the OpenAPI TypeScript bindings, etc.




      //const foo = this.config[0].value[0].value.items as Array<ContentmentDataListItem>;
      //const list = foo.map(x => x.name);

      //const { data } = await tryExecuteAndNotify(this, DataTypeResource.getFilterDataType({ take: 50 }));
      //const list = data?.items.reverse().map(x => x.name);

      const manifests = await this.observe(umbExtensionsRegistry.byType('propertyEditorUi'), () => { }).asPromise();
      const list: Array<ContentmentDataListItem> = manifests.reverse().map(manifest => ({
        name: manifest.meta.label ?? manifest.name,
        value: manifest.alias,
        icon: manifest.meta.icon,
        description: manifest.elementName,
      }));

      const listEditor = {
        propertyEditorUiAlias: 'Umb.PropertyEditorUi.Dropdown',
        config: new UmbPropertyEditorConfigCollection([
          { alias: 'items', value: list },
        ])
      };

      resolve(listEditor);
      //setTimeout(() => resolve(listEditor), 500);
    });
  }

  #onPropertyValueChange(event: UmbPropertyValueChangeEvent) {
    var element = event.target as UmbPropertyEditorUiElement;
    if (!element || element.value === this.value) return;
    this.value = element.value as any;
    this.dispatchEvent(new UmbPropertyValueChangeEvent());
  }

  render() {
    if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
    return html`
            <contentment-property-editor-ui
                .config=${this.#listEditor.config}
                .propertyEditorUiAlias=${this.#listEditor.propertyEditorUiAlias}
                .value=${this.value}
                @property-value-change=${this.#onPropertyValueChange}>
            </contentment-property-editor-ui>
            <details>
            <summary>Debug</summary>
            <pre><code>${JSON.stringify(this.#listEditor.config, null, 2)}</code></pre>
            </details>
        `;
  }
}

export default ContentmentPropertyEditorUIDataListElement;

declare global {
  interface HTMLElementTagNameMap {
    'contentment-property-editor-ui-data-list': ContentmentPropertyEditorUIDataListElement;
  }
}
