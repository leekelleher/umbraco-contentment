// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataListItem } from '../types.js';
import {
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	repeat,
	state,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import type { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

import '../../components/info-box/info-box.element.js';

interface ContentmentItemPickerModalData {
	defaultIcon?: string;
	enableFilter: boolean;
	enableMultiple: boolean;
	items: Array<ContentmentDataListItem>;
	listType: string;
	maxItems: number;
}

interface ContentmentItemPickerModalValue {
	selection: Array<string>;
}

export const CONTENTMENT_ITEM_PICKER_MODAL = new UmbModalToken<
	ContentmentItemPickerModalData,
	ContentmentItemPickerModalValue
>('Umb.Contentment.Modal.ItemPicker', {
	modal: {
		type: 'sidebar',
		size: 'small',
	},
});

@customElement('contentment-property-editor-ui-item-picker-modal')
export class ContentmentPropertyEditorUIItemPickerModalElement extends UmbModalBaseElement<
	ContentmentItemPickerModalData,
	ContentmentItemPickerModalValue
> {
	@state()
	private _allowSubmit = false;

	@state()
	private _itemCount = 0;

	@state()
	private _items: Array<ContentmentDataListItem> = [];

	override connectedCallback() {
		super.connectedCallback();
		this._items = this.data?.items ? this.data.items : [];
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		query = (query || '').toLocaleLowerCase();
		this._items = query ? this.data.items.filter((item) => this.#predicate(query, item)) : this.data.items;
	}, 100);

	#predicate = (query: string, item: ContentmentDataListItem) =>
		item.name.toLocaleLowerCase().includes(query) ||
		item.value.toLocaleLowerCase().includes(query) ||
		item.description?.toLocaleLowerCase().includes(query);

	#onSelect(item: ContentmentDataListItem) {
		if (item.disabled) return;

		if (this.data?.enableMultiple) {
			item.selected = !item.selected;
			this._itemCount = this._items.filter((x) => x.selected === true).length;
			this._allowSubmit = this._itemCount > 0 && (this.data?.maxItems === 0 || this._itemCount <= this.data?.maxItems);
		} else {
			this.value = { selection: [item.value] };
			this._submitModal();
		}
	}

	#onSubmit() {
		const selection: Array<string> = [];

		this._items.forEach((item) => {
			if (item.selected) {
				delete item.selected;
				selection.push(item.value);
			}
		});

		this.value = { selection };

		this._submitModal();
	}

	#onInput(event: UUIInputEvent) {
		this.#debouncedFilter(event.target.value);
	}

	override render() {
		return html`
			<umb-body-layout headline=${this.localize.term('general_choose')}>
				${this.#renderFilter()} ${this.#renderInfo()} ${this.#renderItems()}
				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
					${when(
						this.data?.enableMultiple,
						() => html`
							<uui-button
								color="positive"
								look="primary"
								label=${this.localize.term('buttons_select')}
								?disabled=${!this._allowSubmit}
								@click=${this.#onSubmit}></uui-button>
						`
					)}
				</div>
			</umb-body-layout>
		`;
	}

	#renderFilter() {
		if (!this.data?.enableFilter) return nothing;
		const label = this.localize.term('placeholders_filter');
		return html`
			<uui-input
				type="search"
				id="filter"
				autocomplete="off"
				label=${label}
				placeholder=${label}
				@input=${this.#onInput}
				${umbFocus()}>
				<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
			</uui-input>
		`;
	}

	#renderInfo() {
		if (
			!this.data?.enableMultiple ||
			this._itemCount === 0 ||
			this.data?.maxItems === 0 ||
			this._itemCount <= this.data?.maxItems
		) {
			return nothing;
		}

		return html`
			<contentment-info-box type="danger">
				<span>
					Too many items selected, please unselect
					<strong>${this._itemCount - (this.data?.maxItems ?? 0)}</strong>
					${this._itemCount - (this.data?.maxItems ?? 0) === 1 ? 'item' : 'items'}.
				</span>
			</contentment-info-box>
		`;
	}

	#renderItems() {
		if (!this._items?.length) return html`<uui-box><p>No items found</p></uui-box>`;
		return html`
			<uui-box>
				<uui-ref-list>
					${repeat(
						this._items,
						(item) => item.key,
						(item) => html`
							<umb-ref-item
								name=${item.name}
								detail=${ifDefined(item.description)}
								icon=${ifDefined(item.icon ?? 'icon-document' ?? this.data?.defaultIcon)}
								@click=${() => this.#onSelect(item)}>
							</umb-ref-item>
						`
					)}
				</uui-ref-list>
			</uui-box>
		`;
	}

	static override styles = [
		css`
			#filter {
				width: 100%;
				margin-bottom: var(--uui-size-space-4);
			}

			#filter-icon {
				display: flex;
				color: var(--uui-color-border);
				height: 100%;
				padding-left: var(--uui-size-space-2);
			}

			contentment-info-box {
				margin-bottom: var(--uui-size-space-4);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIItemPickerModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-item-picker-modal': ContentmentPropertyEditorUIItemPickerModalElement;
	}
}
