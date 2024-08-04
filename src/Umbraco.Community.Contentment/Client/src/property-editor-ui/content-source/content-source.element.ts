// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UMB_APP_CONTEXT } from '@umbraco-cms/backoffice/app';

const ELEMENT_NAME = 'contentment-property-editor-ui-content-source';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIContentSourceElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#umbBackofficePath = '/umbraco';

	@state()
	private _loaded = false;

	@property({ type: Object })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	constructor() {
		super();

		this.consumeContext(UMB_APP_CONTEXT, (appContext) => {
			this.#umbBackofficePath = appContext.getBackofficePath();
		});
	}

	override async firstUpdated() {
		const path =
			'/backoffice/packages/property-editors/content-picker/dynamic-root/input-content-picker-document-root/input-content-picker-document-root.element.js';
		await import(this.#umbBackofficePath + path);
		this._loaded = true;
	}

	#onChange(event: CustomEvent & { target: { data: unknown } }) {
		this.value = event.target.data ?? {};
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		if (!this._loaded) return html`<uui-loader></uui-loader>`;
		return html`
			<umb-input-content-picker-document-root .data=${this.value} @change=${this.#onChange}>
			</umb-input-content-picker-document-root>
		`;
	}
}

export { ContentmentPropertyEditorUIContentSourceElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIContentSourceElement;
	}
}
