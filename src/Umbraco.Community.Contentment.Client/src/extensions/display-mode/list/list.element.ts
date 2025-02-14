// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, property, repeat, when } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, parseInt } from '../../../utils/index.js';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { CONTENTMENT_DISPLAY_MODE_CONTEXT } from '../display-mode.context-token.js';
import type { ContentmentDataListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentDisplayModeElement } from '../display-mode.extension.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

import '../../../components/sortable-list/sortable-list.element.js';

@customElement('contentment-display-mode-list')
export class ContentmentDisplayModeListElement extends UmbLitElement implements ContentmentDisplayModeElement {
	#confirmRemoval = false;

	#context?: typeof CONTENTMENT_DISPLAY_MODE_CONTEXT.TYPE;

	#defaultIcon?: string;

	#maxItems = Infinity;

	@property({ type: Boolean, attribute: 'allow-add' })
	allowAdd = false;

	@property({ type: Boolean, attribute: 'allow-edit' })
	allowEdit = false;

	@property({ type: Boolean, attribute: 'allow-remove' })
	allowRemove = false;

	@property({ type: Boolean, attribute: 'allow-sort' })
	allowSort = false;

	@property({ type: Array })
	public items?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon');
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
	}

	constructor() {
		super();

		this.consumeContext(CONTENTMENT_DISPLAY_MODE_CONTEXT, (context) => {
			this.#context = context;
			this.observe(context.items, (items) => {
				this.items = items.map((x) => x.unique);
			});
		});
	}

	#getMetadata(item: ContentmentDataListItem, key: string): string | unknown | undefined {
		return item[key];
	}

	#onChoose() {
		this.dispatchEvent(new CustomEvent('open', { bubbles: true, detail: 'list' }));
	}

	async #onRemove(item: ContentmentDataListItem, index: number) {
		if (!item || !this.items || index == -1) return;

		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: this.localize.term('contentment_removeItemHeadline'),
				content: this.localize.term('contentment_removeItemMessage'),
				confirmLabel: this.localize.term('contentment_removeItemButton'),
			});
		}

		const tmp = [...this.items];
		tmp.splice(index, 1);
		this.items = tmp;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSortEnd(event: CustomEvent<{ newIndex: number; oldIndex: number }>) {
		const items = [...(this.items ?? [])];
		items.splice(event.detail.newIndex, 0, items.splice(event.detail.oldIndex, 1)[0]);
		this.items = items;

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (!this.allowAdd) return nothing;
		if (this.items && this.items.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#onChoose}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.items) return;
		return html`
			<contentment-sortable-list
				class="uui-ref-list"
				item-selector="uui-ref-node"
				?disabled=${!this.allowSort}
				@sort-end=${this.#onSortEnd}>
				${repeat(
					this.items,
					(value) => value,
					(value, index) => this.#renderItem(value, index)
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(value: string, index: number) {
		const item = this.#context?.getItem(value);
		if (!item) return;
		const icon = this.#getMetadata(item, 'icon') ?? this.#defaultIcon;
		return html`
			<uui-ref-node
				name=${this.#getMetadata(item, 'name') ?? value}
				detail=${this.#getMetadata(item, 'description') ?? ''}
				?standalone=${this.#maxItems === 1}>
				${when(icon, (_icon) => html`<umb-icon slot="icon" name=${_icon}></umb-icon>`)}
				${when(
					this.allowRemove,
					() => html`
						<uui-action-bar slot="actions">
							<uui-button
								label=${this.localize.term('general_remove')}
								@click=${() => this.#onRemove(item, index)}></uui-button>
						</uui-action-bar>
					`
				)}
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

export { ContentmentDisplayModeListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-display-mode-list': ContentmentDisplayModeListElement;
	}
}
