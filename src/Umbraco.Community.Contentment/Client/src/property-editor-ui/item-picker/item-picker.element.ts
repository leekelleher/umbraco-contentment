// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import type { ContentmentDataListItem } from '../types.js';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { CONTENTMENT_ITEM_PICKER_MODAL } from './item-picker-modal.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-item-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIItemPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _items: Array<ContentmentDataListItem> = [];

	#lookup: Record<string, ContentmentDataListItem> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;

		this._items = config.getValueByAlias<Array<ContentmentDataListItem>>('items') ?? [];

		this.#populateItemLookup();
	}

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	#getItemByValue(value: string): ContentmentDataListItem | undefined {
		return this.#lookup[value];
	}

	#populateItemLookup() {
		if (!this._items) return;
		this._items.forEach((item) => {
			this.#lookup[item.value] = item;
		});
	}

	#setValue(value: Array<string> | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp.splice(index, 0, ...value);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	async #onChoose() {
		if (!this.#modalManager) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_ITEM_PICKER_MODAL, {
			data: { items: this._items ?? [] },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data?.selection, this.value?.length ?? 0);
	}

	async #removeItem(item: ContentmentDataListItem, index: number) {
		console.log('#removeItem', item);
		//this.#pickerContext?.requestRemoveItem(item.unique);

		if (!item || !this.value || index == -1) return;

		await umbConfirmModal(this, {
			color: 'danger',
			headline: 'Remove item?',
			content: 'Are you sure you want to remove this item?',
			confirmLabel: this.localize.term('general_remove'),
		});

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (this.value && this.value.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#onChoose}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.value) return;
		return html`
			<uui-ref-list>
				${repeat(
					this.value,
					(value) => value,
					(value, index) => this.#renderItem(value, index)
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(value: string, index: number) {
		const item = this.#getItemByValue(value);
		if (!item) return;
		const icon = this.#renderMetadata(value, item, 'icon');
		return html`
			<uui-ref-node
				name=${this.#renderMetadata(value, item, 'name') ?? value}
				detail=${this.#renderMetadata(value, item, 'description') ?? ''}
				?standalone=${this.#maxItems === 1}>
				${when(icon, () => html`<uui-icon slot="icon" name=${icon!}></uui-icon>`)}
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						@click=${() => this.#removeItem(item, index)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	#renderMetadata(value: string, item: ContentmentDataListItem, key: string): string | unknown | undefined {
		return item[key] ?? value;
	}

	static override styles = [
		css`
			#btn-add {
				display: block;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIItemPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIItemPickerElement;
	}
}
