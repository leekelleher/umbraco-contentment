// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { CONTENTMENT_ITEM_PICKER_MODAL } from './item-picker-modal.element.js';
import type { ContentmentDataListItem } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import '../../components/sortable-list/sortable-list.element.js';

@customElement('contentment-property-editor-ui-item-picker')
export class ContentmentPropertyEditorUIItemPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _items: Array<ContentmentDataListItem> = [];

	#allowDuplicates = false;

	#confirmRemoval = false;

	#defaultIcon?: string;

	#disableSorting = false;

	#enableFilter = true;

	#enableMultiple = false;

	#listType = 'list';

	#lookup: Record<string, ContentmentDataListItem> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	#overlaySize: UUIModalSidebarSize = 'small';

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

		this.#allowDuplicates = parseBoolean(config.getValueByAlias('allowDuplicates'));
		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon');
		this.#enableFilter = parseBoolean(config.getValueByAlias('enableFilter') ?? '1');
		this.#enableMultiple = parseBoolean(config.getValueByAlias('enableMultiple'));
		this.#listType = config.getValueByAlias<string>('listType') ?? 'list';
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#overlaySize = config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'small';
		this.#disableSorting = this.#maxItems === 1 ? true : parseBoolean(config.getValueByAlias('disableSorting'));

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

	#getMetadata(item: ContentmentDataListItem, key: string): string | unknown | undefined {
		return item[key];
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

		const items = this.#allowDuplicates
			? this._items
			: this._items.filter((x) => this.value?.some((y) => x.value === y) === false);

		const modal = this.#modalManager.open(this, CONTENTMENT_ITEM_PICKER_MODAL, {
			data: {
				defaultIcon: this.#defaultIcon,
				enableFilter: this.#enableFilter,
				enableMultiple: this.#enableMultiple,
				items: items ?? [],
				listType: this.#listType,
				maxItems: this.#maxItems === 0 ? this.#maxItems : this.#maxItems - (this.value?.length ?? 0),
			},
			modal: { size: this.#overlaySize },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data?.selection, this.value?.length ?? 0);
	}

	async #onRemove(item: ContentmentDataListItem, index: number) {
		if (!item || !this.value || index == -1) return;

		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: 'Remove item?',
				content: 'Are you sure you want to remove this item?',
				confirmLabel: this.localize.term('general_remove'),
			});
		}

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onSortEnd(event: CustomEvent<{ newIndex: number; oldIndex: number }>) {
		const items = [...(this.value ?? [])];
		items.splice(event.detail.newIndex, 0, items.splice(event.detail.oldIndex, 1)[0]);
		this.value = items;

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
			<contentment-sortable-list
				class="uui-ref-list"
				item-selector="uui-ref-node"
				?disabled=${this.#disableSorting}
				@sort-end=${this.#onSortEnd}>
				${repeat(
					this.value,
					(value) => value,
					(value, index) => this.#renderItem(value, index)
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(value: string, index: number) {
		const item = this.#getItemByValue(value);
		if (!item) return;
		const icon = this.#getMetadata(item, 'icon') ?? this.#defaultIcon;
		return html`
			<uui-ref-node
				name=${this.#getMetadata(item, 'name') ?? value}
				detail=${this.#getMetadata(item, 'description') ?? ''}
				?standalone=${this.#maxItems === 1}>
				${when(icon, () => html`<umb-icon slot="icon" name=${icon!}></umb-icon>`)}
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						@click=${() => this.#onRemove(item, index)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
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
		'contentment-property-editor-ui-item-picker': ContentmentPropertyEditorUIItemPickerElement;
	}
}
