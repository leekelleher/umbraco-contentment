// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, property, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, tryHideLabel } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-notes';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUINotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#hideLabel: boolean = false;

	#notes?: string;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#notes = (config.getValueByAlias('notes') as unknown as any).markup;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));
	}

	connectedCallback() {
		super.connectedCallback();
		tryHideLabel(this, this.#hideLabel);
	}

	render() {
		return unsafeHTML(this.#notes);
	}

	static styles = [UmbTextStyles];
}

export { ContentmentPropertyEditorUINotesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUINotesElement;
	}
}
