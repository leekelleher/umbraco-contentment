// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import {
	css,
	html,
	nothing,
	customElement,
	property,
	query,
	queryAll,
	state,
	repeat,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentListItem } from '../types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIInputElement, UUIInputEvent, UUITagElement } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-tags')
export class ContentmentPropertyEditorUITagsElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property({ type: Array })
	value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		// this._showIcons = parseBoolean(config.getValueByAlias('showIcons'));
		// this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));

		this._items = config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
	}

	@state()
	private _items: Array<ContentmentListItem> = [];

	@state()
	private _matches: Array<ContentmentListItem> = [];

	@state()
	private _currentInput = '';

	@query('#main-tag')
	private _mainTag!: UUITagElement;

	@query('#tag-input')
	private _tagInput!: UUIInputElement;

	@query('#input-width-tracker')
	private _widthTracker!: HTMLElement;

	@queryAll('.options')
	private _optionCollection?: HTMLCollectionOf<HTMLInputElement>;

	public override focus() {
		this._tagInput.focus();
	}

	#getExistingTags(query: string) {
		this._matches = this._items.filter((tag) => tag.value.toLowerCase().includes(query.toLowerCase()));
		console.log('getExistingTags', query, this._matches);
	}

	#onKeydown(e: KeyboardEvent) {
		//Prevent tab away if there is a input.
		if (e.key === 'Tab' && (this._tagInput.value as string).trim().length && !this._matches.length) {
			e.preventDefault();
			this.#createTag();
			return;
		}

		if (e.key === 'Enter') {
			this.#createTag();
			return;
		}

		if (e.key === 'ArrowDown' || e.key === 'Tab') {
			e.preventDefault();
			this._currentInput = this._optionCollection?.item(0)?.value ?? this._currentInput;
			this._optionCollection?.item(0)?.focus();
			return;
		}

		this.#inputError(false);
	}

	#onInput(e: UUIInputEvent) {
		this._currentInput = e.target.value as string;
		if (!this._currentInput || !this._currentInput.length) {
			this._matches = [];
		} else {
			this.#getExistingTags(this._currentInput);
		}
	}

	protected override updated(): void {
		this._mainTag.style.width = `${this._widthTracker.offsetWidth - 4}px`;
	}

	#onBlur() {
		if (this._matches.length) return;
		else this.#createTag();
	}

	#createTag() {
		this.#inputError(false);
		const newTag = (this._tagInput.value as string).trim();
		if (!newTag) return;

		const tagExists = this.value?.find((tag) => tag === newTag);
		if (tagExists) return this.#inputError(true);

		this.#inputError(false);

		this.value = [...(this.value ?? []), newTag];
		this._tagInput.value = '';
		this._currentInput = '';

		this.dispatchEvent(new UmbChangeEvent());
	}

	#inputError(error: boolean) {
		if (error) {
			this._mainTag.style.border = '1px solid var(--uui-color-danger)';
			this._tagInput.style.color = 'var(--uui-color-danger)';
			return;
		}
		this._mainTag.style.border = '';
		this._tagInput.style.color = '';
	}

	#delete(tag: string) {
		const currentItems = [...(this.value ?? [])];
		const index = currentItems.findIndex((x) => x === tag);

		currentItems.splice(index, 1);
		currentItems.length ? (this.value = [...currentItems]) : (this.value = []);

		this.dispatchEvent(new UmbChangeEvent());
	}

	/** Dropdown */

	#optionClick(index: number) {
		this._tagInput.value = this._optionCollection?.item(index)?.value ?? '';
		this.#createTag();
		this.focus();
		return;
	}

	#optionKeydown(e: KeyboardEvent, index: number) {
		if (e.key === 'Enter' || e.key === 'Tab') {
			e.preventDefault();
			this._currentInput = this._optionCollection?.item(index)?.value ?? '';
			this.#createTag();
			this.focus();
			return;
		}

		if (e.key === 'ArrowDown') {
			e.preventDefault();
			if (!this._optionCollection?.item(index + 1)) return;
			this._optionCollection?.item(index + 1)?.focus();
			this._currentInput = this._optionCollection?.item(index + 1)?.value ?? '';
			return;
		}

		if (e.key === 'ArrowUp') {
			e.preventDefault();
			if (!this._optionCollection?.item(index - 1)) return;
			this._optionCollection?.item(index - 1)?.focus();
			this._currentInput = this._optionCollection?.item(index - 1)?.value ?? '';
		}

		if (e.key === 'Backspace') {
			this.focus();
		}
	}

	/** Render */

	override render() {
		return html`
			<div id="wrapper">
				${this.#enteredTags()}
				<span id="main-tag-wrapper">
					<uui-tag id="input-width-tracker" aria-hidden="true" style="visibility:hidden;opacity:0;position:absolute;">
						${this._currentInput}
					</uui-tag>
					${this.#renderAddButton()}
				</span>
			</div>
		`;
	}

	#enteredTags() {
		return html`
			${this.value?.map((tag) => {
				return html`
					<uui-tag class="tag">
						<span>${tag}</span>
						<uui-icon name="icon-wrong" @click=${() => this.#delete(tag)}></uui-icon>
					</uui-tag>
				`;
			})}
		`;
	}

	#renderTagOptions() {
		if (!this._currentInput.length || !this._matches.length) return nothing;
		const matchfilter = this._matches; //.filter((tag) => tag !== this._items.find((x) => x.value === tag.value));
		console.log('matchfilter', matchfilter, this._matches, this._currentInput);
		if (!matchfilter.length) return;
		return html`
			<div id="matchlist">
				${repeat(
					matchfilter.slice(0, 5),
					(tag: ContentmentListItem) => tag.value,
					(tag: ContentmentListItem, index: number) => {
						return html`
							<input
								class="options"
								id="tag-${tag.value}"
								type="radio"
								name=""
								@click=${() => this.#optionClick(index)}
								@keydown="${(e: KeyboardEvent) => this.#optionKeydown(e, index)}"
								value=${tag.value ?? ''} />
							<label for="tag-${tag.value}">${tag.name}</label>
						`;
					}
				)}
			</div>
		`;
	}

	#renderAddButton() {
		return html`
			<uui-tag look="outline" id="main-tag" @click=${this.focus} slot="trigger">
				<input
					id="tag-input"
					aria-label="tag input"
					placeholder="Enter tag"
					.value=${this._currentInput ?? undefined}
					@keydown=${this.#onKeydown}
					@input=${this.#onInput}
					@blur=${this.#onBlur} />
				<uui-icon id="icon-add" name="icon-add"></uui-icon>
				${this.#renderTagOptions()}
			</uui-tag>
		`;
	}

	static override styles = [
		css`
			#wrapper {
				box-sizing: border-box;
				display: flex;
				gap: var(--uui-size-space-2);
				flex-wrap: wrap;
				align-items: center;
				padding: var(--uui-size-space-2);
				border: 1px solid var(--uui-color-border);
				background-color: var(--uui-input-background-color, var(--uui-color-surface));
				flex: 1;
				min-height: 40px;
			}

			#main-tag-wrapper {
				position: relative;
			}

			/** Tags */

			uui-tag {
				position: relative;
				max-width: 200px;
			}

			uui-tag uui-icon {
				cursor: pointer;
				min-width: 12.8px !important;
			}

			uui-tag span {
				overflow: hidden;
				text-overflow: ellipsis;
				white-space: nowrap;
			}

			/** Created tags */

			.tag uui-icon {
				margin-left: var(--uui-size-space-2);
			}

			.tag uui-icon:hover,
			.tag uui-icon:active {
				color: var(--uui-color-selected-contrast);
			}

			/** Main tag */

			#main-tag {
				padding: 3px;
				background-color: var(--uui-color-selected-contrast);
				min-width: 20px;
				position: relative;
				border-radius: var(--uui-size-5, 12px);
			}

			#main-tag uui-icon {
				position: absolute;
				top: 50%;
				left: 50%;
				transform: translate(-50%, -50%);
			}

			#main-tag:hover uui-icon,
			#main-tag:active uui-icon {
				color: var(--uui-color-selected);
			}

			#main-tag #tag-input:focus ~ uui-icon,
			#main-tag #tag-input:not(:placeholder-shown) ~ uui-icon {
				display: none;
			}

			#main-tag:has(*:hover),
			#main-tag:has(*:active),
			#main-tag:has(*:focus) {
				border: 1px solid var(--uui-color-selected-emphasis);
			}

			#main-tag:has(#tag-input:not(:focus)):hover {
				cursor: pointer;
				border: 1px solid var(--uui-color-selected-emphasis);
			}

			#main-tag:not(:focus-within) #tag-input:placeholder-shown {
				opacity: 0;
			}

			#main-tag:has(#tag-input:focus),
			#main-tag:has(#tag-input:not(:placeholder-shown)) {
				min-width: 65px;
			}

			#main-tag #tag-input {
				box-sizing: border-box;
				max-height: 25.8px;
				background: none;
				font: inherit;
				color: var(--uui-color-selected);
				line-height: reset;
				padding: 0 var(--uui-size-space-2);
				margin: 0.5px 0 -0.5px;
				border: none;
				outline: none;
				width: 100%;
			}

			/** Dropdown matchlist */

			#matchlist input[type='radio'] {
				-webkit-appearance: none;
				appearance: none;
				/* For iOS < 15 to remove gradient background */
				background-color: transparent;
				/* Not removed via appearance */
				margin: 0;
			}

			uui-tag:focus-within #matchlist {
				display: flex;
			}

			#matchlist {
				display: none;
				display: flex;
				flex-direction: column;
				background-color: var(--uui-color-surface);
				position: absolute;
				width: 150px;
				left: 0;
				top: var(--uui-size-space-6);
				border-radius: var(--uui-border-radius);
				border: 1px solid var(--uui-color-border);
			}

			#matchlist label {
				display: none;
				cursor: pointer;
				box-sizing: border-box;
				display: block;
				width: 100%;
				background: none;
				border: none;
				text-align: left;
				padding: 10px 12px;

				/** Overflow */
				overflow: hidden;
				text-overflow: ellipsis;
				white-space: nowrap;
			}

			#matchlist label:hover,
			#matchlist label:focus,
			#matchlist label:focus-within,
			#matchlist input[type='radio']:focus + label {
				display: block;
				background-color: var(--uui-color-focus);
				color: var(--uui-color-selected-contrast);
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
