// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbDataTypeInputElement } from '@umbraco-cms/backoffice/data-type';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '@umbraco-cms/backoffice/data-type';

@customElement('contentment-property-editor-ui-data-type-picker')
export class ContentmentPropertyEditorUIDataTypePickerElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Array })
	public value?: Array<string>;

	public config?: UmbPropertyEditorUiElement['config'];

	#onChange(event: Event) {
		const target = event.target as UmbDataTypeInputElement;
		this.value = target.selection;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`<umb-data-type-input .selection=${this.value ?? []} @change=${this.#onChange}></umb-data-type-input>`;
	}
}

export { ContentmentPropertyEditorUIDataTypePickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-type-picker': ContentmentPropertyEditorUIDataTypePickerElement;
	}
}
