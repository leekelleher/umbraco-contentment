// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { closest } from './dom-closest.function.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

export function tryMoveBeforePropertyGroup(element: UmbPropertyEditorUiElement): void {
	if (element) {
		const umbProperty = closest('umb-property', element) as HTMLElement;
		if (!umbProperty) return;

		const uuiBox = closest('uui-box', umbProperty) as HTMLElement;
		if (!uuiBox) return;

		const umbContentWorkspaceViewEditTab = closest('umb-content-workspace-view-edit-tab', uuiBox);
		if (!umbContentWorkspaceViewEditTab) return;

		umbContentWorkspaceViewEditTab.shadowRoot?.insertBefore(element, uuiBox);

		// Hide the outermost per-property wrapper (not just `umb-property`), so its divider is hidden too.
		const workspaceProperty = closest('umb-content-workspace-property', umbProperty) as HTMLElement;
		(workspaceProperty ?? umbProperty).style.display = 'none';
		if (!workspaceProperty) return;

		// `umb-property` sits behind several shadow roots below `uui-box`, so `querySelectorAll` can't see it from here.
		// Query siblings from the wrapper's own parent instead, and hide the box once none remain visible.
		const workspaceProperties = workspaceProperty.parentNode?.querySelectorAll('umb-content-workspace-property') ?? [];
		const hasVisibleProperty = Array.from(workspaceProperties).some(
			(property) => (property as HTMLElement).style.display !== 'none',
		);

		if (!hasVisibleProperty) {
			uuiBox.style.display = 'none';
		}
	}
}
