// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { customElement, html, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbDocumentTypeItemRepository } from '@umbraco-cms/backoffice/document-type';
import { UmbFormControlMixin } from '@umbraco-cms/backoffice/validation';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { CONTENTMENT_ITEM_PICKER_MODAL } from '../item-picker/item-picker-modal.element.js';
import { CONTENTMENT_ELEMENT_WORKSPACE_MODAL } from '../../workspace/element/index.js';
import type {
	ContentmentConfigurationEditorValue,
	ContentmentContentBlockValue,
	ContentmentListItem,
} from '../types.js';
import type { UmbDocumentTypeItemModel } from '@umbraco-cms/backoffice/document-type';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import '../../extensions/display-mode/display-mode-ui.element.js';

interface ContentBlockTypeConfig {
	nameTemplate?: string;
	overlaySize?: UUIModalSidebarSize;
}

@customElement('contentment-property-editor-ui-content-blocks')
export class ContentmentPropertyEditorUIContentBlocksElement
	extends UmbFormControlMixin<Array<ContentmentContentBlockValue> | undefined, typeof UmbLitElement, undefined>(
		UmbLitElement,
	)
	implements UmbPropertyEditorUiElement
{
	#config?: UmbPropertyEditorConfigCollection;
	#createButtonLabelKey = '#content_createEmpty';
	#disableSorting = false;
	#enableFilter = false;
	#maxItems = Infinity;
	#uiAlias = 'Umb.Contentment.DisplayMode.List';
	#blockTypeConfig = new Map<string, ContentBlockTypeConfig>();
	#docTypeItems = new Map<string, UmbDocumentTypeItemModel>();
	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	@state()
	private _items: Array<ContentmentListItem> = [];

	constructor() {
		super();
		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (ctx) => (this.#modalManager = ctx));
	}

	public override set value(value: Array<ContentmentContentBlockValue> | undefined) {
		super.value = value;
		this.#buildItems();
	}
	public override get value(): Array<ContentmentContentBlockValue> | undefined {
		return super.value;
	}

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#config = config;

		this.#createButtonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? 'content_createEmpty';
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1 ? true : parseBoolean(config.getValueByAlias('disableSorting'));
		this.#enableFilter = parseBoolean(config.getValueByAlias('enableFilter'));

		const displayModeConfig = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('displayMode');
		this.#uiAlias = displayModeConfig?.[0]?.key ?? 'Umb.Contentment.DisplayMode.List';

		const contentBlockTypes =
			config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('contentBlockTypes') ?? [];

		this.#blockTypeConfig = new Map(
			contentBlockTypes.map((cbt) => [
				cbt.key,
				{
					nameTemplate: cbt.value.nameTemplate as string | undefined,
					overlaySize: cbt.value.overlaySize as UUIModalSidebarSize | undefined,
				},
			]),
		);

		const keys = contentBlockTypes.map((cbt) => cbt.key);
		if (keys.length) {
			this.#fetchDocTypeItems(keys);
		}
	}

	async #fetchDocTypeItems(keys: Array<string>): Promise<void> {
		const repo = new UmbDocumentTypeItemRepository(this);
		const { data } = await repo.requestItems(keys);
		if (data) {
			this.#docTypeItems = new Map(data.map((item) => [item.unique, item]));
			this.#buildItems();
		}
	}

	#buildItems(): void {
		this._items = (this.value ?? []).map((block, index) => {
			const dt = this.#docTypeItems.get(block.elementType);
			const cfg = this.#blockTypeConfig.get(block.elementType);
			return {
				name: cfg?.nameTemplate ?? dt?.name ?? block.elementType,
				value: block.key,
				icon: dt?.icon ?? 'icon-document',
				data: { ...block.value, $index: index },
			};
		});
	}

	public set readonly(value: boolean) {
		this.#readonly = value;
	}
	public get readonly(): boolean {
		return this.#readonly;
	}
	#readonly = false;

	#setValue(items: Array<ContentmentContentBlockValue>): void {
		this.value = items.length ? items : undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onAdd(): Promise<void> {
		const configuredKeys = Array.from(this.#blockTypeConfig.keys());
		if (!configuredKeys.length) return;

		let elementType: string;

		if (configuredKeys.length === 1) {
			elementType = configuredKeys[0];
		} else {
			const items = configuredKeys.map((key) => {
				const dt = this.#docTypeItems.get(key);
				return {
					name: dt?.name ?? key,
					value: key,
					icon: dt?.icon ?? 'icon-document',
					description: dt?.description ?? undefined,
				};
			});

			const pickerModal = this.#modalManager?.open(this, CONTENTMENT_ITEM_PICKER_MODAL, {
				data: {
					items,
					enableFilter: this.#enableFilter,
					enableMultiple: false,
					maxItems: 1,
					listType: 'list',
				},
			});

			let picked: string | undefined;
			try {
				const result = await pickerModal?.onSubmit();
				picked = result?.selection?.[0];
			} catch {
				return;
			}

			if (!picked) return;
			elementType = picked;
		}

		const overlaySize = this.#blockTypeConfig.get(elementType)?.overlaySize ?? 'medium';

		const workspaceModal = this.#modalManager?.open(this, CONTENTMENT_ELEMENT_WORKSPACE_MODAL, {
			data: { element: { elementType, key: UmbId.new(), value: {} } },
			modal: { type: 'sidebar', size: overlaySize },
		});

		try {
			const result = await workspaceModal?.onSubmit();
			if (result?.element) {
				this.#setValue([...(this.value ?? []), result.element]);
			}
		} catch {
			// cancelled
		}
	}

	async #onEdit(event: CustomEvent<{ item: ContentmentListItem; index: number }>): Promise<void> {
		const item = this.value?.[event.detail.index];
		if (!item) return;

		const overlaySize = this.#blockTypeConfig.get(item.elementType)?.overlaySize ?? 'medium';

		const workspaceModal = this.#modalManager?.open(this, CONTENTMENT_ELEMENT_WORKSPACE_MODAL, {
			data: { element: item, readonly: this.#readonly },
			modal: { type: 'sidebar', size: overlaySize },
		});

		try {
			const result = await workspaceModal?.onSubmit();
			if (result?.element) {
				this.#setValue((this.value ?? []).map((v) => (v.key === result.element.key ? result.element : v)));
			}
		} catch {
			// cancelled
		}
	}

	async #onRemove(event: CustomEvent<{ item: ContentmentListItem; index: number }>): Promise<void> {
		const item = this.value?.[event.detail.index];
		if (!item) return;

		const name = this.#docTypeItems.get(item.elementType)?.name ?? item.elementType;

		try {
			await umbConfirmModal(this, {
				headline: this.localize.term('blockEditor_confirmDeleteBlockTitle', name),
				content: this.localize.term('blockEditor_confirmDeleteBlockMessage', name),
				confirmLabel: '#general_delete',
				color: 'danger',
			});
		} catch {
			return;
		}

		this.#setValue((this.value ?? []).filter((v) => v.key !== item.key));
	}

	#onSort(event: CustomEvent<{ newIndex: number; oldIndex: number }>): void {
		const items = [...(this.value ?? [])];
		items.splice(event.detail.newIndex, 0, items.splice(event.detail.oldIndex, 1)[0]);
		this.#setValue(items);
	}

	override render() {
		return html`
			<contentment-display-mode-ui
				.addButtonLabelKey=${this.#createButtonLabelKey}
				?allowAdd=${!this.#readonly && (this.value?.length ?? 0) < this.#maxItems}
				?allowEdit=${!this.#readonly}
				?allowRemove=${!this.#readonly}
				?allowSort=${!this.#disableSorting && !this.#readonly}
				.config=${this.#config}
				.items=${this._items}
				.uiAlias=${this.#uiAlias}
				@add=${this.#onAdd}
				@edit=${this.#onEdit}
				@remove=${this.#onRemove}
				@sort=${this.#onSort}></contentment-display-mode-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIContentBlocksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-blocks': ContentmentPropertyEditorUIContentBlocksElement;
	}
}
