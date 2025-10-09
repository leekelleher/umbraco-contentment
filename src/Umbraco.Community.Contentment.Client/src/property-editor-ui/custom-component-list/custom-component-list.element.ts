// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import type { ContentmentListItem, ContentmentDataListOption } from '../types.js';
import type { ContentmentDataListItemUiElement } from '../../extensions/types.js';
import {
	classMap,
	customElement,
	html,
	ifDefined,
	nothing,
	property,
	repeat,
	state,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { ManifestBase } from '@umbraco-cms/backoffice/extension-api';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { ContentmentPropertyEditorUITemplatedListElement } from '../templated-list/templated-list.element.js';

@customElement('contentment-property-editor-ui-custom-component-list')
export default class ContentmentPropertyEditorUICustomComponentListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#component?: string;

	@state()
	private _element?: ContentmentDataListItemUiElement;

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

		const components = config.getValueByAlias<Array<string>>('component') ?? [];
		this.#component = components[0];

		this._flexDirection = config.getValueByAlias('orientation') === 'horizontal' ? 'row' : 'column';

		this._listStyles = config.getValueByAlias('listStyles');
		this._listItemStyles = config.getValueByAlias('listItemStyles');

		const items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
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
		if (!this._element) return html`<lee-was-here></lee-was-here>`;

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
		if (!this._element) return nothing;
		const element = this._element.cloneNode(true) as ContentmentDataListItemUiElement;
		element.item = item;
		return html`
			<li class=${classMap({ selected: item.selected })} style=${ifDefined(this._listItemStyles)}>
				<button ?disabled=${item.disabled} @click=${() => this.#onClick(item)}>${element}</button>
			</li>
		`;
	}

	static override styles = ContentmentPropertyEditorUITemplatedListElement.styles;
}

export { ContentmentPropertyEditorUICustomComponentListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-custom-component-list': ContentmentPropertyEditorUICustomComponentListElement;
	}
}
