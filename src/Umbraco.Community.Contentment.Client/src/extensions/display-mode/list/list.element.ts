// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { parseInt } from '../../../utils/index.js';
import { css, customElement, html, nothing, repeat, when } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentDisplayModeElement } from '../display-mode-base.element.js';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { SortableEvent } from '../../../external/sortablejs/index.js';

@customElement('contentment-display-mode-list')
export class ContentmentDisplayModeListElement extends ContentmentDisplayModeElement {
	#defaultIcon?: string;

	#maxItems = Infinity;

	override connectedCallback() {
		super.connectedCallback();

		this.#defaultIcon = this.getConfigByAlias<string>('defaultIcon');
		this.#maxItems = parseInt(this.getConfigByAlias('maxItems')) || Infinity;
	}

	#onAdd(event: Event) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('add', { bubbles: true, detail: { listType: 'list' } }));
	}

	#onEdit(event: Event, item: ContentmentListItem, index: number) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('edit', { bubbles: true, detail: { item, index } }));
	}

	#onRemove(event: Event, item: ContentmentListItem, index: number) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('remove', { bubbles: true, detail: { item, index } }));
	}

	#onSort(event: CustomEvent<SortableEvent>) {
		event.stopPropagation();
		const { newIndex, oldIndex } = event.detail;
		this.dispatchEvent(new CustomEvent('sort', { bubbles: true, detail: { newIndex, oldIndex } }));
	}

	override render() {
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (!this.allowAdd) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term(this.addButtonLabelKey ?? 'general_choose')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.items) return;
		return html`
			<contentment-sortable-list
				class="uui-ref-list"
				item-selector="uui-ref-node"
				?disabled=${!this.allowSort}
				@sort-end=${this.#onSort}>
				${repeat(
					this.items,
					(item) => item.value,
					(item, index) => this.#renderItem(item, index)
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(item: ContentmentListItem, index: number) {
		if (!item) return;
		return html`
			<uui-ref-node
				name=${item.name}
				detail=${item.description ?? ''}
				?standalone=${this.items?.length === 1 && this.#maxItems === 1}
				@open=${(event: Event) => this.#onEdit(event, item, index)}>
				${when(item.icon ?? this.#defaultIcon, (_icon) => html`<umb-icon slot="icon" name=${_icon}></umb-icon>`)}
				${when(
					this.allowEdit || this.allowRemove,
					() => html`
						<uui-action-bar slot="actions">
							${when(
								this.allowEdit || this.canEdit(item, index),
								() => html`
									<uui-button
										label=${this.localize.term('general_edit')}
										@click=${(event: Event) => this.#onEdit(event, item, index)}></uui-button>
								`
							)}
							${when(
								this.allowRemove,
								() => html`
									<uui-button
										label=${this.localize.term('general_remove')}
										@click=${(event: Event) => this.#onRemove(event, item, index)}></uui-button>
								`
							)}
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
