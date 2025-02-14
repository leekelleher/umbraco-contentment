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

@customElement('contentment-display-mode-cards')
export class ContentmentDisplayModeCardsElement extends UmbLitElement implements ContentmentDisplayModeElement {
	#confirmRemoval = false;

	#context?: typeof CONTENTMENT_DISPLAY_MODE_CONTEXT.TYPE;

	#defaultIcon: string = 'icon-document';

	#disableSorting = false;

	#maxItems = Infinity;

	@property({ type: Boolean, attribute: 'allow-add' })
	allowAdd = false;

	@property({ type: Boolean, attribute: 'allow-edit' })
	allowEdit = false;

	@property({ type: Boolean, attribute: 'allow-remove' })
	allowRemove = false;

	@property({ type: Array })
	public items?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon') ?? 'icon-document';
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1 ? true : parseBoolean(config.getValueByAlias('disableSorting'));
	}

	constructor() {
		super();

		this.consumeContext(CONTENTMENT_DISPLAY_MODE_CONTEXT, (context) => {
			this.#context = context;
			this.observe(context.items, (items) => {
				//console.log('cards.items', items);
				this.items = items.map((x) => x.unique);
			});
		});
	}

	#onChoose() {
		this.dispatchEvent(new CustomEvent('open', { bubbles: true, detail: 'cards' }));
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
		return html`
			<contentment-sortable-list
				class="container"
				?disabled=${this.#disableSorting}
				item-selector="uui-card-media"
				@sort-end=${this.#onSortEnd}>
				${this.#renderItems()} ${this.#renderAddButton()}
			</contentment-sortable-list>
		`;
	}

	#renderAddButton() {
		if (!this.allowAdd) return nothing;
		if (this.items && this.items.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#onChoose}>
				<uui-icon name="icon-add"></uui-icon>
				<umb-localize key="general_choose">Choose</umb-localize>
			</uui-button>
		`;
	}

	#renderItems() {
		if (!this.items) return;
		return html`
			${repeat(
				this.items,
				(value) => value,
				(value, index) => this.#renderItem(value, index)
			)}
		`;
	}

	#renderItem(value: string, index: number) {
		const item = this.#context?.getItem(value);
		if (!item) return;
		const icon = item.icon ?? this.#defaultIcon;
		return html`
			<uui-card-media name=${item.name ?? value} detail=${item.description ?? ''}>
				${when(
					item.image,
					() => html`<img src=${item.image!} alt="" />`,
					() => html`<umb-icon name=${icon}></umb-icon>`
				)}
				${when(
					this.allowRemove,
					() => html`
						<uui-action-bar slot="actions">
							<uui-button
								label=${this.localize.term('general_remove')}
								look="secondary"
								@click=${() => this.#onRemove(item, index)}>
								<uui-icon name="icon-trash"></uui-icon>
							</uui-button>
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
