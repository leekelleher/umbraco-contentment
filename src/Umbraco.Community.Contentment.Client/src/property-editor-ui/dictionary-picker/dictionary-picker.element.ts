// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbDictionaryPickerInputContext } from '@umbraco-cms/backoffice/dictionary';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UmbUniqueItemModel } from '@umbraco-cms/backoffice/models';

@customElement('contentment-property-editor-ui-dictionary-picker')
export class ContentmentPropertyEditorUIDictionaryPickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#pickerContext = new UmbDictionaryPickerInputContext(this);

	@state()
	private _items?: Array<UmbUniqueItemModel>;

	@property({ type: String })
	public set value(unique: string | undefined) {
		this.#pickerContext?.setSelection(unique ? [unique] : []);
	}
	public get value(): string | undefined {
		const selection = this.#pickerContext?.getSelection() ?? [];
		return selection.length > 0 ? selection.join(',') : undefined;
	}

	public config?: UmbPropertyEditorConfigCollection | undefined;

	constructor() {
		super();

		this.observe(this.#pickerContext.selection, (selection) => (this.value = selection.join(',')), '_observeSelection');
		this.observe(this.#pickerContext.selectedItems, (selectedItems) => (this._items = selectedItems), '_observerItems');
	}

	#openPicker() {
		this.#pickerContext?.openPicker();
	}

	#removeItem(item: UmbUniqueItemModel) {
		this.#pickerContext?.requestRemoveItem(item.unique);
	}

	override render() {
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (this.value) return;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#openPicker}></uui-button>
		`;
	}

	#renderItems() {
		if (!this._items) return;
		return html`
			<uui-ref-list>
				${repeat(
					this._items,
					(item) => item.unique,
					(item) => this.#renderItem(item)
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(item: UmbUniqueItemModel) {
		if (!item.unique) return;
		return html`
			<uui-ref-node name=${item.name} id=${item.unique}>
				<umb-icon slot="icon" name="icon-book-alt"></umb-icon>
				<uui-action-bar slot="actions">
					<uui-button label=${this.localize.term('general_remove')} @click=${() => this.#removeItem(item)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	static override styles = [
		css`
			#btn-add {
				display: block;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIDictionaryPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-dictionary-picker': ContentmentPropertyEditorUIDictionaryPickerElement;
	}
}
