// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { customElement, property, state } from '@umbraco-cms/backoffice/external/lit';
import { formatBytes } from '@umbraco-cms/backoffice/utils';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-bytes')
export class ContentmentPropertyEditorUIBytesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _options = { decimals: 0, kilo: 1024 };

	@property({ type: Number })
	public value: number = 0;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		const decimals = config.getValueByAlias<{ from: number; to: number }>('decimals');
		this._options.decimals = decimals?.from && decimals.from > 0 ? decimals.from : 2;

		const kilo = Number(config.getValueByAlias('kilo'));
		this._options.kilo = kilo > 0 ? kilo : 1024;
	}

	override render() {
		return formatBytes(this.value, this._options);
	}
}

export { ContentmentPropertyEditorUIBytesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-bytes': ContentmentPropertyEditorUIBytesElement;
	}
}
