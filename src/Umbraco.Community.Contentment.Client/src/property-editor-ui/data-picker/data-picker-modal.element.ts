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
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentDataListItem } from '../types.js';
import { DataPickerService } from '../../api/sdk.gen.js';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UMB_CONTENT_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/content';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UUIInputEvent, UUIPaginationEvent } from '@umbraco-cms/backoffice/external/uui';

import '../../components/info-box/info-box.element.js';

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
	selection: Array<ContentmentDataListItem>;
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
	private _items: Array<ContentmentDataListItem> = [];

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

		this.consumeContext('UmbMenuStructureWorkspaceContext', (context: any) => {
			this.observe(context.structure, (structure: Array<{ unique: string }>) => {
				this._entityUnique = structure.at(-1)?.unique;
			});
		});

		this.consumeContext(UMB_CONTENT_PROPERTY_CONTEXT, (context) => {
			this.observe(context.dataType, (dataType) => {
				this._dataTypeKey = dataType?.unique;
			});
		});

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext.alias, (alias) => (this._propertyAlias = alias));
			this.observe(propertyContext.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
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

	#onSelect(item: ContentmentDataListItem) {
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
		const selection: Array<ContentmentDataListItem> = [];

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
		const requestData = {
			alias: this._propertyAlias,
			dataTypeKey: this._dataTypeKey,
			id: this._entityUnique,
			pageNumber: this._pageNumber,
			pageSize: this.data?.pageSize ?? 12,
			query: this._query,
			variant: this._variantId,
		};

		const { data } = await tryExecuteAndNotify(this, DataPickerService.getDataPickerSearch(requestData));

		this._items =
			data?.items.map((item) => ({
				name: item.name ?? '',
				value: item.value ?? '',
				description: item.description ?? '',
				icon: item.icon ?? '',
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
			${when(
				this._totalPages > 1,
				() => html`
					<uui-pagination current=${this._pageNumber} total=${this._totalPages} @change=${this.#onPagination}>
					</uui-pagination>
				`
			)}
		`;
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
		`,
	];
}

export { ContentmentPropertyEditorUIDataPickerModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-picker-modal': ContentmentPropertyEditorUIDataPickerModalElement;
	}
}
