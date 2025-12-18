// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbDataTypeDetailModel } from '@umbraco-cms/backoffice/data-type';
import type {
	ManifestPropertyEditorUi,
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

interface InputListField {
	dataType: UmbDataTypeDetailModel;
	manifest: ManifestPropertyEditorUi | undefined;
}

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#repository = new UmbDataTypeDetailRepository(this);

	@property()
	public value: string = '';

	@state()
	private _fields: Array<InputListField> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const dataTypes = config.getValueByAlias<Array<string>>('dataTypes');

		if (dataTypes?.length) {
			this.#loadDataTypes(dataTypes);
		}
	}

	async #loadDataTypes(uniques: Array<string>) {
		const results = await Promise.all(
			uniques.map(async (unique) => {
				const { data: dataType } = await this.#repository.requestByUnique(unique);
				if (!dataType) return null;

				const manifest = dataType.editorUiAlias
					? ((await firstValueFrom(umbExtensionsRegistry.byTypeAndAlias('propertyEditorUi', dataType.editorUiAlias))) as ManifestPropertyEditorUi | undefined)
					: undefined;

				return { dataType, manifest };
			})
		);

		this._fields = results.filter((r): r is InputListField => r !== null);
	}

	constructor() {
		super();
	}

	override render() {
		return html`<umb-code-block language="json">${JSON.stringify(this._fields, null, 2)}</umb-code-block>`;
	}

	static override readonly styles = [css``];
}

export { ContentmentPropertyEditorUIInputListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list': ContentmentPropertyEditorUIInputListElement;
	}
}
