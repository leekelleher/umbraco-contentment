// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import {
	UmbMemberTypeEntityType,
	UmbMemberTypePickerModalData,
	UmbMemberTypePickerModalValue,
	UmbMemberTypeTreeItemModel,
	UMB_MEMBER_TYPE_ITEM_REPOSITORY_ALIAS,
	UMB_MEMBER_TYPE_PICKER_MODAL,
} from '@umbraco-cms/backoffice/member-type';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPickerInputContext } from '@umbraco-cms/backoffice/picker-input';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { UmbUniqueItemModel } from '@umbraco-cms/backoffice/models';

const ELEMENT_NAME = 'contentment-property-editor-ui-member-type-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIMemberTypePickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#pickerContext = new ContentmentMemberTypePickerContext(this);

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
		this.#pickerContext?.openPicker({
			hideTreeRoot: true,
		});
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
				look="placeholder"
				@click=${this.#openPicker}
				label=${this.localize.term('general_choose')}></uui-button>
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
				<umb-icon slot="icon" name="icon-user"></umb-icon>
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

export { ContentmentPropertyEditorUIMemberTypePickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIMemberTypePickerElement;
	}
}

// TODO: [LK] Temporary. Remove this once `UmbMemberTypePickerContext` is publicly available.
// https://github.com/umbraco/Umbraco.CMS.Backoffice/blob/v14.1.1/src/packages/members/member-type/components/input-member-type/input-member-type.context.ts

class ContentmentMemberTypePickerContext extends UmbPickerInputContext<
	{ entityType: UmbMemberTypeEntityType; unique: string; name: string; icon: string },
	UmbMemberTypeTreeItemModel,
	UmbMemberTypePickerModalData,
	UmbMemberTypePickerModalValue
> {
	constructor(host: UmbControllerHost) {
		super(host, UMB_MEMBER_TYPE_ITEM_REPOSITORY_ALIAS, UMB_MEMBER_TYPE_PICKER_MODAL);
	}
}
