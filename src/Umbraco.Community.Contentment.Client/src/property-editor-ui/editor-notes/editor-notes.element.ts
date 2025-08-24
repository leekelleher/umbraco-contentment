// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, tryHideLabel, tryMoveBeforePropertyGroup } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentInfoBoxElementType } from '../../components/info-box/info-box.element.js';
import type { PropertyValues } from '@umbraco-cms/backoffice/external/lit';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-editor-notes')
export class ContentmentPropertyEditorUIEditorNotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#hideLabel: boolean = false;

	#hidePropertyGroup: boolean = false;

	#alertType?: ContentmentInfoBoxElementType;

	#icon?: string;

	#heading?: string;

	#message?: string;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#alertType = config.getValueByAlias<ContentmentInfoBoxElementType>('alertType') || 'default';
		this.#icon = config.getValueByAlias('icon');
		this.#heading = config.getValueByAlias('heading');
		const message = config.getValueByAlias('message');
		this.#message = (message as unknown as any)?.markup ?? message;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
		this.#hidePropertyGroup = parseBoolean(config.getValueByAlias('hidePropertyGroup'));

		if (this.#icon) {
			// NOTE: To workaround the `color-text` part of the value. [LK]
			this.#icon = this.#icon.split(' ')[0];
		}
	}

	protected override firstUpdated(_changedProperties: PropertyValues): void {
		super.firstUpdated(_changedProperties);

		tryHideLabel(this, this.#hideLabel);
		tryMoveBeforePropertyGroup(this, this.#hidePropertyGroup);
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
		'contentment-property-editor-ui-editor-notes': ContentmentPropertyEditorUIEditorNotesElement;
	}
}
