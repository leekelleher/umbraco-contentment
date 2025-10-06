// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

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
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { DataPickerService } from '../../api/sdk.gen.js';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import {
	UMB_CONTENT_WORKSPACE_CONTEXT,
	UMB_PROPERTY_TYPE_BASED_PROPERTY_CONTEXT,
} from '@umbraco-cms/backoffice/content';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { ContentmentListItem } from '../types.js';
import type { UUIInputEvent, UUIPaginationEvent } from '@umbraco-cms/backoffice/external/uui';

interface ContentmentDataPickerModalData {
	allowDuplicates: boolean;
	defaultIcon?: string;
	enableMultiple: boolean;
	hideSearch: boolean;
	listType: string;
	maxItems: number;
	pageSize: number;
	value: Array<string>;
}

interface ContentmentDataPickerModalValue {
	selection: Array<ContentmentListItem>;
}

export const CONTENTMENT_DATA_PICKER_MODAL = new UmbModalToken<
	ContentmentDataPickerModalData,
	ContentmentDataPickerModalValue
>('Umb.Contentment.Modal.DataPicker', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});

@customElement('contentment-property-editor-ui-data-picker-modal')
export class ContentmentPropertyEditorUIDataPickerModalElement extends UmbModalBaseElement<
	ContentmentDataPickerModalData,
	ContentmentDataPickerModalValue
> {
	@state()
	private _allowSubmit = false;

	@state()
	private _dataTypeKey?: string;

	@state()
	private _entityUnique?: string;

	@state()
	private _itemCount = 0;

	@state()
	private _items: Array<ContentmentListItem> = [];

	@state()
	private _loading = false;

	@state()
	private _pageNumber = 1;

	@state()
	private _propertyAlias?: string;

	@state()
	private _query = '';

	@state()
	private _totalPages = 0;

	@state()
	private _variantId?: string;

	constructor() {
		super();

		this.consumeContext(UMB_CONTENT_WORKSPACE_CONTEXT, (contentWorkspaceContext) => {
			this.observe(contentWorkspaceContext?.unique, (unique) => (this._entityUnique = unique || undefined));
		}).passContextAliasMatches();

		this.consumeContext(UMB_PROPERTY_TYPE_BASED_PROPERTY_CONTEXT, (context) => {
			this.observe(context?.dataType, (dataType) => (this._dataTypeKey = dataType?.unique));
		});

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext?.alias, (alias) => (this._propertyAlias = alias));
			this.observe(propertyContext?.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		});
	}

	override async firstUpdated() {
		this.#requestItems();
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		this._loading = true;
		this._query = query;
		this._pageNumber = 1;
		this.#requestItems();
	}, 500);

	#onInput(event: UUIInputEvent) {
		this.#debouncedFilter(event.target.value);
	}

	#onSelect(item: ContentmentListItem) {
		if (item.disabled) return;

		if (this.data?.enableMultiple) {
			item.selected = !item.selected;
			this._itemCount = this._items.filter((x) => x.selected === true).length;
			this._allowSubmit = this._itemCount > 0 && (this.data?.maxItems === 0 || this._itemCount <= this.data?.maxItems);
		} else {
			this.value = { selection: [item] };
			this._submitModal();
		}
	}

	#onPagination(event: UUIPaginationEvent) {
		this._pageNumber = event.target.current;
		this.#requestItems();
	}

	#onSubmit() {
		const selection: Array<ContentmentListItem> = [];

		this._items.forEach((item) => {
			if (item.selected) {
				delete item.selected;
				selection.push(item);
			}
		});

		this.value = { selection };

		this._submitModal();
	}

	async #requestItems() {
		const query = {
			alias: this._propertyAlias,
			dataTypeKey: this._dataTypeKey,
			id: this._entityUnique,
			pageNumber: this._pageNumber,
			pageSize: this.data?.pageSize ?? 12,
			query: this._query,
			variant: this._variantId,
		};

		const { data } = await tryExecute(this, DataPickerService.getDataPickerSearch({ client: umbHttpClient, query }));

		this._items =
			data?.items.map((item) => ({
				...item,
				name: item.name ?? item.value ?? '',
				value: item.value ?? '',
			})) ?? [];

		this._loading = false;
		this._totalPages = data?.total ?? 0;
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
		if (this.data?.hideSearch) return nothing;
		const label = this.localize.term('placeholders_filter');
		return html`
			<uui-input
				type="search"
				id="filter"
				autocomplete="off"
				label=${label}
				placeholder=${label}
				.value=${this._query}
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
			this._itemCount <= (this.data?.maxItems ?? Infinity)
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
		if (this._loading) return html`<uui-loader></uui-loader>`;
		if (!this._totalPages) return this.#renderNoItems();
		return html`
			${when(
				// HACK: [LK] Until I figure out how to render custom display modes in the modal.
				this.data?.listType === 'cards',
				() => html`
					<section>
						${repeat(
							this._items,
							(item) => item.key,
							(item) => this.#renderItem(item)
						)}
					</section>
				`,
				() => html`
					<uui-box>
						<uui-ref-list>
							${repeat(
								this._items,
								(item) => item.key,
								(item) => this.#renderItem(item)
							)}
						</uui-ref-list>
					</uui-box>
				`
			)}
			${when(
				this._totalPages > 1,
				() => html`
					<uui-pagination current=${this._pageNumber} total=${this._totalPages} @change=${this.#onPagination}>
					</uui-pagination>
				`
			)}
		`;
	}

	#renderItem(item: ContentmentListItem) {
		return when(
			// HACK: [LK] Until I figure out how to render custom display modes in the modal.
			this.data?.listType === 'cards',
			() => html`
				<uui-card-media
					name=${item.name}
					detail=${ifDefined(item.description ?? undefined)}
					select-only
					selectable
					@selected=${() => this.#onSelect(item)}>
					${when(!item.image && item.icon, (_icon) => html`<umb-icon name=${_icon}></umb-icon>`)}
					${when(item.image, () => html`<img src=${item.image!} alt="" />`)}
				</uui-card-media>
			`,
			() => html`
				<umb-ref-item
					name=${item.name}
					detail=${ifDefined(item.description ?? undefined)}
					icon=${ifDefined(item.icon ?? this.data?.defaultIcon ?? 'icon-document')}
					select-only
					selectable
					@selected=${() => this.#onSelect(item)}
					@deselected=${() => this.#onSelect(item)}>
				</umb-ref-item>
			`
		);
	}

	#renderNoItems() {
		return html`
			<uui-box>
				<p>
					<umb-localize key="general_searchNoResult">Sorry, we can not find what you are looking for.</umb-localize>
				</p>
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

			contentment-info-box,
			uui-box {
				margin-bottom: var(--uui-size-space-4);
			}

			section {
				display: grid;
				gap: var(--uui-size-space-3);
				grid-template-columns: repeat(auto-fill, minmax(160px, 1fr));
				grid-template-rows: repeat(auto-fill, minmax(160px, 1fr));
				margin-bottom: var(--uui-size-space-5);

				uui-card-media {
					min-height: 160px;
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUIDataPickerModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-picker-modal': ContentmentPropertyEditorUIDataPickerModalElement;
	}
}
