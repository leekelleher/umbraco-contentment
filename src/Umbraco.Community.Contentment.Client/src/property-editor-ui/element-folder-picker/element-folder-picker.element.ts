// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbElementFolderItemRepository, UMB_ELEMENT_FOLDER_PICKER_MODAL } from '@umbraco-cms/backoffice/element';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

const EMPTY_GUID = '00000000-0000-0000-0000-000000000000';

@customElement('contentment-property-editor-ui-element-folder-picker')
export class ContentmentPropertyEditorUIElementFolderPickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#itemRepository = new UmbElementFolderItemRepository(this);

	@state()
	private _folderName?: string;

	@property({ type: String })
	public value?: string;

	public config?: UmbPropertyEditorUiElement['config'];

	override updated(changedProperties: Map<string | symbol, unknown>) {
		super.updated(changedProperties);
		if (changedProperties.has('value')) {
			this._folderName = undefined;
			const key = this.#parseKey(this.value);
			if (key) this.#loadFolderName(key);
		}
	}

	async #loadFolderName(key: string) {
		const { data } = await this.#itemRepository.requestItems([key]);
		this._folderName = data?.[0]?.name;
	}

	async #openPicker() {
		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		const currentKey = this.#parseKey(this.value);

		const modal = modalManager?.open(this, UMB_ELEMENT_FOLDER_PICKER_MODAL, {
			value: { selection: currentKey ? [currentKey] : [] },
		});

		const result = await modal?.onSubmit().catch(() => undefined);
		if (!result) return;

		const selection = result.selection.filter(Boolean) as Array<string>;
		this.value = selection.length > 0 ? JSON.stringify(selection) : undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#remove() {
		this.value = undefined;
		this._folderName = undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#parseKey(json: string | undefined): string | undefined {
		if (!json) return undefined;
		try {
			const keys = JSON.parse(json) as Array<string>;
			const key = keys?.[0];
			return key && key !== EMPTY_GUID ? key : undefined;
		} catch {
			return undefined;
		}
	}

	override render() {
		const key = this.#parseKey(this.value);
		return html`
			<uui-ref-list>
				<uui-ref-node name=${key ? (this._folderName ?? key) : 'Element Library'} standalone>
					<umb-icon slot="icon" name=${key ? 'icon-folder' : 'icon-umbraco'}></umb-icon>
					<uui-action-bar slot="actions">
						<uui-button label=${this.localize.term('general_change')} @click=${this.#openPicker}></uui-button>
						${key
							? html`<uui-button label=${this.localize.term('general_remove')} @click=${this.#remove}></uui-button>`
							: nothing}
					</uui-action-bar>
				</uui-ref-node>
			</uui-ref-list>
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

export { ContentmentPropertyEditorUIElementFolderPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-element-folder-picker': ContentmentPropertyEditorUIElementFolderPickerElement;
	}
}
