// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import type { ContentmentDataListItemUiElement } from '../types.js';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import { customElement, html, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';

import '../../../components/info-box/info-box.element.js';

@customElement('contentment-data-list-item-ui-info-box')
export class ContentmentDataListItemUiInfoBoxElement extends UmbLitElement implements ContentmentDataListItemUiElement {
	@property({ attribute: false })
	item?: ContentmentListItem;

	override render() {
		if (!this.item) return nothing;
		return html`
			<contentment-info-box
				.icon=${this.item.icon ?? undefined}
				.headline=${this.item.name ?? this.item.value}
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
