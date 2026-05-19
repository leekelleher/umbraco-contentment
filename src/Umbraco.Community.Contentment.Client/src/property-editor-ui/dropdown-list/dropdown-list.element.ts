// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { customElement, html, ifDefined, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { ContentmentListItem } from '../types.js';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUISelectElement } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-dropdown-list')
export class ContentmentPropertyEditorUIDropdownListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#allowEmpty = false;

	@property()
	name?: string;

	@property()
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) === true ? value[0] : (value ?? '');
	}
	public get value(): string | undefined {
		return this.#value;
	}
	#value?: string = '';

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		this.#allowEmpty = parseBoolean(config.getValueByAlias('allowEmpty'));

		const items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];

		if (items.length) {
			this._options = items.map((item) => ({
				name: this.localize.string(item.name),
				value: item.value,
				disabled: item.disabled ?? false,
				selected: item.value === this.#value,
			}));

			if (this.#allowEmpty) {
				this._options.unshift({ name: '', value: '', selected: false });
			}

			if (!this.#allowEmpty && !this._options.find((option) => option.selected)) {
				this._options[0].selected = true;
			}
		}
	}

	protected override firstUpdated(_changedProperties: PropertyValues): void {
		super.firstUpdated(_changedProperties);

		if (!this.#allowEmpty && this._options.length > 0 && (!this.value || this.value === '')) {
			this.value = this._options[0].value;
			this.dispatchEvent(new UmbChangeEvent());
		}
	}

	@state()
	private _options: UUISelectElement['options'] = [];

	#onChange(event: CustomEvent & { target: UUISelectElement }) {
		this.value = event.target.value as string;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._options?.length) {
			return html`
				<contentment-info-box
					compact
					type="warning"
					icon="icon-alert"
					headline="There are no items to display"></contentment-info-box>
			`;
		}

		return html`
			<uui-select
				.label=${this.name ?? 'Dropdown List'}
				.options=${this._options}
				value=${ifDefined(this.value)}
				@change=${this.#onChange}>
			</uui-select>
		`;
	}
}

export { ContentmentPropertyEditorUIDropdownListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-dropdown-list': ContentmentPropertyEditorUIDropdownListElement;
	}
}
