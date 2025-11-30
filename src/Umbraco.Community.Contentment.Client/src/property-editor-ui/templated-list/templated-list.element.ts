// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentListItem, ContentmentDataListOption } from '../types.js';
import {
	classMap,
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	property,
	repeat,
	state,
	unsafeHTML,
	until,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { Liquid } from '../../external/liquidjs/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { Template } from '../../external/liquidjs/index.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-templated-list')
export class ContentmentPropertyEditorUITemplatedListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#engine = new Liquid({ cache: true });

	#template?: Array<Template>;

	@state()
	private _enableMultiple = false;

	@state()
	private _flexDirection: 'row' | 'column' = 'column';

	@state()
	private _items: Array<ContentmentDataListOption> = [];

	@state()
	private _listStyles?: string;

	@state()
	private _listItemStyles?: string;

	@property({ type: Array })
	public set value(value: Array<string> | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
		this.#updateItems();
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		const defaultValue = config.getValueByAlias('defaultValue') ?? [];
		this._enableMultiple = parseBoolean(config.getValueByAlias('enableMultiple'));

		const template = config.getValueByAlias<string>('template') ?? '{{ item.name }}';
		this.#template = this.#engine.parse(template);

		this._flexDirection = config.getValueByAlias('orientation') === 'horizontal' ? 'row' : 'column';

		this._listStyles = config.getValueByAlias('listStyles');
		this._listItemStyles = config.getValueByAlias('listItemStyles');

		const items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
		this._items = items.map((item) => ({ ...item, selected: this.value?.includes(item.value) ?? false }));

		if (!this.value) {
			this.value = this._enableMultiple && Array.isArray(defaultValue) ? defaultValue : [defaultValue];
		}
	}

	#onClick(option: ContentmentDataListOption) {
		option.selected = !option.selected;

		const values: Array<string> = [];

		this._items.forEach((item) => {
			if (!this._enableMultiple) {
				item.selected = option.selected && item.value === option.value;
			}

			if (item.selected) {
				values?.push(item.value);
			}
		});

		this.value = values;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#updateItems() {
		if (this._items?.length) {
			this._items.forEach((item) => {
				item.selected = this.#value?.includes(item.value) ?? false;
			});
		}
	}

	override render() {
		if (!this._items?.length) {
			return html`
				<contentment-info-box
					compact
					type="warning"
					icon="icon-alert"
					heading="There are no items to display"></contentment-info-box>
			`;
		}

		return html`
			<ul class=${this._flexDirection} style=${ifDefined(this._listStyles)}>
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item)
				)}
			</ul>
		`;
	}

	#renderItem(item: ContentmentDataListOption) {
		if (!item) return nothing;
		return html`
			<li class=${classMap({ selected: item.selected })} style=${ifDefined(this._listItemStyles)}>
				<button
					class=${classMap({ selected: item.selected })}
					?disabled=${item.disabled}
					@click=${() => this.#onClick(item)}>
					${until(this.#renderTemplate(item))}
				</button>
			</li>
		`;
	}

	async #renderTemplate(item: ContentmentDataListOption) {
		if (!this.#engine || !this.#template) return null;
		const markup = await this.#engine.render(this.#template, { item });
		return markup ? unsafeHTML(markup) : nothing;
	}

	static override styles = [
		css`
			ul {
				display: flex;
				flex-direction: column;
				flex-wrap: wrap;
				gap: var(--uui-size-3);

				list-style: none;
				padding: 0;
				margin: 0;

				&.row {
					flex-direction: row;
				}

				> li {
					flex: 1;
					border-radius: calc(var(--uui-border-radius) * 2);

					&.selected {
						outline: var(--uui-size-2) solid var(--uui-color-selected);
					}

					> button {
						all: initial;
						font: inherit;
						color: inherit;

						border-radius: calc(var(--uui-border-radius) * 2);
						cursor: pointer;
						display: flex;
						width: 100%;

						> * {
							flex: 1;
							pointer-events: none;
						}

						&[disabled] {
							cursor: not-allowed !important;
							opacity: 0.5;
						}

						&:focus-visible {
							outline: 2px solid var(--uui-color-focus);
						}
					}
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUITemplatedListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-templated-list': ContentmentPropertyEditorUITemplatedListElement;
	}
}
