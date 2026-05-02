// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import {
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	property,
	repeat,
	state,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, parseInt } from '../../utils/index.js';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { DataListService } from '../../api/index.js';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbFormControlMixin, UMB_VALIDATION_EMPTY_LOCALIZATION_KEY } from '@umbraco-cms/backoffice/validation';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentConfigurationEditorValue, ContentmentListItem } from '../types.js';
import type { InputType } from '@umbraco-cms/backoffice/external/uui';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import './uui-input-wrapper.element.js';

@customElement('contentment-property-editor-ui-text-input')
export class ContentmentPropertyEditorUITextInputElement
	extends UmbFormControlMixin<string, typeof UmbLitElement, undefined>(UmbLitElement, undefined)
	implements UmbPropertyEditorUiElement
{
	@property({ type: Boolean })
	mandatory?: boolean;

	@property({ type: String })
	mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;

	@property()
	name?: string;

	@property({ type: Boolean, reflect: true })
	readonly = false;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this._inputType = config.getValueByAlias<InputType>('inputType') ?? 'text';
		this._placeholderText = config.getValueByAlias<string>('placeholderText');
		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		const maxChars = parseInt(config.getValueByAlias('maxChars'));
		this._maxChars = maxChars && maxChars > 0 ? maxChars : undefined;
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
	private _maxChars?: number;

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
		await this.#init().catch(() => undefined);
	}

	async #init() {
		this.addFormControlElement(this.shadowRoot!.querySelector('input')!);

		if (!this._dataSource) return;

		const body = { dataSource: this._dataSource, listEditor: null };

		const { data } = await tryExecute(this, DataListService.postDataListEditor({ body }));

		if (!data) return;

		this._items = (data.config?.find((x) => x.alias === 'items')?.value as Array<ContentmentListItem>) ?? [];
	}

	#onInput(event: InputEvent & { target: HTMLInputElement }) {
		const newValue = (event.target as HTMLInputElement).value;
		if (newValue === this.value) return;
		this.value = newValue;

		// Show exceed validation instantly when limit is reached
		if (!!this._maxChars && newValue.length > this._maxChars) {
			this.pristine = false;
		}

		this.dispatchEvent(new UmbChangeEvent());
	}

	#renderCharacterCount() {
		if (!this._maxChars || this.readonly) return nothing;

		const remaining = this._maxChars - (this.value?.length ?? 0);

		// Only surface the count once the user is within the warning threshold.
		const threshold = Math.max(50, Math.floor(this._maxChars * 0.2));
		if (remaining > threshold) return nothing;

		return when(
			remaining < 0,
			() => html`
				<div class="char-count exceeded">
					${this.localize.htmlString('#textbox_characters_exceed', this._maxChars, -remaining)}
				</div>
			`,
			() => html`<div class="char-count">${this.localize.htmlString('#textbox_characters_left', remaining)}</div>`,
		);
	}

	override render() {
		const hasItems = !!this._items?.length;
		return html`
			<contentment-uui-input-wrapper>
				${when(this._prepend, (icon) => html`<umb-icon slot="prepend" name=${icon}></umb-icon>`)}
				<input
					slot="input"
					type=${this._inputType}
					autocomplete=${this._autocomplete ? 'on' : 'off'}
					label=${this.name ?? 'Text input'}
					list=${ifDefined(hasItems ? 'items' : undefined)}
					placeholder=${ifDefined(this._placeholderText)}
					spellcheck=${this._spellcheck ? 'true' : 'false'}
					.value=${this.value ?? ''}
					@input=${this.#onInput} />
				${when(this._append, (icon) => html`<umb-icon slot="append" name=${icon}></umb-icon>`)}
			</contentment-uui-input-wrapper>
			${this.#renderCharacterCount()}
			${when(
				hasItems,
				() => html`
					<datalist id="items">
						${repeat(
							this._items,
							(item) => item.value,
							(item) => html`<option label=${item.name} value=${item.value}></option>`,
						)}
					</datalist>
				`,
			)}
		`;
	}

	static override styles = [
		css`
			input {
				font-family: inherit;
				padding: var(--uui-input-padding);
				font-size: inherit;
				color: inherit;
				border-radius: var(--uui-border-radius);
				box-sizing: border-box;
				border: none;
				background: none;
				min-width: 0;
				flex-grow: 1;
				flex-shrink: 1;
				height: inherit;
				text-align: inherit;
				outline: none;
				text-overflow: ellipsis;
				line-height: 1;
				flex: 1 1 auto;
				min-width: 60px;
			}

			umb-icon {
				display: flex;
				align-items: center;
				line-height: 1;
				height: 100%;
			}

			umb-icon[slot='prepend'] {
				padding-left: var(--uui-size-space-2);
			}
			umb-icon[slot='append'] {
				padding-right: var(--uui-size-space-2);
			}

			.char-count {
				display: block;
				color: var(--uui-color-text-alt);
				font-size: 0.85em;
				text-align: right;
				margin-top: var(--uui-size-space-1);

				&.exceeded {
					color: var(--uui-color-danger);
				}
			}

			:host(:not(:focus-within)) .char-count {
				display: none;
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
