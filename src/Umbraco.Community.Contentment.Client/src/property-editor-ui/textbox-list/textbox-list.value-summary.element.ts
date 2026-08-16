// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';

@customElement('contentment-property-editor-ui-textbox-list-value-summary')
class ContentmentPropertyEditorUITextboxListValueSummaryElement extends UmbValueSummaryElementBase<
	Record<string, string> | undefined
> {
	override render() {
		const values = this._value ? Object.values(this._value).filter(Boolean) : [];
		return when(values.length, () => html`<span>${values.join(', ')}</span>`);
	}
}

export { ContentmentPropertyEditorUITextboxListValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-textbox-list-value-summary': ContentmentPropertyEditorUITextboxListValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.TextboxList': Record<string, string>;
	}
}
