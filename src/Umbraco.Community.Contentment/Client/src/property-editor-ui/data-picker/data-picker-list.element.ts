import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, parseInt } from '../../utils/index.js';
import { ContentmentDataListItem } from '../types.js';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import { CONTENTMENT_DATA_PICKER_MODAL } from './data-picker-modal.element.js';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

const ELEMENT_NAME = 'contentment-property-editor-ui-data-picker-list';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDataPickerListElement
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
				listType: 'list',
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
						@click=${() => this.#removeItem(item, index)}></uui-button>
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

export { ContentmentPropertyEditorUIDataPickerListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDataPickerListElement;
	}
}
