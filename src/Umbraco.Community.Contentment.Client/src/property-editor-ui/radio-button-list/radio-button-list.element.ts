// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import {
	css,
	customElement,
	html,
	property,
	repeat,
	state,
	unsafeHTML,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { ContentmentListItem } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIRadioEvent } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-radio-button-list')
export class ContentmentPropertyEditorUIRadioButtonListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@state()
	private _flexDirection: 'row' | 'column' = 'column';

	@property()
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) === true ? value[0] : value ?? '';
	}
	public get value(): string | undefined {
		return this.#value;
	}
	#value?: string = '';

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this._defaultValue = config.getValueByAlias<string>('defaultValue') ?? '';
		this._items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
		this._showDescriptions = parseBoolean(config.getValueByAlias('showDescriptions'));
		this._showIcons = parseBoolean(config.getValueByAlias('showIcons'));

		this._flexDirection = config.getValueByAlias('orientation') === 'horizontal' ? 'row' : 'column';
	}

	@state()
	private _defaultValue = '';

	@state()
	private _items: Array<ContentmentListItem> = [];

	@state()
	private _showDescriptions = false;

	@state()
	private _showIcons = false;

	#onChange(event: UUIRadioEvent) {
		if (event.target.nodeName !== 'UUI-RADIO') return;
		this.value = event.target.value;
		this.dispatchEvent(new UmbChangeEvent());
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
			<uui-radio-group
				class=${this._flexDirection}
				.value=${this.value || this._defaultValue}
				@change=${this.#onChange}>
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item)
				)}
			</uui-radio-group>
		`;
	}

	#renderItem(item: ContentmentListItem) {
		return html`
			<uui-radio value=${item.value} ?disabled=${item.disabled}>
				<div class="outer">
					${when(this._showIcons && item.icon, (_icon) => html`<umb-icon name=${_icon}></umb-icon>`)}
					<uui-form-layout-item>
						<span slot="label">${this.localize.string(item.name)}</span>
						${when(
							this._showDescriptions && item.description,
							() => html`<span slot="description">${unsafeHTML(item.description)}</span>`
						)}
					</uui-form-layout-item>
				</div>
			</uui-radio>
		`;
	}

	static override styles = [
		css`
			.row {
				display: flex;
				flex-direction: row;
				flex-wrap: wrap;
				gap: 1rem;
			}

			.outer {
				display: flex;
				flex-direction: row;
				gap: 0.5rem;
			}

			uui-form-layout-item {
				margin-top: 10px;
				margin-bottom: 0;
			}

			umb-icon {
				font-size: 1.2rem;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIRadioButtonListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-radio-button-list': ContentmentPropertyEditorUIRadioButtonListElement;
	}
}
