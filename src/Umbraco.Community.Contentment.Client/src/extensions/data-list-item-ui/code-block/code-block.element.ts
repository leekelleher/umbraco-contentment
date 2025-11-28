// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentDataListItemUiElement } from '../types.js';
import { customElement, html, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

@customElement('contentment-data-list-item-ui-code-block')
export class ContentmentDataListItemUiCodeBlockElement
	extends UmbLitElement
	implements ContentmentDataListItemUiElement
{
	@property({ attribute: false })
	item?: ContentmentListItem;

	override render() {
		if (!this.item) return nothing;
		return html`<umb-code-block language="JSON" copy>${JSON.stringify(this.item, null, 2)}</umb-code-block>`;
	}
}

export { ContentmentDataListItemUiCodeBlockElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-data-list-item-ui-code-block': ContentmentDataListItemUiCodeBlockElement;
	}
}
