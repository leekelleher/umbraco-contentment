// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';
import type { ContentmentContentBlockValue } from '../types.js';

@customElement('contentment-property-editor-ui-content-blocks-value-summary')
export class ContentmentPropertyEditorUIContentBlocksValueSummaryElement extends UmbValueSummaryElementBase<
	Array<ContentmentContentBlockValue> | undefined
> {
	override render() {
		return when(
			this._value?.length,
			(count) => html`<uui-icon name="icon-fa-server"></uui-icon> <span>&times; ${count}</span>`,
		);
	}
}

export { ContentmentPropertyEditorUIContentBlocksValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-blocks-value-summary': ContentmentPropertyEditorUIContentBlocksValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.ContentBlocks': Array<ContentmentContentBlockValue>;
	}
}
