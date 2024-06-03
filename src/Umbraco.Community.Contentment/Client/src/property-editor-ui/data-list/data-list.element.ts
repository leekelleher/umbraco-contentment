// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { ContentmentService } from '../../api/services.gen.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
// import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
// import { UMB_WORKSPACE_CONTEXT } from '@umbraco-cms/backoffice/workspace';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../components/property-editor-ui/property-editor-ui.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-data-list';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDataListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#listEditor?: ContentmentDataListEditor;

	// @state()
	// private _entityUnique?: string | null;

	// @state()
	// private _propertyAlias?: string;

	// @state()
	// private _variantId?: string;

	@state()
	private _initialized = false;

	@state()
	private _dataSource?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _listEditor?: Array<ContentmentConfigurationEditorValue>;

	@property()
	public value?: string | string[];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._dataSource = config.getValueByAlias('dataSource');
		this._listEditor = config.getValueByAlias('listEditor');
	}

	constructor() {
		super();

		// this.consumeContext(UMB_WORKSPACE_CONTEXT, (entityContext) => {
		// 	// @ts-ignore
		// 	this.observe(entityContext.unique, (unique) => (this._entityUnique = unique));
		// });

		// this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
		// 	this.observe(propertyContext.alias, (alias) => (this._propertyAlias = alias));
		// 	this.observe(propertyContext.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		// });
	}

	protected async firstUpdated() {
		await Promise.all([await this.#init()]);
		this._initialized = true;
	}

	async #init() {
		this.#listEditor = await new Promise<ContentmentDataListEditor>(async (resolve, reject) => {
			if (!this._dataSource || !this._listEditor) return reject();

			const requestBody = { dataSource: this._dataSource, listEditor: this._listEditor };

			const { data } = await tryExecuteAndNotify(
				this,
				ContentmentService.postContentmentDataListEditor({ requestBody })
			);

			if (!data) return reject();

			const listEditor = {
				propertyEditorUiAlias: data.propertyEditorUiAlias,
				config: new UmbPropertyEditorConfigCollection(data.config ?? []),
			};

			resolve(listEditor);
		});
	}

	#onChange(event: UmbPropertyValueChangeEvent & { target: UmbPropertyEditorUiElement }) {
		var element = event.target;
		if (!element || element.value === this.value) return;
		this.value = element.value as any;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<contentment-lee-was-here></contentment-lee-was-here>`;
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
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDataListElement;
	}
}
