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
import type { ContentmentConfigurationEditorItem, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../components/property-editor-ui/property-editor-ui.element.js';

@customElement('contentment-property-editor-ui-data-list')
export default class ContentmentPropertyEditorUIDataListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#listEditor?: ContentmentDataListEditor;

	@state()
	private _initialized = false;

	@state()
	private _dataSource?: Array<ContentmentConfigurationEditorItem>;

	@state()
	private _listEditor?: Array<ContentmentConfigurationEditorItem>;

	@property()
	public value?: string | string[];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._dataSource = config.getValueByAlias('dataSource');
		this._listEditor = config.getValueByAlias('listEditor');
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

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-list': ContentmentPropertyEditorUIDataListElement;
	}
}
