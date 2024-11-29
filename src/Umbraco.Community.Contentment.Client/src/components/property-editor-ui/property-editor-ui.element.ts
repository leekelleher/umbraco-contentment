// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher
// This Source Code has been derived from Umbraco CMS Backoffice.
// https://github.com/umbraco/Umbraco.CMS.Backoffice/blob/v14.0.0-beta002/src/packages/core/property/property/property.element.ts
// Modified under the permissions of the MIT License.
// Modifications are licensed under the Mozilla Public License.

import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ManifestPropertyEditorUi, UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

import '../lee-was-here/lee-was-here.element.js';

@customElement('contentment-property-editor-ui')
export default class ContentmentPropertyEditorUiElement extends UmbLitElement {
	@property({ attribute: false })
	config?: UmbPropertyEditorConfigCollection | undefined;

	@property({ type: String, attribute: 'property-editor-ui-alias' })
	public set propertyEditorUiAlias(value: string | undefined) {
		this.#propertyEditorUiAlias = value;
		this.#observePropertyEditorUI();
	}
	public get propertyEditorUiAlias(): string | undefined {
		return this.#propertyEditorUiAlias;
	}
	#propertyEditorUiAlias?: string;

	@property()
	value?: any;

	@state()
	private _element?: ManifestPropertyEditorUi['ELEMENT_TYPE'];

	@state()
	private _undefined: boolean = false;

	#observePropertyEditorUI() {
		if (this.#propertyEditorUiAlias) {
			this.observe(
				umbExtensionsRegistry.byTypeAndAlias('propertyEditorUi', this.#propertyEditorUiAlias),
				(manifest) => {
					if (manifest) {
						this.#getPropertyEditorUI(manifest);
					} else {
						console.error(`Failed to find manifest for property editor UI alias: ${this.#propertyEditorUiAlias}`);
						this._undefined = true;
					}
				},
				'_observePropertyEditorUI'
			);
		}
	}

	async #getPropertyEditorUI(manifest?: ManifestPropertyEditorUi | null) {
		if (!manifest) return;

		const element = await createExtensionElement(manifest, 'lee-was-here');

		if (!element) {
			console.error(`Failed to create extension element for manifest: ${manifest}`);
			this._undefined = true;
		}

		const oldElement = this._element;

		this._element = element;

		if (this._element) {
			this._element.value = this.value;

			if (this.config) {
				this._element.config = this.config;
			}

			this.dispatchEvent(new CustomEvent('loaded'));
		}

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
		'contentment-property-editor-ui': ContentmentPropertyEditorUiElement;
	}
}
