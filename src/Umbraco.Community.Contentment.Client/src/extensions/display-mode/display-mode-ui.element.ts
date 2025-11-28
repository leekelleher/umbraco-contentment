// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { ContentmentDisplayModeContext } from './display-mode.context.js';
import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentDisplayModeExtentionManifestType } from './display-mode.extension.js';
import type { ContentmentListItem } from '../../property-editor-ui/types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-display-mode-ui')
export default class ContentmentDisplayModeUIElement extends UmbLitElement {
	#context = new ContentmentDisplayModeContext(this);

	@property()
	addButtonLabelKey: string = 'general_add';

	@property({ type: Boolean })
	public set allowAdd(value: boolean) {
		this.#context.setAllowAdd(value);
	}

	@property({ type: Boolean })
	public set allowEdit(value: boolean) {
		this.#context.setAllowEdit(value);
	}

	@property({ type: Boolean })
	public set allowRemove(value: boolean) {
		this.#context.setAllowRemove(value);
	}

	@property({ type: Boolean })
	public set allowSort(value: boolean) {
		this.#context.setAllowSort(value);
	}

	@property({ attribute: false })
	public set canEdit(callback: (item: ContentmentListItem, index: number) => boolean) {
		this.#context.setCanEdit(callback);
	}

	@property({ type: Object, attribute: false })
	public set config(value: UmbPropertyEditorConfigCollection | undefined) {
		this.#context.setConfig(value);
	}

	@property({ type: Array, attribute: false })
	public set items(value: Array<ContentmentListItem> | undefined) {
		this.#context.setItems(value);
	}

	@property({ attribute: 'ui-alias' })
	public set uiAlias(value: string | undefined) {
		this.#uiAlias = value;
		this.#observePropertyEditorUI();
	}
	public get uiAlias(): string | undefined {
		return this.#uiAlias;
	}
	#uiAlias?: string | undefined;

	@state()
	private _element?: ContentmentDisplayModeExtentionManifestType['ELEMENT_TYPE'];

	@state()
	private _undefined: boolean = false;

	#observePropertyEditorUI() {
		if (this.#uiAlias) {
			this.observe(
				umbExtensionsRegistry.byTypeAndAlias('contentmentDisplayMode', this.#uiAlias),
				(manifest) => {
					if (manifest) {
						this.#getDisplayModeUI(manifest);
					} else {
						console.error(`Failed to find manifest for display mode UI alias: ${this.#uiAlias}`);
						this._undefined = true;
					}
				},
				'_observePropertyEditorUI'
			);
		}
	}

	async #getDisplayModeUI(manifest?: ContentmentDisplayModeExtentionManifestType | null) {
		if (!manifest) return;

		const element = await createExtensionElement(manifest, 'lee-was-here');

		if (!element) {
			console.error(`Failed to create extension element for manifest: ${manifest}`);
			this._undefined = true;
		}

		const oldElement = this._element;
		this._element = element;

		if (this._element) {
			this._element.addButtonLabelKey = this.addButtonLabelKey;
		}

		this.dispatchEvent(new CustomEvent('loaded'));

		this.requestUpdate('_element', oldElement);
	}

	// Disable the Shadow DOM for this element; as event propagation needs to pass-through.
	override createRenderRoot() {
		return this;
	}

	override render() {
		if (this._element) return this._element;
		if (this._undefined) return html`<lee-was-here></lee-was-here>`;
		return nothing;
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-display-mode-ui': ContentmentDisplayModeUIElement;
	}
}
