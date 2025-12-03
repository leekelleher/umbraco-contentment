// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import {
	css,
	customElement,
	html,
	ifDefined,
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

type ContentmentDataListCheckboxOption = ContentmentListItem & { checked: boolean };

@customElement('contentment-property-editor-ui-checkbox-list')
export class ContentmentPropertyEditorUICheckboxListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Array })
	public set value(value: Array<string> | undefined) {
		this.#value = value ?? [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		this._checkAll = parseBoolean(config.getValueByAlias('checkAll'));
		this._showDescriptions = parseBoolean(config.getValueByAlias('showDescriptions'));
		this._showIcons = parseBoolean(config.getValueByAlias('showIcons'));

		this._listStyles = config.getValueByAlias('listStyles');
		this._listItemStyles = config.getValueByAlias('listItemStyles');

		const items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
		this._items = items.map((item) => ({ ...item, checked: this.value?.includes(item.value) ?? false }));

		if (this._checkAll) {
			this._toggleChecked = this._items.every((item) => item.checked);
		}
	}

	@state()
	private _checkAll = false;

	@state()
	private _items: Array<ContentmentDataListCheckboxOption> = [];

	@state()
	private _listStyles?: string;

	@state()
	private _listItemStyles?: string;

	@state()
	private _showDescriptions = false;

	@state()
	private _showIcons = false;

	@state()
	private _toggleChecked = false;

	#onClick(item: ContentmentDataListCheckboxOption) {
		item.checked = !item.checked;

		this._toggleChecked = this._items.every((item) => item.checked);

		const values: Array<string> = [];

		this._items.forEach((item) => {
			if (item.checked) {
				values?.push(item.value);
			}
		});

		this.value = values;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onToggle(event: CustomEvent & { target: HTMLInputElement }) {
		this._toggleChecked = event.target.checked;

		const values: Array<string> = [];

		this._items.forEach((item) => {
			item.checked = this._toggleChecked;
			if (item.checked) {
				values.push(item.value);
			}
		});

		this.value = values;

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
			${when(this._checkAll, () => this.#renderCheckAll())}
			<ul style=${ifDefined(this._listStyles)}>
				${repeat(
					this._items,
					(item) => item.value,
					(item) => this.#renderItem(item)
				)}
			</ul>
		`;
	}

	#renderCheckAll() {
		const label = !this._toggleChecked
			? this.localize.term('contentment_checkboxListCheckAll')
			: this.localize.term('contentment_checkboxListUncheckAll');
		return html`
			<ul>
				<li>
					<uui-checkbox label=${label} ?checked=${this._toggleChecked} @change=${this.#onToggle}></uui-checkbox>
				</li>
			</ul>
		`;
	}

	#renderItem(item: ContentmentDataListCheckboxOption) {
		return html`
			<li style=${ifDefined(this._listItemStyles)}>
				<uui-checkbox
					label=${item.name}
					value=${item.value}
					?checked=${item.checked}
					?disabled=${item.disabled}
					@change=${() => this.#onClick(item)}>
					<div class="outer">
						${when(this._showIcons && item.icon, (icon) => html`<umb-icon name=${icon}></umb-icon>`)}
						<uui-form-layout-item>
							<span slot="label">${this.localize.string(item.name)}</span>
							${when(
								this._showDescriptions && item.description,
								() => html`<span slot="description">${unsafeHTML(item.description)}</span>`
							)}
						</uui-form-layout-item>
					</div>
				</uui-checkbox>
			</li>
		`;
	}

	static override styles = [
		css`
			ul {
				list-style: none;
				padding: 0;
				margin: 0;
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

export { ContentmentPropertyEditorUICheckboxListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-checkbox-list': ContentmentPropertyEditorUICheckboxListElement;
	}
}
