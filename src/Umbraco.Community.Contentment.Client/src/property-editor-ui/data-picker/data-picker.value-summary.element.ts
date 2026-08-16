// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-data-picker-value-summary')
export class ContentmentPropertyEditorUIDataPickerValueSummaryElement extends UmbValueSummaryElementBase<
	Array<string> | undefined
> {
	override render() {
		return when(
			this._value?.length,
			(count) => html`<uui-icon name="icon-fa-arrow-pointer"></uui-icon> <span>&times; ${count}</span>`,
		);
	}
}

export { ContentmentPropertyEditorUIDataPickerValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-picker-value-summary': ContentmentPropertyEditorUIDataPickerValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.DataPicker': Array<string>;
	}
}
