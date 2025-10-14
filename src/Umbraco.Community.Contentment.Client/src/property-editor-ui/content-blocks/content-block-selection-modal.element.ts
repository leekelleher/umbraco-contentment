// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL } from './content-block-workspace-modal.element.js';
import type { ContentBlockType, ContentBlock } from './types.js';
import {
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	repeat,
	state,
} from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalBaseElement, UmbModalToken, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import type { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

interface ContentBlockSelectionModalData {
	elementTypes: Array<ContentBlockType>;
}

export const CONTENTMENT_CONTENT_BLOCK_SELECTION_MODAL = new UmbModalToken<
	ContentBlockSelectionModalData,
	ContentBlock
>('Umb.Contentment.Modal.ContentBlock.Selection', {
	modal: {
		type: 'sidebar',
		size: 'small',
	},
});

@customElement('contentment-property-editor-ui-content-block-selection-modal')
export class ContentmentPropertyEditorUIContentBlockSelectionModalElement extends UmbModalBaseElement<
	ContentBlockSelectionModalData,
	ContentBlock
> {
	@state()
	private _filteredTypes?: Array<ContentBlockType>;

	@state()
	private _hideFilter = false;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	override connectedCallback() {
		super.connectedCallback();
		this._filteredTypes = this.data?.elementTypes;
		this._hideFilter = (this.data?.elementTypes.length ?? 0) <= 7;
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		const types = query
			? this.data.elementTypes.filter((type) => type.name.toLowerCase().includes(query.toLowerCase()))
			: this.data.elementTypes;
		this._filteredTypes = types;
	}, 500);

	async #onChoose(elementType: ContentBlockType) {
		// Create a new block instance
		const newBlock: ContentBlock = {
			elementType: elementType.key,
			key: crypto.randomUUID(),
			value: {},
		};

		if (!this.#modalManager) {
			// If no modal manager, just return the empty block
			this.value = newBlock;
			this._submitModal();
			return;
		}

		// Open workspace modal to edit the block
		const modal = this.#modalManager.open(this, CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL, {
			data: { item: newBlock, elementType },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		if (!data) return;

		this.value = data;
		this._submitModal();
	}

	#onInput(event: UUIInputEvent) {
		const query = (event.target.value as string) || '';
		this.#debouncedFilter(query);
	}

	override render() {
		return html`
			<umb-body-layout headline="Choose element type">
				${this.#renderFilter()} ${this.#renderTypes()}
				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderFilter() {
		if (this._hideFilter) return nothing;
		const label = this.localize.term('placeholders_filter');
		return html`
			<uui-input type="search" id="filter" label=${label} placeholder=${label} @input=${this.#onInput} ${umbFocus()}>
				<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
			</uui-input>
		`;
	}

	#renderTypes() {
		if (!this._filteredTypes?.length) return html`<uui-box><p>No element types found</p></uui-box>`;
		return html`
			<uui-box>
				<uui-ref-list>
					${repeat(
						this._filteredTypes,
						(type) => type.key,
						(type) => html`
							<umb-ref-item
								name=${type.name}
								detail=${ifDefined(type.description ?? undefined)}
								icon=${ifDefined(type.icon ?? 'icon-document')}
								@open=${() => this.#onChoose(type)}>
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
		`,
	];
}

export { ContentmentPropertyEditorUIContentBlockSelectionModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-block-selection-modal': ContentmentPropertyEditorUIContentBlockSelectionModalElement;
	}
}
