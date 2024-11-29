// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, tryHideLabel } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';
import type { UUIInterfaceColor } from '@umbraco-cms/backoffice/external/uui';

import '../../components/info-box/info-box.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-editor-notes';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIEditorNotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#hideLabel: boolean = false;

	#alertType?: UUIInterfaceColor;

	#icon?: string;

	#heading?: string;

	#message?: string;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#alertType = config.getValueByAlias('alertType') || 'default';
		this.#icon = config.getValueByAlias('icon');
		this.#heading = config.getValueByAlias('heading');
		const message = config.getValueByAlias('message');
		this.#message = (message as unknown as any)?.markup ?? message;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));

		if (this.#icon) {
			// HACK: To workaround the `color-text` part of the value. [LK]
			this.#icon = this.#icon.split(' ')[0];
		}
	}

	override connectedCallback() {
		super.connectedCallback();
		tryHideLabel(this, this.#hideLabel);
	}

	override render() {
		return html`
			<contentment-info-box
				.type=${this.#alertType}
				.icon=${this.#icon}
				.heading=${this.#heading}
				.message=${this.#message}>
			</contentment-info-box>
		`;
	}
}

export { ContentmentPropertyEditorUIEditorNotesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIEditorNotesElement;
	}
}
