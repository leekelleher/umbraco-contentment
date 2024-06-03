// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-bytes';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIBytesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _decimals = 0;

	@state()
	private _kilo = 1024;

	@property({ type: Number })
	public value: number = 0;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const decimals = Number(config.getValueByAlias('decimals'));
		this._decimals = decimals > 0 ? decimals : 2;

		const kilo = Number(config.getValueByAlias('kilo'));
		this._kilo = kilo > 0 ? kilo : 1024;
	}

	#formatBytes(bytes?: number): string {
		if (!bytes) return '0 Bytes';

		const k = this._kilo || 1024;
		const dm = this._decimals || 0;
		const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB', 'PB', 'EB', 'ZB', 'YB'];

		const i = Math.floor(Math.log(bytes) / Math.log(k));

		return Number.parseFloat((bytes / Math.pow(k, i)).toFixed(dm)) + ' ' + sizes[i];
	}

	render() {
		return this.#formatBytes(this.value);
	}
}

export { ContentmentPropertyEditorUIBytesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIBytesElement;
	}
}
