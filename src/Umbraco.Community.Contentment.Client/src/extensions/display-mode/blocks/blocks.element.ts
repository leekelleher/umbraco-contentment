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

	#onInsert(event: Event, index: number) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('insert', { bubbles: true, detail: { index, listType: 'blocks' } }));
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
			<contentment-sortable-list item-selector=".item" ?disabled=${!this.allowSort} @sort-end=${this.#onSort}>
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
			<div class="item">
				${when(
					this.allowAdd,
					() => html`
						<uui-button-inline-create
							label=${this.localize.term(this.addButtonLabelKey ?? 'general_choose')}
							@click=${(event: Event) => this.#onInsert(event, index)}>
						</uui-button-inline-create>
					`,
				)}
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
							<uui-action-bar slot="actions" class="actions">
								${when(
									this.allowEdit || this.canEdit(item, index),
									() => html`
										<uui-button
											look="secondary"
											label=${this.localize.term('general_edit')}
											title=${this.localize.term('general_edit')}
											@click=${(event: Event) => this.#onEdit(event, item, index)}>
											<uui-icon name="icon-edit"></uui-icon>
										</uui-button>
									`,
								)}
								${when(
									this.allowRemove,
									() => html`
										<uui-button
											look="secondary"
											label=${this.localize.term('general_remove')}
											title=${this.localize.term('general_remove')}
											@click=${(event: Event) => this.#onRemove(event, item, index)}>
											<uui-icon name="icon-remove"></uui-icon>
										</uui-button>
									`,
								)}
							</uui-action-bar>
						`,
					)}
				</uui-ref-node>
			</div>
		`;
	}

	static override styles = [
		css`
			#btn-add {
				--uui-button-padding-top-factor: 1.5;
				--uui-button-padding-bottom-factor: 1.5;
				display: block;
			}

			.item {
				--contentment-block-actions-opacity: 0;
				margin-bottom: 1px;

				&:hover,
				&:focus-within {
					--contentment-block-actions-opacity: 1;
				}

				.actions {
					position: absolute;
					top: var(--uui-size-2);
					right: var(--uui-size-2);
					opacity: var(--contentment-block-actions-opacity, 0);
					transition: opacity 120ms;
					z-index: 1;
				}
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
