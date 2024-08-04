// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataListItem } from '../types.js';
import { css, customElement, html, ifDefined, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

interface ContentmentItemPickerModalData {
	items: Array<ContentmentDataListItem>;
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

const ELEMENT_NAME = 'contentment-property-editor-ui-item-picker-modal';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIItemPickerModalElement extends UmbModalBaseElement<
	ContentmentItemPickerModalData,
	ContentmentItemPickerModalValue
> {
	@state()
	private _items: Array<ContentmentDataListItem> = [];

	connectedCallback() {
		super.connectedCallback();
		this._items = this.data?.items ? this.data.items : [];
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		this._items = query ? this.data.items.filter((item) => item.name.toLowerCase().includes(query)) : this.data.items;
	}, 500);

	async #onSelect(item: ContentmentDataListItem) {
		this.value = { selection: [item.value] };
		this._submitModal();
	}

	#onInput(event: UUIInputEvent) {
		const query = (event.target.value as string) || '';
		this.#debouncedFilter(query.toLowerCase());
	}

	render() {
		return html`
			<umb-body-layout headline=${this.localize.term('general_choose')}>
				${this.#renderFilter()} ${this.#renderItems()}
				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderFilter() {
		const label = this.localize.term('placeholders_filter');
		return html`
			<uui-input type="search" id="filter" label=${label} placeholder=${label} @input=${this.#onInput} ${umbFocus()}>
				<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
			</uui-input>
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
								icon=${ifDefined(item.icon)}
								@click=${() => this.#onSelect(item)}>
							</umb-ref-item>
						`
					)}
				</uui-ref-list>
			</uui-box>
		`;
	}

	static styles = [
		css`
			h4 {
				margin-top: 0.5rem;
				margin-bottom: 0.5rem;
			}

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
		`,
	];
}

export { ContentmentPropertyEditorUIItemPickerModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIItemPickerModalElement;
	}
}
