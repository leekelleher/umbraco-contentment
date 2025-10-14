// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_CONTENT_BLOCK_SELECTION_MODAL } from './content-block-selection-modal.element.js';
import { CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL } from './content-block-workspace-modal.element.js';
import type { ContentBlock, ContentBlockType } from './types.js';
import { css, customElement, html, ifDefined, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

@customElement('contentment-property-editor-ui-content-blocks')
export class ContentmentPropertyEditorUIContentBlocksElement extends UmbLitElement {
	@state()
	private _value: Array<ContentBlock> = [];

	@state()
	private _elementTypes: Array<ContentBlockType> = [];

	@state()
	private _elementTypeLookup: Record<string, ContentBlockType> = {};

	@state()
	private _maxItems = 0;

	@state()
	private _allowAdd = true;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	public set value(newValue: Array<ContentBlock> | undefined) {
		this._value = newValue ?? [];
		this.#updateAllowAdd();
	}

	public get value(): Array<ContentBlock> {
		return this._value;
	}

	public set config(config: any) {
		if (!config) return;

		// Parse element types from configuration
		const contentBlockTypes = config.getValueByAlias?.('contentBlockTypes') ?? [];
		this._elementTypes = contentBlockTypes.map((item: any) => ({
			key: item.key,
			alias: item.value?.elementType ?? '',
			name: item.value?.name ?? item.name,
			description: item.value?.description ?? item.description,
			icon: item.value?.icon ?? item.icon,
			nameTemplate: item.value?.nameTemplate,
			overlaySize: item.value?.overlaySize,
			previewEnabled: item.value?.previewEnabled ?? false,
		}));

		// Create lookup
		this._elementTypeLookup = Object.fromEntries(this._elementTypes.map((t) => [t.key, t]));

		// Get max items
		this._maxItems = parseInt(config.getValueByAlias?.('maxItems') ?? '0', 10);

		this.#updateAllowAdd();
	}

	#updateAllowAdd() {
		this._allowAdd = this._maxItems === 0 || this._value.length < this._maxItems;
	}

	async #onAdd() {
		if (!this.#modalManager) return;

		// If only one element type, skip selection modal
		if (this._elementTypes.length === 1) {
			const elementType = this._elementTypes[0];
			const newBlock: ContentBlock = {
				elementType: elementType.key,
				key: crypto.randomUUID(),
				value: {},
			};

			const modal = this.#modalManager.open(this, CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL, {
				data: { item: newBlock, elementType },
			});

			const data = await modal.onSubmit().catch(() => undefined);
			if (!data) return;

			this._value = [...this._value, data];
			this.#dispatchChangeEvent();
			return;
		}

		// Multiple element types - show selection modal
		const modal = this.#modalManager.open(this, CONTENTMENT_CONTENT_BLOCK_SELECTION_MODAL, {
			data: { elementTypes: this._elementTypes },
		});

		const data = await modal.onSubmit().catch(() => undefined);
		if (!data) return;

		this._value = [...this._value, data];
		this.#dispatchChangeEvent();
	}

	async #onEdit(block: ContentBlock, index: number) {
		if (!this.#modalManager) return;

		const elementType = this._elementTypeLookup[block.elementType];
		if (!elementType) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL, {
			data: { item: block, elementType },
		});

		const data = await modal.onSubmit().catch(() => undefined);
		if (!data) return;

		this._value = [...this._value.slice(0, index), data, ...this._value.slice(index + 1)];
		this.#dispatchChangeEvent();
	}

	#onDelete(index: number) {
		this._value = [...this._value.slice(0, index), ...this._value.slice(index + 1)];
		this.#dispatchChangeEvent();
	}

	#dispatchChangeEvent() {
		this.#updateAllowAdd();
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#getBlockName(block: ContentBlock, index: number): string {
		const elementType = this._elementTypeLookup[block.elementType];
		if (!elementType) return `Block ${index + 1}`;

		// TODO: Implement name template evaluation
		// For now, just return a simple name
		return elementType.nameTemplate
			? elementType.nameTemplate.replace('{{ $index }}', String(index + 1))
			: `${elementType.name} ${index + 1}`;
	}

	override render() {
		return html`
			${when(
				this._value.length > 0,
				() => html`
					<uui-ref-list>
						${repeat(
							this._value,
							(block) => block.key,
							(block, index) => this.#renderBlock(block, index)
						)}
					</uui-ref-list>
				`
			)}
			${when(
				this._allowAdd,
				() => html`
					<uui-button
						look="placeholder"
						label="Add content block"
						@click=${this.#onAdd}
						style="width: 100%; margin-top: var(--uui-size-space-3);">
						<uui-icon name="add"></uui-icon>
						Add content block
					</uui-button>
				`
			)}
		`;
	}

	#renderBlock(block: ContentBlock, index: number) {
		const elementType = this._elementTypeLookup[block.elementType];
		const name = this.#getBlockName(block, index);

		return html`
			<uui-ref-node name=${name} detail=${ifDefined(elementType?.description ?? undefined)}>
				<uui-icon slot="icon" name=${ifDefined(elementType?.icon ?? 'icon-document')}></uui-icon>
				<uui-action-bar slot="actions">
					<uui-button label="Edit" @click=${() => this.#onEdit(block, index)}>
						<uui-icon name="edit"></uui-icon>
					</uui-button>
					<uui-button label="Delete" @click=${() => this.#onDelete(index)}>
						<uui-icon name="delete"></uui-icon>
					</uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			:host {
				display: block;
			}

			uui-ref-list {
				margin-bottom: var(--uui-size-space-3);
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
