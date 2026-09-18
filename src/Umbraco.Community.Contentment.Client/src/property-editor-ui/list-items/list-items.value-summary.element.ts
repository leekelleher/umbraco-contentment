// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';
import type { ContentmentListItemValue } from '../types.js';

@customElement('contentment-property-editor-ui-list-items-value-summary')
class ContentmentPropertyEditorUIListItemsValueSummaryElement extends UmbValueSummaryElementBase<
	Array<ContentmentListItemValue> | undefined
> {
	override render() {
		return when(
			this._value?.length,
			(count) => html`<uui-icon name="icon-fa-table-list"></uui-icon> <span>&times; ${count}</span>`,
		);
	}
}

export { ContentmentPropertyEditorUIListItemsValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-list-items-value-summary': ContentmentPropertyEditorUIListItemsValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.ListItems': Array<ContentmentListItemValue>;
	}
}
