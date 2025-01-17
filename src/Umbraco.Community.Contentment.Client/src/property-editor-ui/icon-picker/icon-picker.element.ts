// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentIconPickerElement, IconSize } from '../../components/icon-picker/icon-picker.element.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/icon-picker/icon-picker.element.js';

@customElement('contentment-property-editor-ui-icon-picker')
export class ContentmentPropertyEditorUIIconPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _defaultIcon: string = '';

	@state()
	private _size: IconSize = 'large';

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._defaultIcon = config.getValueByAlias('defaultIcon') ?? '';
		this._size = config.getValueByAlias<IconSize>('size') ?? 'large';
	}

	#onChange(event: CustomEvent & { target: ContentmentIconPickerElement }) {
		if (!event.target || event.target.value === this.value) return;
		this.value = event.target.value;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`
			<contentment-icon-picker
				.defaultIcon=${this._defaultIcon}
				.size=${this._size}
				.value=${this.value}
				@change=${this.#onChange}>
			</contentment-icon-picker>
		`;
	}
}

export { ContentmentPropertyEditorUIIconPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-icon-picker': ContentmentPropertyEditorUIIconPickerElement;
	}
}
