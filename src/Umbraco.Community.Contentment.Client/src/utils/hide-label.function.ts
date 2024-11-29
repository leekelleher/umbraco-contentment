// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

export function tryHideLabel(element: UmbPropertyEditorUiElement, hideLabel: boolean): void {
	if (hideLabel) {
		// HACK: Distinguished guests and honourable friends, may I present the mother of old skool DOM hacks! [LK]
		const umbPropertyLayout = element.parentElement?.parentElement;
		if (umbPropertyLayout) {
			umbPropertyLayout.setAttribute('orientation', 'vertical');
			const headerColumn = umbPropertyLayout.shadowRoot?.querySelector('#headerColumn') as HTMLElement;
			if (headerColumn) {
				headerColumn.style.display = 'none';
			}
		}
	}
}
