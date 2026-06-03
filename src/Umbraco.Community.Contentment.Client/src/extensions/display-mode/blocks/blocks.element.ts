// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, repeat, when } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentDisplayModeElement } from '../display-mode-base.element.js';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentSortEndEvent } from '../../../components/sortable-list/sort-end.event.js';

@customElement('contentment-display-mode-blocks')
export class ContentmentDisplayModeBlocksElement extends ContentmentDisplayModeElement {
	#onAdd(event: Event) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('add', { bubbles: true, detail: { listType: 'blocks' } }));
	}

	#onEdit(event: Event, item: ContentmentListItem, index: number) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('edit', { bubbles: true, detail: { item, index } }));
	}

	#onRemove(event: Event, item: ContentmentListItem, index: number) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('remove', { bubbles: true, detail: { item, index } }));
	}

	#onSort(event: ContentmentSortEndEvent) {
		event.stopPropagation();
		const { newIndex, oldIndex } = event;
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
		if (!this.items?.length) return nothing;
		return html`
			<contentment-sortable-list item-selector="uui-ref-node" ?disabled=${!this.allowSort} @sort-end=${this.#onSort}>
				${repeat(
					this.items,
					(item) => item.value,
					(item, index) => this.#renderItem(item, index),
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(item: ContentmentListItem, index: number) {
		if (!item) return nothing;
		return html`
			<uui-ref-node standalone @open=${(event: Event) => this.#onEdit(event, item, index)}>
				${when(item.icon, (icon) => html`<umb-icon slot="icon" .name=${icon}></umb-icon>`)}
				<umb-ufm-render slot="name" inline .markdown=${item.name} .value=${item.data}></umb-ufm-render>
				${when(
					item.description,
					(detail) =>
						html`<umb-ufm-render slot="detail" inline .markdown=${detail} .value=${item.data}></umb-ufm-render>`,
				)}
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
								`,
							)}
							${when(
								this.allowRemove,
								() => html`
									<uui-button
										label=${this.localize.term('general_remove')}
										@click=${(event: Event) => this.#onRemove(event, item, index)}></uui-button>
								`,
							)}
						</uui-action-bar>
					`,
				)}
			</uui-ref-node>
		`;
	}

	static override styles = [
		css`
			uui-ref-node {
				margin-bottom: 1px;
			}

			#btn-add {
				display: block;
			}
		`,
	];
}

export { ContentmentDisplayModeBlocksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-display-mode-blocks': ContentmentDisplayModeBlocksElement;
	}
}
