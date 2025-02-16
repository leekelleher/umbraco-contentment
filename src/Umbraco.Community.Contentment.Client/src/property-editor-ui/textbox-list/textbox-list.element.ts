// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { DataListService } from '../../api/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentConfigurationEditorValue, ContentmentDataListItem } from '../types.js';
import type { InputType, UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-textbox-list')
export class ContentmentPropertyEditorUITextboxListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#inputTypes = ['color', 'email', 'number', 'password', 'tel', 'text', 'url'];

	@property()
	value?: Record<string, string>;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._dataSource = config.getValueByAlias('dataSource');
		this._defaultIcon = config.getValueByAlias<string>('defaultIcon');

		const labelStyle = config.getValueByAlias('labelStyle') ?? 'both';
		this._hideIcon = labelStyle === 'text';
		this._hideLabel = labelStyle === 'icon';
	}

	@state()
	private _dataSource?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _defaultIcon?: string;

	@state()
	private _hideIcon = false;

	@state()
	private _hideLabel = false;

	@state()
	private _items: Array<ContentmentDataListItem> = [];

	override async firstUpdated() {
		await Promise.all([await this.#init().catch(() => undefined)]);
	}

	async #init() {
		this._items = await new Promise<Array<ContentmentDataListItem>>(async (resolve, reject) => {
			if (!this._dataSource) return reject();

			const requestBody = { dataSource: this._dataSource[0], listEditor: null };

			const { data } = await tryExecuteAndNotify(this, DataListService.postDataListEditor({ requestBody }));

			if (!data) return reject();

			const items = (data.config?.find((x) => x.alias === 'items')?.value as Array<ContentmentDataListItem>) ?? [];

			if (!this.value && items.length > 0) {
				this.value = Object.fromEntries(items.map((item) => [item.value, '']));
			}

			resolve(items);
		});
	}

	#onInput(key: string, event: UUIInputEvent) {
		this.value = { ...this.value, [key]: event.target.value as string };
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		if (!this._items?.length) return nothing;
		return html`
			<div id="wrapper">
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item)
				)}
			</div>
		`;
	}

	#renderItem(item: ContentmentDataListItem) {
		return html`
			<div class="item">
				<uui-label for="item-${item.value}" title=${item.value}>
					${when(!this._hideIcon, () => html`<umb-icon .name=${item.icon || this._defaultIcon}></umb-icon>`)}
					${when(!this._hideLabel, () => html`<span>${item.name}</span>`)}
				</uui-label>
				<uui-input
					type=${this.#inputTypes.includes(item.value) ? (item.value as InputType) : 'text'}
					id="item-${item.value}"
					.value=${this.value?.[item.value] ?? ''}
					@input=${(event: UUIInputEvent) => this.#onInput(item.value, event)}></uui-input>
			</div>
		`;
	}

	static override styles = [
		css`
			#wrapper {
				display: flex;
				flex-direction: column;
				gap: 1px;
			}

			.item {
				background-color: var(--uui-color-surface-alt);
				display: flex;
				align-items: center;
				gap: var(--uui-size-6);
				padding: var(--uui-size-3) var(--uui-size-6);
			}

			uui-label {
				display: flex;
				gap: 1rem;
				flex: 0.2;
			}

			uui-label:has(umb-icon):not(:has(span)) {
				flex: 0 0 var(--uui-size-6);
			}

			uui-label:has(span):not(:has(umb-icon)) {
				flex: 0.1;
			}

			.item > uui-input {
				flex: 1;
			}
		`,
	];
}

export { ContentmentPropertyEditorUITextboxListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-textbox-list': ContentmentPropertyEditorUITextboxListElement;
	}
}
