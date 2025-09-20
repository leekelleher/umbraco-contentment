// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-bytes')
export class ContentmentPropertyEditorUIBytesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _decimals = 0;

	@state()
	private _kilo = 1024;

	@property({ type: Number })
	public value: number = 0;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		const decimals = config.getValueByAlias<{ from: number; to: number }>('decimals');
		this._decimals = decimals?.from && decimals.from > 0 ? decimals.from : 2;

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

	override render() {
		return this.#formatBytes(this.value);
	}
}

export { ContentmentPropertyEditorUIBytesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-bytes': ContentmentPropertyEditorUIBytesElement;
	}
}
