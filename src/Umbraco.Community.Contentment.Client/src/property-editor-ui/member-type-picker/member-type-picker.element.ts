// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseInt } from '../../utils/index.js';
import { css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbMemberTypePickerInputContext } from '@umbraco-cms/backoffice/member-type';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UmbUniqueItemModel } from '@umbraco-cms/backoffice/models';

@customElement('contentment-property-editor-ui-member-type-picker')
export class ContentmentPropertyEditorUIMemberTypePickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#maxItems = Infinity;

	#pickerContext = new UmbMemberTypePickerInputContext(this);

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

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#pickerContext.max = this.#maxItems;
	}

	constructor() {
		super();

		this.observe(this.#pickerContext.selection, (selection) => (this.value = selection.join(',')), '_observeSelection');
		this.observe(this.#pickerContext.selectedItems, (selectedItems) => (this._items = selectedItems), '_observerItems');
	}

	#onAdd() {
		this.#pickerContext?.openPicker({
			hideTreeRoot: true,
		});
	}

	#onRemove(item: UmbUniqueItemModel) {
		this.#pickerContext?.requestRemoveItem(item.unique);
	}

	override render() {
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (this.value && this.value.length >= this.#maxItems) return;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('general_choose')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderItems() {
		if (!this._items) return;
		return html`
			<uui-ref-list>
				${repeat(
					this._items,
					(item) => item.unique,
					(item) => this.#renderItem(item),
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(item: UmbUniqueItemModel) {
		if (!item.unique) return;
		return html`
			<uui-ref-node name=${item.name} id=${item.unique}>
				<umb-icon slot="icon" name="icon-user"></umb-icon>
				<uui-action-bar slot="actions">
					<uui-button label=${this.localize.term('general_remove')} @click=${() => this.#onRemove(item)}></uui-button>
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

export { ContentmentPropertyEditorUIMemberTypePickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-member-type-picker': ContentmentPropertyEditorUIMemberTypePickerElement;
	}
}
