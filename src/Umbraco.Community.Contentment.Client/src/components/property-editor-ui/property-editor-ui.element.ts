// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher
// This Source Code has been derived from Umbraco CMS Backoffice.
// https://github.com/umbraco/Umbraco.CMS.Backoffice/blob/v14.0.0-beta002/src/packages/core/property/property/property.element.ts
// Modified under the permissions of the MIT License.
// Modifications are licensed under the Mozilla Public License.

import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { css, customElement, html, nothing, property, state } from '@umbraco-cms/backoffice/external/lit';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbFormControlMixin, UMB_VALIDATION_EMPTY_LOCALIZATION_KEY } from '@umbraco-cms/backoffice/validation';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ManifestPropertyEditorUi, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui')
export default class ContentmentPropertyEditorUiElement
	extends UmbFormControlMixin<any | undefined, typeof UmbLitElement, undefined>(UmbLitElement)
	implements UmbPropertyEditorUiElement
{
	@property({ attribute: false })
	config: UmbPropertyEditorUiElement['config'];

	@property({ type: Boolean })
	mandatory = false;

	@property({ type: String })
	mandatoryMessage = UMB_VALIDATION_EMPTY_LOCALIZATION_KEY;

	@property()
	name?: string;

	@property({ type: String, attribute: 'property-editor-ui-alias' })
	public set propertyEditorUiAlias(value: string | undefined) {
		this.#propertyEditorUiAlias = value;
		this.#observePropertyEditorUI();
	}
	public get propertyEditorUiAlias(): string | undefined {
		return this.#propertyEditorUiAlias;
	}
	#propertyEditorUiAlias?: string;

	@property({ type: Boolean, reflect: true })
	readonly = false;

	@state()
	private _element?: ManifestPropertyEditorUi['ELEMENT_TYPE'];

	@state()
	private _undefined: boolean = false;

	constructor() {
		super();

		// This component needs a validator stub, so that the validation events can propagate upwards. [LK]
		this.addValidator(
			'valid',
			() => '#errors_propertyHasErrors',
			() => true
		);
	}

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
			this._element.mandatory = this.mandatory;
			this._element.mandatoryMessage = this.mandatoryMessage;
			this._element.manifest = manifest;
			this._element.readonly = this.readonly;
			this._element.value = this.value;
			this._element.name = this.name;

			if (this.config) {
				this._element.config = this.config;
			}

			if ('validity' in this._element) {
				this.addFormControlElement(this._element as any);
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

	override destroy(): void {
		super.destroy();
		this.#propertyEditorUiAlias = undefined;
		this._element = undefined;
		this._undefined = false;
	}

	static override styles = [
		css`
			:host {
				display: contents;
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui': ContentmentPropertyEditorUiElement;
	}
}
