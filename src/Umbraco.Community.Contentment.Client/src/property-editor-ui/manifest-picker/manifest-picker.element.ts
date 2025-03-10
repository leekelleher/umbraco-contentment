// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, property, repeat, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UMB_ITEM_PICKER_MODAL, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-manifest-picker')
export class ContentmentPropertyEditorUIManifestPickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#extensions: Array<typeof umbExtensionsRegistry.MANIFEST_TYPES> = [];

	#extensionType?: string;

	#maxItems = Infinity;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#extensionType = config.getValueByAlias('extensionType');
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;

		this.#observeExtensions();
	}

	#observeExtensions() {
		if (!this.#extensionType) return;
		this.observe(umbExtensionsRegistry.byType(this.#extensionType), (extensions) => {
			this.#extensions = extensions.sort((a, b) => a.type.localeCompare(b.type) || a.alias.localeCompare(b.alias));
		});
	}

	async #onClick() {
		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		const modalContext = modalManager.open(this, UMB_ITEM_PICKER_MODAL, {
			data: {
				headline: `${this.localize.term('general_choose')}...`,
				items: this.#extensions
					.filter((ext) => ext.type === this.#extensionType)
					.map((ext) => ({
						label: (ext as any).meta?.name ?? ext.name,
						value: ext.alias,
						description: (ext as any).meta?.description ?? ext.alias,
						icon: (ext as any).meta?.icon,
					})),
			},
			modal: { size: 'medium' },
		});

		const modalValue = await modalContext.onSubmit();

		if (!modalValue) return;

		this.#setValue([modalValue.value], this.value?.length ?? 0);
	}

	async #removeItem(item: string, index: number) {
		if (!item || !this.value || index == -1) return;

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#setValue(value: Array<string> | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp.splice(index, 0, ...value);
		this.value = tmp;

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this.#extensionType) return html`<pre><code>Missing configuration for the extension type.</code></pre>`;
		return html`${this.#renderItems()} ${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if (!this.#extensionType || (this.value && this.value.length >= this.#maxItems)) return nothing;
		return html`
			<uui-button
				color="default"
				look="placeholder"
				label=${this.localize.term('general_choose')}
				@click=${this.#onClick}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.#extensionType || !this.value) return;
		return html`
			<uui-ref-list>
				${repeat(
					this.value,
					(alias) => alias,
					(alias, index) => this.#renderItem(alias, index)
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(alias: string, index: number) {
		const ext = this.#extensions.find((ext) => ext.alias === alias) as any;
		return html`
			<uui-ref-node
				name=${ext.meta?.name ?? ext.name}
				detail=${ext.meta?.description ?? ext.alias}
				?standalone=${this.#maxItems === 1}>
				${when(ext.meta?.icon, (_icon) => html`<umb-icon slot="icon" name=${_icon}></umb-icon>`)}
				<uui-action-bar slot="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						@click=${() => this.#removeItem(alias, index)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	static override readonly styles = [
		css`
			:host {
				display: flex;
				flex-direction: column;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIManifestPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-manifest-picker': ContentmentPropertyEditorUIManifestPickerElement;
	}
}
