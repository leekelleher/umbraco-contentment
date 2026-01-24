// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean } from '../../utils/parse-boolean.function.js';
import { css, customElement, html, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type ContentmentIconPickerElement from '../../components/icon-picker/icon-picker.element.js';
import type { IconSize } from '../../components/icon-picker/icon-picker.element.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-icon-picker')
export class ContentmentPropertyEditorUIIconPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#defaultIcon: string = '';

	#hideClearButton = false;

	#size: IconSize = 'large';

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#defaultIcon = config.getValueByAlias('defaultIcon') ?? '';
		this.#size = config.getValueByAlias<IconSize>('size') ?? 'large';
		this.#hideClearButton = parseBoolean(config.getValueByAlias('hideClearButton'));
	}

	#onChange(event: CustomEvent & { target: ContentmentIconPickerElement }) {
		if (!event.target || event.target.value === this.value) return;
		this.value = event.target.value ?? undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onClear() {
		this.value = undefined;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`
			<contentment-icon-picker
				.defaultIcon=${this.#defaultIcon}
				.size=${this.#size}
				.value=${this.value}
				@change=${this.#onChange}>
			</contentment-icon-picker>
			${when(
				!this.#hideClearButton && this.value,
				() =>
					html`<uui-button compact label=${this.localize.term('general_clear')} @click=${this.#onClear}></uui-button>`
			)}
		`;
	}

	static override styles = [
		css`
			:host {
				display: flex;
				gap: var(--uui-size-2);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIIconPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-icon-picker': ContentmentPropertyEditorUIIconPickerElement;
	}
}
