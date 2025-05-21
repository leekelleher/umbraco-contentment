// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentDataListRepository } from './data-list.repository.js';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_ANCESTORS_ENTITY_CONTEXT } from '@umbraco-cms/backoffice/entity';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-data-list')
export class ContentmentPropertyEditorUIDataListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#listEditor?: ContentmentDataListEditor;

	#repository = new ContentmentDataListRepository(this);

	@state()
	private _entityUnique?: string | null;

	@state()
	private _propertyAlias?: string;

	@state()
	private _variantId?: string;

	@state()
	private _initialized = false;

	@state()
	private _dataSource?: ContentmentConfigurationEditorValue;

	@state()
	private _listEditor?: ContentmentConfigurationEditorValue;

	@property()
	public value?: string | string[];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		this._listEditor = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('listEditor')?.[0];
	}

	constructor() {
		super();

		this.consumeContext(UMB_ANCESTORS_ENTITY_CONTEXT, (ancestorsEntityContext) => {
			this.observe(ancestorsEntityContext?.ancestors, (ancestors) => {
				this._entityUnique = ancestors?.at(-1)?.unique;
			});
		});

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext?.alias, (alias) => (this._propertyAlias = alias));
			this.observe(propertyContext?.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		});
	}

	override async firstUpdated() {
		await Promise.all([await this.#init().catch(() => undefined)]);
		this._initialized = true;
	}

	async #init() {
		if (!this._dataSource || !this._listEditor) return;

		this.#listEditor = await this.#repository.getEditor(
			this._dataSource,
			this._listEditor,
			this._entityUnique,
			this._propertyAlias,
			this._variantId
		);
	}

	#onChange(event: UmbChangeEvent & { target: UmbPropertyEditorUiElement }) {
		var element = event.target;
		if (!element || element.value === this.value) return;
		this.value = element.value as any;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<lee-was-here></lee-was-here>`;
		//console.log('data-list', [this._entityUnique, this._propertyAlias, this._variantId]);
		return html`
			<contentment-property-editor-ui
				.config=${this.#listEditor.config}
				.propertyEditorUiAlias=${this.#listEditor.propertyEditorUiAlias}
				.value=${this.value}
				@change=${this.#onChange}>
			</contentment-property-editor-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIDataListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-list': ContentmentPropertyEditorUIDataListElement;
	}
}
