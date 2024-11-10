// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataListItem, ContentmentDataListOption } from '../types.js';
import type { ContentmentDataListItemUiElement } from '../../extensions/index.js';
import {
	classMap,
	css,
	customElement,
	html,
	nothing,
	property,
	repeat,
	state,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { createExtensionElement, ManifestBase } from '@umbraco-cms/backoffice/extension-api';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

import '../../components/lee-was-here/lee-was-here.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-templated-list';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUITemplatedListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#component?: string;

	@state()
	private _element?: ContentmentDataListItemUiElement;

	@state()
	private _enableMultiple = false;

	@state()
	private _items: Array<ContentmentDataListOption> = [];

	@property({ type: Array })
	public set value(value: Array<string> | undefined) {
		this.#value = value ?? [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const defaultValue = config.getValueByAlias('defaultValue') ?? [];
		this._enableMultiple = parseBoolean(config.getValueByAlias('enableMultiple'));

		const components = config.getValueByAlias<Array<string>>('component') ?? [];
		this.#component = components[0];

		const items = config.getValueByAlias<Array<ContentmentDataListItem>>('items') ?? [];
		this._items = items.map((item) => ({ ...item, selected: this.value?.includes(item.value) ?? false }));

		if (!this.value) {
			this.value = this._enableMultiple && Array.isArray(defaultValue) ? defaultValue : [defaultValue];
		}

		this.#observeComponent();
	}

	#observeComponent() {
		if (this.#component) {
			this.observe(
				umbExtensionsRegistry.byTypeAndAlias('contentmentDataListItemUi', this.#component),
				(manifest) => {
					if (manifest) {
						this.#getComponent(manifest);
					} else {
						console.error(`Failed to find manifest for Contentment Data List Item UI alias: ${this.#component}`);
					}
				},
				'_observeComponent'
			);
		}
	}

	async #getComponent(manifest?: ManifestBase | null) {
		if (!manifest) return;

		const element = await createExtensionElement(manifest);

		if (!element) {
			console.error(`Failed to create extension element for manifest: ${manifest}`);
		}

		this._element = element;
	}

	#onClick(option: ContentmentDataListOption) {
		option.selected = !option.selected;

		const values: Array<string> = [];

		this._items.forEach((item) => {
			if (!this._enableMultiple) {
				item.selected = item.value === option.value;
			}

			if (item.selected) {
				values?.push(item.value);
			}
		});

		this.value = values;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		if (!this._element) return html`<lee-was-here></lee-was-here>`;
		return html`
			<ul>
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item)
				)}
			</ul>
		`;
	}

	#renderItem(item: ContentmentDataListOption) {
		if (!this._element) return nothing;
		const element = this._element.cloneNode(true) as ContentmentDataListItemUiElement;
		element.item = item;
		return html`
			<li>
				<button
					class=${classMap({ selected: item.selected })}
					?disabled=${item.disabled}
					@click=${() => this.#onClick(item)}>
					${element}
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
				gap: var(--uui-size-2);

				list-style: none;
				padding: 0;
				margin: 0;

				> li {
					flex: 1;

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
						}

						&[disabled] {
							cursor: not-allowed !important;
							opacity: 0.5;
						}

						&.selected {
							background-color: var(--uui-menu-item-background-color-active, var(--uui-color-current, #f5c1bc));
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
		[ELEMENT_NAME]: ContentmentPropertyEditorUITemplatedListElement;
	}
}
