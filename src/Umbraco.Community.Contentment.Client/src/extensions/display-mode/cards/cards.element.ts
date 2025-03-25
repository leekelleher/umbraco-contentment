// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, repeat, styleMap, when } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentDisplayModeElement } from '../display-mode-base.element.js';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { SortableEvent } from '../../../external/sortablejs/index.js';
import type { StyleInfo } from '@umbraco-cms/backoffice/external/lit';

import '../../../components/sortable-list/sortable-list.element.js';

@customElement('contentment-display-mode-cards')
export class ContentmentDisplayModeCardsElement extends ContentmentDisplayModeElement {
	#defaultIcon: string = 'icon-document';

	#onAdd(event: Event) {
		event.stopPropagation();
		this.dispatchEvent(new CustomEvent('add', { bubbles: true, detail: { listType: 'cards' } }));
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
		return html`
			<contentment-sortable-list
				class="container"
				?disabled=${!this.allowSort}
				item-selector="uui-card-media"
				@sort-end=${this.#onSort}>
				${this.#renderItems()} ${this.#renderAddButton()}
			</contentment-sortable-list>
		`;
	}

	#renderAddButton() {
		if (!this.allowAdd) return nothing;
		const label = this.localize.term(this.addButtonLabelKey ?? 'general_choose');
		return html`
			<uui-button id="btn-add" label=${label} look="placeholder" @click=${this.#onAdd}>
				<uui-icon name="icon-add"></uui-icon>
				<span>${label}</span>
			</uui-button>
		`;
	}

	#renderItems() {
		if (!this.items) return;
		return html`
			${repeat(
				this.items,
				(item) => item.value,
				(item, index) => this.#renderItem(item, index)
			)}
		`;
	}

	#renderItem(item: ContentmentListItem, index: number) {
		if (!item) return;
		const cardStyle = (item.cardStyle as StyleInfo) ?? {};
		const iconStyle = (item.iconStyle as StyleInfo) ?? {};
		return html`
			<uui-card-media
				name=${item.name}
				detail=${item.description ?? ''}
				style=${styleMap(cardStyle)}
				@open=${(event: Event) => this.#onEdit(event, item, index)}>
				${when(
					item.image,
					() => html`<img src=${item.image!} alt="" />`,
					() => html`<umb-icon name=${item.icon ?? this.#defaultIcon} style=${styleMap(iconStyle)}></umb-icon>`
				)}
				${when(
					this.allowEdit || this.allowRemove,
					() => html`
						<uui-action-bar slot="actions">
							${when(
								this.allowEdit,
								() => html`
									<uui-button
										label=${this.localize.term('general_edit')}
										look="secondary"
										@click=${(event: Event) => this.#onEdit(event, item, index)}>
										<uui-icon name="icon-edit"></uui-icon>
									</uui-button>
								`
							)}
							${when(
								this.allowRemove,
								() => html`
									<uui-button
										label=${this.localize.term('general_remove')}
										look="secondary"
										@click=${(event: Event) => this.#onRemove(event, item, index)}>
										<uui-icon name="icon-trash"></uui-icon>
									</uui-button>
								`
							)}
						</uui-action-bar>
					`
				)}
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

			uui-card-media {
				&[drag-placeholder] {
					opacity: 0.2;
				}

				umb-icon {
					font-size: var(--uui-size-8);
					/* HACK: To make the icon position appear vertically centred within the top half of the card. [LK] */
					padding-bottom: var(--uui-size-12);
				}
			}

			img {
				background-image: url('data:image/svg+xml;charset=utf-8,<svg xmlns="http://www.w3.org/2000/svg" width="100" height="100" fill-opacity=".1"><path d="M50 0h50v50H50zM0 50h50v50H0z"/></svg>');
				background-size: 10px 10px;
				background-repeat: repeat;
			}
		`,
	];
}

export { ContentmentDisplayModeCardsElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-display-mode-cards': ContentmentDisplayModeCardsElement;
	}
}
