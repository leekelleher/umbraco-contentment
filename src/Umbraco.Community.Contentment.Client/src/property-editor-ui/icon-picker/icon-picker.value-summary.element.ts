// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-icon-picker-value-summary')
class ContentmentPropertyEditorUIIconPickerValueSummaryElement extends UmbValueSummaryElementBase<string | undefined> {
	override render() {
		return when(this._value, (icon) => html`<umb-icon name=${icon}></umb-icon>`);
	}
}

export { ContentmentPropertyEditorUIIconPickerValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-icon-picker-value-summary': ContentmentPropertyEditorUIIconPickerValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.IconPicker': string;
	}
}
