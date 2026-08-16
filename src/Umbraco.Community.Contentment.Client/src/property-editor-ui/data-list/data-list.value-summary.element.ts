// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, nothing } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-data-list-value-summary')
export class ContentmentPropertyEditorUIDataListValueSummaryElement extends UmbValueSummaryElementBase<
	Array<string> | string | undefined
> {
	override render() {
		if (!this._value) return nothing;
		const count = Array.isArray(this._value) ? this._value.length : 1;
		return html`<uui-icon name="icon-fa-list-ul"></uui-icon> <span>&times; ${count}</span>`;
	}
}

export { ContentmentPropertyEditorUIDataListValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-list-value-summary': ContentmentPropertyEditorUIDataListValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.DataList': Array<string>;
	}
}
