// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { customElement, html } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentDisplayModeElement } from '../display-mode-base.element.js';

import '../../../components/sortable-list/sortable-list.element.js';

@customElement('contentment-property-editor-ui-blocks')
export class ContentmentPropertyEditorUIBlocksElement extends ContentmentDisplayModeElement {
	override render() {
		return html`${this.tagName}`;
	}
}

export { ContentmentPropertyEditorUIBlocksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-blocks': ContentmentPropertyEditorUIBlocksElement;
	}
}
