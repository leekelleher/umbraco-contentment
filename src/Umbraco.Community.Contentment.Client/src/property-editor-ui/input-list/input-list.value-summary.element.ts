// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-input-list-value-summary')
class ContentmentPropertyEditorUIInputListValueSummaryElement extends UmbValueSummaryElementBase<
	Array<unknown> | undefined
> {
	override render() {
		return when(
			this._value?.length,
			(count) => html`<uui-icon name="icon-list"></uui-icon> <span>&times; ${count}</span>`,
		);
	}
}

export { ContentmentPropertyEditorUIInputListValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list-value-summary': ContentmentPropertyEditorUIInputListValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.InputList': Array<unknown>;
	}
}
