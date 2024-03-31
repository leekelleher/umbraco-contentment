/* Copyright © 2024 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

import { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

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
