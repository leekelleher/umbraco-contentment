// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { ContentmentListItem } from '../types.js';
import {
	css,
	customElement,
	html,
	nothing,
	property,
	repeat,
	state,
	unsafeHTML,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIComboboxElement } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-tags')
export class ContentmentPropertyEditorUITagsElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	//#allowDuplicates = false;

	#confirmRemoval = false;

	#items: Array<ContentmentListItem> = [];

	#lookup: Record<string, ContentmentListItem> = {};

	#showIcons = false;

	@state()
	private _loading = false;

	@state()
	private _options: Array<ContentmentListItem> = [];

	@state()
	private _query?: string;

	@property({ type: Boolean, reflect: true })
	readonly = false;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		//this.#allowDuplicates = parseBoolean(config.getValueByAlias('allowDuplicates'));
		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#showIcons = parseBoolean(config.getValueByAlias('showIcons'));

		this.#items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];

		// populate item lookup
		if (!this.#items) return;
		this.#items.forEach((item) => {
			this.#lookup[item.value] = item;
		});
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.#items?.length) return;
		query = (query || '').toLocaleLowerCase();
		this._options = query ? this.#items.filter((item) => this.#predicate(query, item)) : [];
		this._loading = false;
	}, 100);

	#predicate = (query: string, item: ContentmentListItem) =>
		item.name.toLocaleLowerCase().includes(query) ||
		item.value.toLocaleLowerCase().includes(query) ||
		item.description?.toLocaleLowerCase().includes(query);

	#getItemByValue(value: string): ContentmentListItem | undefined {
		return this.#lookup[value];
	}

	#getMetadata<T = string>(item: ContentmentListItem, key: string): T | undefined {
		return item[key] as T;
	}

	async #onRemove(item: ContentmentListItem, index: number) {
		if (!item || !this.value || index == -1) return;

		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: this.localize.term('contentment_removeItemHeadline', [item.name]),
				content: this.localize.term('contentment_removeItemMessage'),
				confirmLabel: this.localize.term('contentment_removeItemButton'),
			});
		}

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSearch(event: CustomEvent & { target: UUIComboboxElement }) {
		this._loading = true;
		this._query = event.target.search;
		this.#debouncedFilter(this._query);
	}

	#onSelect(event: CustomEvent & { target: UUIComboboxElement }) {
		if (event.target.nodeName !== 'UUI-COMBOBOX') return;

		const value = event.target.value as string;
		if (!value) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp.push(value);
		this.value = tmp;

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onTagKeydown(event: KeyboardEvent, item: ContentmentListItem, index: number) {
		if (event.key === 'Backspace' || event.key === 'Delete') {
			event.preventDefault();
			await this.#onRemove(item, index);
		}
	}

	override render() {
		if (!this.#items?.length) {
			return html`
				<contentment-info-box
					compact
					type="warning"
					icon="icon-alert"
					headline="There are no items to choose from."></contentment-info-box>
			`;
		}

		return html`<div id="container">${this.#renderTags()}${this.#renderInput()}</div>`;
	}

	#renderTags() {
		if (!this.value?.length) return nothing;
		return html`
			<div id="tags">
				${repeat(
					this.value,
					(value) => value,
					(value, index) => this.#renderTag(value, index)
				)}
			</div>
		`;
	}

	#renderTag(value: string, index: number) {
		const item = this.#getItemByValue(value);
		if (!item) return nothing;
		const icon = this.#getMetadata(item, 'icon');
		const name = this.localize.string(this.#getMetadata(item, 'name') ?? value);
		return html`
			<div class="tag" tabindex="0" @keydown=${(event: KeyboardEvent) => this.#onTagKeydown(event, item, index)}>
				${when(this.#showIcons && icon, (_icon) => html`<umb-icon name=${_icon}></umb-icon>`)}
				<strong>${name}</strong>
				${when(
					!this.readonly,
					() => html`
						<uui-button
							compact
							class="action"
							label=${this.localize.term('general_remove')}
							tabindex="-1"
							@click=${() => this.#onRemove(item, index)}>
							<umb-icon name="icon-trash"></umb-icon>
						</uui-button>
					`
				)}
			</div>
		`;
	}

	#renderInput() {
		if (this.readonly) return nothing;
		return html`
			<uui-combobox id="input" placeholder="Enter a tag..." @change=${this.#onSelect} @search=${this.#onSearch}>
				${when(this._loading, () => html`<uui-loader id="loader"></uui-loader>`)}
				${when(
					!this._loading && !this._query,
					() => html`<div class="info">${this.localize.term('placeholders_search')}</div>`
				)}
				${when(
					!this._loading && this._query && this._options.length === 0,
					() => html`<div class="info">${this.localize.term('general_searchNoResult')}</div>`
				)}
				${when(
					!this._loading && this._options?.length,
					() => html`
						<uui-combobox-list>
							${repeat(
								this._options,
								(option) => option.value,
								(option) => this.#renderOption(option)
							)}
						</uui-combobox-list>
					`
				)}
			</uui-combobox>
		`;
	}

	#renderOption(option: ContentmentListItem) {
		return html`
			<uui-combobox-list-option
				class="option"
				display-value=${this.localize.string(option.name)}
				value=${option.value}
				?disabled=${option.disabled}>
				<div class="outer">
					${when(this.#showIcons && option.icon, (_icon) => html`<umb-icon name=${_icon}></umb-icon>`)}
					${when(
						option.description,
						() => html`
							<uui-form-layout-item>
								<span slot="label">${this.localize.string(option.name)}</span>
								<span slot="description">${unsafeHTML(option.description)}</span>
							</uui-form-layout-item>
						`,
						() => html`<span>${this.localize.string(option.name)}</span>`
					)}
				</div>
			</uui-combobox-list-option>
		`;
	}

	static override styles = [
		css`
			:host {
				--uui-focus-outline-color: var(--uui-color-focus);
				--uui-tag-border-radius: calc(var(--uui-size-4) * 2);
			}

			#container {
				box-sizing: border-box;
				display: flex;
				flex-wrap: wrap;
				align-items: center;
				gap: var(--uui-size-space-2);

				padding: var(--uui-size-space-2);

				background-color: var(--uui-input-background-color, var(--uui-color-surface));
				border: 1px solid var(--uui-color-border);
				border-radius: var(--uui-border-radius);
			}

			#tags {
				display: flex;
				gap: var(--uui-size-space-2);
				flex-wrap: wrap;
				border-radius: var(--uui-size-1);

				&:focus {
					outline: var(--uui-size-1) solid var(--uui-focus-outline-color);
					outline-offset: var(--uui-size-1);
				}
			}

			.tag {
				--uui-border-radius: var(--uui-tag-border-radius);

				display: flex;
				align-items: center;
				justify-content: space-between;

				padding: var(--uui-size-space-1) var(--uui-size-space-3);
				line-height: normal;

				background-color: var(--uui-color-surface-alt);
				border-radius: var(--uui-border-radius);
				color: var(--color-standalone);

				&:focus {
					outline: var(--uui-size-1) solid var(--uui-focus-outline-color);
				}

				strong {
					font-size: var(--uui-type-small-size);
					margin: 0 var(--uui-size-space-2);
				}
			}

			.action {
				--uui-button-height: var(--uui-size-9);
				--uui-button-padding-left-factor: 0;
				--uui-button-padding-right-factor: 0;
				--uui-button-padding-top-factor: 0;
				--uui-button-padding-bottom-factor: 0;

				font-size: var(--uui-type-small-size);
			}

			#input {
				/* HACK: 'uui-combobox' uses '--uui-size-1' for the border-radius. [LK]
				 * https://github.com/umbraco/Umbraco.UI/blob/v1.16.0/packages/uui-combobox/lib/uui-combobox.element.ts#L420
				 */
				--uui-size-1: var(--uui-tag-border-radius);
			}

			#loader {
				display: flex;
				align-items: center;
				justify-content: center;
				min-height: var(--uui-size-layout-3);
			}

			.info {
				padding: var(--uui-size-4);
			}

			.option {
				padding: 0.5rem;

				.outer {
					display: flex;
					flex-direction: row;
					align-items: center;
					gap: 0.5rem;

					uui-form-layout-item {
						margin-top: 3px;
						margin-bottom: 0;
					}
				}
			}
		`,
	];
}

export default ContentmentPropertyEditorUITagsElement;

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-tags': ContentmentPropertyEditorUITagsElement;
	}
}
