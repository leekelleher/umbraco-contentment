// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

const ELEMENT_NAME = 'contentment-property-editor-ui-dictionary-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDictionaryPickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Array })
	public value?: string[];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	// #onChange(event: { target: UmbInputEntityElement }) {
	// 	this.value = event.target.selection ?? [];
	// 	this.dispatchEvent(new UmbPropertyValueChangeEvent());
	// }

	render() {
		return html`<h3>${ELEMENT_NAME}</h3>`;
	}

	static styles = [css``];
}

export { ContentmentPropertyEditorUIDictionaryPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDictionaryPickerElement;
	}
}
