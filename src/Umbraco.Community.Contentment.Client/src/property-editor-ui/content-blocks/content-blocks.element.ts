// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, repeat } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbDocumentTypeItemRepository } from '@umbraco-cms/backoffice/document-type';
import { UmbFormControlMixin } from '@umbraco-cms/backoffice/validation';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';
import { CONTENTMENT_ITEM_PICKER_MODAL } from '../item-picker/item-picker-modal.element.js';
import { CONTENTMENT_ELEMENT_WORKSPACE_MODAL } from '../../workspace/element/index.js';
import type { ContentmentConfigurationEditorValue, ContentmentContentBlockValue } from '../types.js';
import type { ContentmentContentBlockRefElement } from './content-block-ref.element.js';
import type { UmbDocumentTypeItemModel } from '@umbraco-cms/backoffice/document-type';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import './content-block-ref.element.js';

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
	readonly #sorter = new UmbSorterController<ContentmentContentBlockValue, ContentmentContentBlockRefElement>(this, {
		getUniqueOfElement: (element) => element.item?.key ?? '',
		getUniqueOfModel: (model) => model.key,
		itemSelector: 'contentment-content-block-ref',
		onChange: ({ model }) => this.#setValue(model),
	});

	#createButtonLabelKey = '#content_createEmpty';
	#disableSorting = false;
	#maxItems = Infinity;
	#blockTypeConfig = new Map<string, ContentBlockTypeConfig>();
	#docTypeItems = new Map<string, UmbDocumentTypeItemModel>();
	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	constructor() {
		super();
		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (ctx) => (this.#modalManager = ctx));
	}

	public override set value(value: Array<ContentmentContentBlockValue> | undefined) {
		super.value = value;
		this.#sorter.setModel(value ?? []);
	}
	public override get value(): Array<ContentmentContentBlockValue> | undefined {
		return super.value;
	}

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#createButtonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? '#content_createEmpty';
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1 ? true : parseBoolean(config.getValueByAlias('disableSorting'));

		if (this.#disableSorting || this.#readonly) {
			this.#sorter.disable();
		}

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
			this.requestUpdate();
		}
	}

	public set readonly(value: boolean) {
		this.#readonly = value;

		if (this.#readonly || this.#disableSorting) {
			this.#sorter.disable();
		} else {
			this.#sorter.enable();
		}
	}
	public get readonly(): boolean {
		return this.#readonly;
	}
	#readonly = false;

	#setValue(items: Array<ContentmentContentBlockValue>): void {
		this.value = items.length ? items : undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onCreate(): Promise<void> {
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
					enableFilter: items.length > 6,
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

	async #onEdit(item: ContentmentContentBlockValue): Promise<void> {
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

	async #onDelete(item: ContentmentContentBlockValue): Promise<void> {
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

	override render() {
		return html`
			${repeat(
				this.value ?? [],
				(item) => item.key,
				(item, index) => {
					const dt = this.#docTypeItems.get(item.elementType);
					const cfg = this.#blockTypeConfig.get(item.elementType);
					// Mark unsupported once items have loaded and this elementType isn't among them
					const unsupported = this.#docTypeItems.size > 0 && (!dt || dt.isElement === false);
					return html`
						<contentment-content-block-ref
							.item=${item}
							.index=${index}
							.name=${dt?.name ?? item.elementType}
							.icon=${dt?.icon ?? 'icon-document'}
							.nameTemplate=${cfg?.nameTemplate ?? ''}
							.unsupported=${unsupported}
							.readonly=${this.#readonly}
							@edit=${() => this.#onEdit(item)}
							@delete=${() => this.#onDelete(item)}></contentment-content-block-ref>
					`;
				},
			)}
			${this.#renderCreateButtonGroup()}
		`;
	}

	#renderCreateButtonGroup() {
		if (this.readonly || (this.value?.length ?? 0) >= this.#maxItems) return nothing;
		return html`
			<uui-button-group>
				<uui-button
					look="placeholder"
					label=${this.localize.string(this.#createButtonLabelKey)}
					@click=${this.#onCreate}></uui-button>
			</uui-button-group>
		`;
	}

	static override readonly styles = [
		css`
			:host {
				display: grid;
				gap: 1px;
			}

			uui-button-group {
				padding-top: 1px;
				display: grid;
				grid-template-columns: 1fr auto;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIContentBlocksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-blocks': ContentmentPropertyEditorUIContentBlocksElement;
	}
}
