// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import type { ContentmentDataListItemUiElement } from '../types.js';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import { customElement, html, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

@customElement('contentment-data-list-item-ui-info-box')
export class ContentmentDataListItemUiInfoBoxElement extends UmbLitElement implements ContentmentDataListItemUiElement {
	@property({ attribute: false })
	item?: ContentmentListItem;

	override render() {
		if (!this.item) return nothing;
		return html`
			<contentment-info-box
				type="transparent"
				.icon=${this.item.icon ?? undefined}
				.heading=${this.item.name ?? this.item.value}
				.message=${this.item.description ?? undefined}></contentment-info-box>
		`;
	}
}

export { ContentmentDataListItemUiInfoBoxElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-data-list-item-ui-info-box': ContentmentDataListItemUiInfoBoxElement;
	}
}
