// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { closest } from './dom-closest.function.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

export function tryHideLabel(element: UmbPropertyEditorUiElement): void {
	if (element) {
		const umbPropertyLayout = closest('umb-property-layout', element);
		if (umbPropertyLayout) {
			umbPropertyLayout.setAttribute('orientation', 'vertical');
			const headerColumn = umbPropertyLayout.shadowRoot?.querySelector('#headerColumn') as HTMLElement;
			if (headerColumn) {
				headerColumn.style.display = 'none';
			}
		}
	}
}
