// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbDataTypeDetailModel } from '@umbraco-cms/backoffice/data-type';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#repository = new UmbDataTypeDetailRepository(this);

	@property()
	public value: string = '';

	@state()
	private _dataTypes: Array<UmbDataTypeDetailModel> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const dataTypeKeys = config.getValueByAlias<Array<string>>('dataTypes');
		if (dataTypeKeys?.length) {
			this.#loadDataTypes(dataTypeKeys);
		}
	}

	async #loadDataTypes(keys: Array<string>) {
		const results = await Promise.all(keys.map((key) => this.#repository.requestByUnique(key)));

		this._dataTypes = results
			.map((result) => result.data)
			.filter((data): data is UmbDataTypeDetailModel => data !== undefined);
	}

	constructor() {
		super();
	}

	override render() {
		return html`<umb-code-block language="json">${JSON.stringify(this._dataTypes, null, 2)}</umb-code-block>`;
	}

	static override readonly styles = [css``];
}

export { ContentmentPropertyEditorUIInputListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list': ContentmentPropertyEditorUIInputListElement;
	}
}
