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
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, parseInt } from '../../utils/index.js';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { DataListService } from '../../api/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UUIInputElement } from '@umbraco-cms/backoffice/external/uui';
import type { ContentmentConfigurationEditorValue, ContentmentListItem } from '../types.js';
import type { InputType } from '@umbraco-cms/backoffice/external/uui';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-text-input')
export class ContentmentPropertyEditorUITextInputElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property()
	name?: string;

	@property()
	value?: string;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this._inputType = config.getValueByAlias<InputType>('inputType') ?? 'text';
		this._placeholderText = config.getValueByAlias<string>('placeholderText');
		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		this._maxChars = parseInt(config.getValueByAlias('maxChars')) || 500;
		this._autocomplete = parseBoolean(config.getValueByAlias('autocomplete'));
		this._spellcheck = parseBoolean(config.getValueByAlias('spellcheck'));
		this._prepend = config.getValueByAlias<string>('prepend');
		this._append = config.getValueByAlias<string>('append');
	}

	@state()
	private _dataSource?: ContentmentConfigurationEditorValue;

	@state()
	private _inputType: InputType = 'text';

	@state()
	private _items: Array<ContentmentListItem> = [];

	@state()
	private _maxChars = 500;

	@state()
	private _autocomplete = false;

	@state()
	private _spellcheck = false;

	@state()
	private _placeholderText?: string;

	@state()
	private _prepend?: string;

	@state()
	private _append?: string;

	override async firstUpdated() {
		await Promise.all([await this.#init().catch(() => undefined)]);
	}

	async #init() {
		this._items = await new Promise<Array<ContentmentListItem>>(async (resolve, reject) => {
			if (!this._dataSource) return reject();

			const body = { dataSource: this._dataSource, listEditor: null };

			const { data } = await tryExecute(this, DataListService.postDataListEditor({ client: umbHttpClient, body }));

			if (!data) return reject();

			const items = (data.config?.find((x) => x.alias === 'items')?.value as Array<ContentmentListItem>) ?? [];

			resolve(items);
		});
	}

	#onInput(event: InputEvent & { target: HTMLInputElement }) {
		// TODO: [LK] `maxChars` validation + threshold warning message.
		this.value = event.target.value;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		const hasItems = !!this._items?.length;
		return html`
			${when(this._prepend, (icon) => html`<umb-icon class="prepend" name=${icon}></umb-icon>`)}
			<input
				type=${this._inputType}
				id="input"
				autocomplete=${this._autocomplete ? 'on' : 'off'}
				label=${this.name ?? 'Text input'}
				list=${ifDefined(hasItems ? 'items' : undefined)}
				max=${this._maxChars}
				placeholder=${ifDefined(this._placeholderText)}
				.value=${this.value ?? ''}
				spellcheck=${this._spellcheck ? 'true' : 'false'}
				@input=${this.#onInput} />
			${when(this._append, (icon) => html`<umb-icon class="append" name=${icon}></umb-icon>`)}
			${when(
				hasItems,
				() => html`
					<datalist id="items">
						${repeat(
							this._items,
							(item) => item.value,
							(item) => html`<option label=${item.name} value=${item.value}></option>`
						)}
					</datalist>
				`
			)}
		`;
	}

	static override styles = [
		UUIInputElement.styles,
		css`
			:host {
				width: 100%;
			}

			umb-icon {
				display: flex;
				align-items: center;
				line-height: 1;
				height: 100%;
			}

			umb-icon.prepend {
				padding-left: var(--uui-size-space-2);
			}
			umb-icon.append {
				padding-right: var(--uui-size-space-2);
			}
		`,
	];
}

export { ContentmentPropertyEditorUITextInputElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-text-input': ContentmentPropertyEditorUITextInputElement;
	}
}
