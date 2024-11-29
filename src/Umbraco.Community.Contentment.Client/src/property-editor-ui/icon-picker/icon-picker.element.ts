// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/property-editor-ui/property-editor-ui.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-icon-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIIconPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _config?: UmbPropertyEditorConfigCollection;

	@state()
	private _defaultIcon: string = '';

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this._defaultIcon = config.getValueByAlias('defaultIcon') ?? '';
		this._config = new UmbPropertyEditorConfigCollection(config.filter((x) => x.alias !== 'defaultIcon'));
	}

	#onChange(event: UmbPropertyValueChangeEvent & { target: UmbPropertyEditorUiElement }) {
		var element = event.target;
		if (!element || element.value === this.value) return;
		this.value = element.value as any;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`
			<contentment-property-editor-ui
				property-editor-ui-alias="Umb.PropertyEditorUi.IconPicker"
				.config=${this._config}
				.value=${this.value || this._defaultIcon}
				@change=${this.#onChange}>
			</contentment-property-editor-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIIconPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIIconPickerElement;
	}
}
