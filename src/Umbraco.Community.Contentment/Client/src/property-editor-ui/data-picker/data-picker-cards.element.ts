import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, parseInt } from '../../utils/index.js';
import { ContentmentDataListItem } from '../types.js';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import { CONTENTMENT_DATA_PICKER_MODAL } from './data-picker-modal.element.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-data-picker-cards';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDataPickerCardsElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@state()
	private _items: Array<ContentmentDataListItem> = [];

	#allowDuplicates = false;

	#confirmRemoval = false;

	#defaultIcon?: string;

	//#disableSorting = false;

	#hideSearch = true;

	#pageSize = 12;

	#lookup: Record<string, ContentmentDataListItem> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	#overlaySize: UUIModalSidebarSize = 'small';

	@property({ type: Array })
	public value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#allowDuplicates = parseBoolean(config.getValueByAlias('allowDuplicates') ?? true);
		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon');
		//this.#disableSorting = parseBoolean(config.getValueByAlias('disableSorting'));
		this.#hideSearch = parseBoolean(config.getValueByAlias('hideSearch') ?? false);
		this.#pageSize = parseInt(config.getValueByAlias('pageSize')) || 12;
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#overlaySize = config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'small';

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

	async #onChoose() {
		if (!this.#modalManager) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_DATA_PICKER_MODAL, {
			data: {
				allowDuplicates: this.#allowDuplicates,
				defaultIcon: this.#defaultIcon,
				enableMultiple: this.#maxItems !== 1,
				hideSearch: this.#hideSearch,
				listType: 'cards',
				maxItems: this.#maxItems === 0 ? this.#maxItems : this.#maxItems - (this.value?.length ?? 0),
				pageSize: this.#pageSize,
				value: this.value ?? [],
			},
			modal: { size: this.#overlaySize },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data?.selection, this.value?.length ?? 0);
	}

	async #removeItem(item: ContentmentDataListItem, index: number) {
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

	#setValue(value: Array<ContentmentDataListItem> | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		value.forEach((item) => {
			this.#lookup[item.value] = item;
		});

		const tmp = [...this.value];
		tmp.splice(index, 0, ...value.map((x) => x.value));
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`<div class="container">${this.#renderItems()} ${this.#renderAddButton()}</div>`;
	}

	#renderAddButton() {
		if (this.value && this.value.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#onChoose}>
				<uui-icon name="icon-add"></uui-icon>
				<umb-localize key="general_choose">Choose</umb-localize>
			</uui-button>
		`;
	}

	#renderItems() {
		if (!this.value) return;
		return html`
			${repeat(
				this.value,
				(value) => value,
				(value, index) => this.#renderItem(value, index)
			)}
		`;
	}

	#renderItem(value: string, index: number) {
		const item = this.#getItemByValue(value);
		if (!item) return;
		const icon = this.#getMetadata(item, 'icon') ?? this.#defaultIcon;
		return html`
			<uui-card-media
				name=${this.#getMetadata(item, 'name') ?? value}
				detail=${this.#getMetadata(item, 'description') ?? ''}>
				${when(icon, () => html`<umb-icon name=${icon!}></umb-icon>`)}
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						look="secondary"
						@click=${() => this.#removeItem(item, index)}>
						<uui-icon name="icon-trash"></uui-icon>
					</uui-button>
				</uui-action-bar>
			</uui-card-media>
		`;
	}

	static override styles = [
		css`
			:host {
				position: relative;
			}

			.container {
				display: grid;
				grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
				grid-auto-rows: 150px;
				gap: var(--uui-size-space-5);
			}

			#btn-add {
				text-align: center;
				height: 100%;
			}

			uui-icon {
				display: block;
				margin: 0 auto;
			}

			uui-card-media umb-icon {
				font-size: var(--uui-size-8);
			}

			uui-card-media[drag-placeholder] {
				opacity: 0.2;
			}

			img {
				background-image: url('data:image/svg+xml;charset=utf-8,<svg xmlns="http://www.w3.org/2000/svg" width="100" height="100" fill-opacity=".1"><path d="M50 0h50v50H50zM0 50h50v50H0z"/></svg>');
				background-size: 10px 10px;
				background-repeat: repeat;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIDataPickerCardsElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDataPickerCardsElement;
	}
}
