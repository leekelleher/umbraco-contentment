// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

export function tryMoveBeforePropertyGroup(element: UmbPropertyEditorUiElement, attempt: boolean): void {
	if (attempt) {
		// HACK: Members of the jury, I present to you, yet another dirty DOM hack! [LK]

		// @ts-ignore
		const umbProperty = element.getRootNode()?.host as HTMLElement;

		// @ts-ignore
		const uuiBox = umbProperty?.getRootNode()?.host?.getRootNode()?.host?.parentElement as HTMLElement;

		// @ts-ignore
		const umbContentWorkspaceViewEditTab = uuiBox?.getRootNode().host as HTMLElement;

		if (umbProperty && uuiBox && umbContentWorkspaceViewEditTab) {
			umbContentWorkspaceViewEditTab.shadowRoot?.insertBefore(element, uuiBox);
			umbProperty.style.display = 'none';
		}
	}
}
