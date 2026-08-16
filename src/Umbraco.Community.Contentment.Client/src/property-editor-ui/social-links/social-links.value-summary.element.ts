// SPDX-License-Identifier: MIT
// Copyright © 2026 Lee Kelleher

import { customElement, html, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbValueSummaryElementBase } from '@umbraco-cms/backoffice/value-summary';
import type { ContentmentSocialLinkValue } from '../types.js';

@customElement('contentment-property-editor-ui-social-links-value-summary')
class ContentmentPropertyEditorUISocialLinksValueSummaryElement extends UmbValueSummaryElementBase<
	Array<ContentmentSocialLinkValue> | undefined
> {
	override render() {
		return when(
			this._value?.length,
			(count) => html`<uui-icon name="icon-molecular-network"></uui-icon> <span>&times; ${count}</span>`,
		);
	}
}

export { ContentmentPropertyEditorUISocialLinksValueSummaryElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-social-links-value-summary': ContentmentPropertyEditorUISocialLinksValueSummaryElement;
	}
	interface UmbValueTypeMap {
		'Umbraco.Community.Contentment.SocialLinks': Array<ContentmentSocialLinkValue>;
	}
}
