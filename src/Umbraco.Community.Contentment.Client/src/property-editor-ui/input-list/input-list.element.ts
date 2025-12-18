// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentSortEndEvent } from '../../components/sortable-list/sort-end.event.js';
import type { UmbDataTypeDetailModel } from '@umbraco-cms/backoffice/data-type';
import type { ManifestPropertyEditorUi, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

interface ContentmentInputListControl {
	dataType: UmbDataTypeDetailModel;
	manifest: ManifestPropertyEditorUi | undefined;
}

interface ContentmentInputListItem {
	key: string;
	values: Record<string, unknown>;
	elements: Map<string, UmbPropertyEditorUiElement>;
}

type ContentmentInputListValue = Array<Record<string, unknown>>;

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#confirmRemoval = false;

	#disableSorting = false;

	#maxItems = Infinity;

	#repository = new UmbDataTypeDetailRepository(this);

	@property({ type: Array })
	public set value(value: ContentmentInputListValue | undefined) {
		this.#value = value ?? [];
		this.#syncItemsWithValue();
	}
	public get value(): ContentmentInputListValue | undefined {
		return this.#value;
	}
	#value: ContentmentInputListValue = [];

	@state()
	private _controls: Array<ContentmentInputListControl> = [];

	@state()
	private _items: Array<ContentmentInputListItem> = [];

	@state()
	private _ready = false;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1;

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
					? ((await firstValueFrom(
							umbExtensionsRegistry.byTypeAndAlias('propertyEditorUi', dataType.editorUiAlias)
					  )) as ManifestPropertyEditorUi | undefined)
					: undefined;

				return { dataType, manifest };
			})
		);

		this._controls = results.filter((r): r is ContentmentInputListControl => r !== null);
		this._ready = true;

		// Initialize items from existing value
		await this.#syncItemsWithValue();
	}

	async #syncItemsWithValue() {
		if (!this._ready || this._controls.length === 0) return;

		// Build items from value, reusing existing item elements where possible
		const existingItemsByKey = new Map(this._items.map((item) => [item.key, item]));
		const newItems: Array<ContentmentInputListItem> = [];

		for (let i = 0; i < this.#value.length; i++) {
			const itemValue = this.#value[i];
			// Use index as key for now (could use a UUID stored in value for more stability)
			const key = `item-${i}`;

			const existingItem = existingItemsByKey.get(key);
			if (existingItem) {
				// Update existing item's values and element values
				existingItem.values = itemValue;
				this.#updateItemElementValues(existingItem);
				newItems.push(existingItem);
			} else {
				// Create new item
				const item = await this.#createItem(key, itemValue);
				newItems.push(item);
			}
		}

		this._items = newItems;
	}

	async #createItem(key: string, values: Record<string, unknown>): Promise<ContentmentInputListItem> {
		const elements = new Map<string, UmbPropertyEditorUiElement>();

		for (const control of this._controls) {
			if (!control.manifest) continue;

			const element = await createExtensionElement(control.manifest);
			if (!element) continue;

			// Configure the element
			element.value = values[control.dataType.unique];
			element.config = new UmbPropertyEditorConfigCollection(control.dataType.values);

			// Listen for changes. ('property-value-change' is for backwards compatibility, to be removed in Umb v18). [LK]
			element.addEventListener('change', () => this.#onInputChange(key, control.dataType.unique, element.value));
			/** @deprecated The `UmbPropertyValueChangeEvent` has been deprecated, and will be removed in Umbraco 18. [LK] */
			element.addEventListener('property-value-change', () =>
				this.#onInputChange(key, control.dataType.unique, element.value)
			);

			elements.set(control.dataType.unique, element);
		}

		return { key, values, elements };
	}

	#updateItemElementValues(item: ContentmentInputListItem) {
		for (const control of this._controls) {
			const element = item.elements.get(control.dataType.unique);
			if (element) {
				element.value = item.values[control.dataType.unique];
			}
		}
	}

	#onInputChange(itemKey: string, dataTypeKey: string, value: unknown) {
		const itemIndex = this._items.findIndex((r) => r.key === itemKey);
		if (itemIndex === -1) return;

		// Update internal state - create new objects to avoid mutation
		const updatedItem = {
			...this._items[itemIndex],
			values: { ...this._items[itemIndex].values, [dataTypeKey]: value },
		};
		const newItems = [...this._items];
		newItems[itemIndex] = updatedItem;
		this._items = newItems;

		// Update external value
		const newValue = [...this.#value];
		newValue[itemIndex] = { ...newValue[itemIndex], [dataTypeKey]: value };
		this.#value = newValue;

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onAdd() {
		// Create empty item value
		const itemValue: Record<string, unknown> = {};
		this._controls.forEach((control) => (itemValue[control.dataType.unique] = undefined));

		// Create new item
		const key = `item-${this._items.length}`;
		const item = await this.#createItem(key, itemValue);

		this._items = [...this._items, item];
		this.#value = [...this.#value, itemValue];

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onRemove(index: number) {
		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: this.localize.term('contentment_removeItemHeadline'),
				content: this.localize.term('contentment_removeItemMessage'),
				confirmLabel: this.localize.term('contentment_removeItemButton'),
			});
		}

		const items = [...this._items];
		items.splice(index, 1);
		this._items = items;

		const value = [...this.#value];
		value.splice(index, 1);
		this.#value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSortEnd(event: ContentmentSortEndEvent) {
		if (event.newIndex === undefined || event.oldIndex === undefined) return;

		const items = [...this._items];
		items.splice(event.newIndex, 0, items.splice(event.oldIndex, 1)[0]);
		this._items = items;

		const value = [...this.#value];
		value.splice(event.newIndex, 0, value.splice(event.oldIndex, 1)[0]);
		this.#value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._ready) return html`<uui-loader></uui-loader>`;
		return html`${this.#renderItems()}${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (this._items.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_add')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderItems() {
		if (this._items.length === 0) return nothing;
		return html`
			<contentment-sortable-list
				item-selector=".item"
				handle-selector=".handle"
				?disabled=${this.#disableSorting}
				@sort-end=${this.#onSortEnd}>
				${repeat(
					this._items,
					(item) => item.key,
					(item, index) => this.#renderItem(item, index)
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(item: ContentmentInputListItem, index: number) {
		return html`
			<div class="item">
				${when(!this.#disableSorting, () => html`<div class="handle"><uui-icon name="icon-grip"></uui-icon></div>`)}
				<div class="inputs">
					${repeat(
						this._controls,
						(control) => control.dataType.unique,
						(control) => html`${item.elements.get(control.dataType.unique) ?? html`<uui-loader></uui-loader>`}`
					)}
				</div>
				<div class="actions">
					<uui-button label=${this.localize.term('general_remove')} @click=${() => this.#onRemove(index)}>
						<uui-icon name="delete"></uui-icon>
					</uui-button>
				</div>
			</div>
		`;
	}

	static override readonly styles = [
		css`
			#btn-add {
				display: block;
			}

			contentment-sortable-list {
				display: flex;
				flex-direction: column;
				gap: 1px;
				margin-bottom: var(--uui-size-1);
			}

			.item {
				display: flex;
				flex-direction: row;
				align-items: center;
				gap: var(--uui-size-6);

				padding: var(--uui-size-3) var(--uui-size-6);
				background-color: var(--uui-color-surface-alt);
				border-radius: var(--uui-border-radius);

				&[drag-placeholder] {
					opacity: 0.5;
				}

				> .handle {
					cursor: grab;
				}

				> .inputs {
					flex: 1;
					display: flex;
					flex-direction: row;
					gap: var(--uui-size-4);
				}

				> .actions {
					flex: 0 0 auto;
					display: flex;
					justify-content: flex-end;
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUIInputListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list': ContentmentPropertyEditorUIInputListElement;
	}
}
