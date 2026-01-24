// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { closest } from './dom-closest.function.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

export function tryMoveBeforePropertyGroup(element: UmbPropertyEditorUiElement): void {
	if (element) {
		const umbProperty = closest('umb-property', element) as HTMLElement;
		if (!umbProperty) return;

		const uuiBox = closest('uui-box', umbProperty);
		if (!uuiBox) return;

		const umbContentWorkspaceViewEditTab = closest('umb-content-workspace-view-edit-tab', uuiBox);
		if (!umbContentWorkspaceViewEditTab) return;

		umbContentWorkspaceViewEditTab.shadowRoot?.insertBefore(element, uuiBox);
		umbProperty.style.display = 'none';
	}
}
