// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property()
	public value: string = '';

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	constructor() {
		super();
	}

	override render() {
		return html`'contentment-property-editor-ui-input-list'`;
	}

	static override readonly styles = [css``];
}

export { ContentmentPropertyEditorUIInputListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list': ContentmentPropertyEditorUIInputListElement;
	}
}
