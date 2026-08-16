// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { parseInt } from '../../utils/index.js';
import { customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { formatBytes } from '@umbraco-cms/backoffice/utils';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-bytes-value-summary')
export class ContentmentPropertyEditorUIBytesValueSummaryElement extends UmbValueSummaryElementBase {
	override render() {
		const value = parseInt(this._value) ?? -1;
		return html`<span>${formatBytes(value)}</span>`;
	}
}

export { ContentmentPropertyEditorUIBytesValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-bytes-value-summary': ContentmentPropertyEditorUIBytesValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.Bytes': number;
	}
}
