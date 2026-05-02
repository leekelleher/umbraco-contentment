// SPDX-License-Identifier: MIT
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_LIQUID_CONTEXT } from '../../global-context/liquid/liquid.context.js';
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
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { Template } from '../../external/liquidjs.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-templated-list')
export class ContentmentPropertyEditorUITemplatedListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#liquid?: typeof CONTENTMENT_LIQUID_CONTEXT.TYPE;

	#template?: string;

	#templateCompiled?: Array<Template>;

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

	@state()
	private _markupLookup = new Map<string, unknown>();

	@state()
	private _parseError?: unknown;

	constructor() {
		super();
		this.consumeContext(CONTENTMENT_LIQUID_CONTEXT, (context) => {
			this.#liquid = context;
			this.#parseLiquidTemplate();
		});
	}

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

		this.#template = config.getValueByAlias<string>('template') ?? '{{ item.name }}';
		this.#parseLiquidTemplate();

		this._flexDirection = config.getValueByAlias('orientation') === 'horizontal' ? 'row' : 'column';

		this._listStyles = config.getValueByAlias('listStyles');
		this._listItemStyles = config.getValueByAlias('listItemStyles');

		const items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
		this._items = items.map((item) => ({ ...item, selected: this.value?.includes(item.value) ?? false }));

		this.#renderLiquidTemplate();

		if (!this.value) {
			this.value = this._enableMultiple && Array.isArray(defaultValue) ? defaultValue : [defaultValue];
		}
	}

	async #parseLiquidTemplate() {
		if (!this.#liquid || !this.#template) return;
		try {
			this.#templateCompiled = await this.#liquid.parse(this.#template);
			this._parseError = undefined;
		} catch (error) {
			console.error('[Contentment] Failed to parse Liquid template:', error);
			this._parseError = error;
			return;
		}
		this.#renderLiquidTemplate();
	}

	async #renderLiquidTemplate() {
		// Capture `#liquid` locally: on navigation away, the context consumer can
		// fire with `undefined` between awaits in the loop below, which would
		// otherwise crash with "Cannot read properties of undefined".
		const liquid = this.#liquid;

		if (!liquid || !this.#templateCompiled || !this._items?.length) return;

		for (const item of this._items) {
			try {
				const markup = await liquid.render(this.#templateCompiled, { item });
				this._markupLookup.set(item.value, markup ? unsafeHTML(markup) : nothing);
			} catch (error) {
				console.error('[Contentment] Failed to render Liquid template:', error);
				this._markupLookup.set(item.value, this.#renderError('render', error));
			}
		}

		this.requestUpdate('_markupLookup');
	}

	#renderError(stage: 'parse' | 'render', error: unknown) {
		const message = error instanceof Error ? error.message : String(error);
		const escaped = message.replace(/[&<>"']/g, (c) => `&#${c.charCodeAt(0)};`);
		return html`
			<contentment-info-box
				type="warning"
				icon="icon-alert"
				headline="Liquid template ${stage} error"
				message=${escaped}></contentment-info-box>
		`;
	}

	#onClick(option: ContentmentDataListOption) {
		option.selected = !option.selected;

		const values: Array<string> = [];

		this._items.forEach((item) => {
			if (!this._enableMultiple) {
				item.selected = option.selected && item.value === option.value;
			}

			if (item.selected) {
				values.push(item.value);
			}
		});

		this._items = [...this._items];
		this.value = values;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#updateItems() {
		if (this._items?.length) {
			this._items = this._items.map((item) => ({
				...item,
				selected: this.#value?.includes(item.value) ?? false,
			}));
			this.#renderLiquidTemplate();
		}
	}

	override render() {
		if (this._parseError) {
			return this.#renderError('parse', this._parseError);
		}

		if (!this._items?.length) {
			return html`
				<contentment-info-box
					compact
					type="warning"
					icon="icon-alert"
					headline="There are no items to display"></contentment-info-box>
			`;
		}

		return html`
			<ul class=${this._flexDirection} style=${ifDefined(this._listStyles)}>
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item),
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
					${this._markupLookup.get(item.value) ?? nothing}
				</button>
			</li>
		`;
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
