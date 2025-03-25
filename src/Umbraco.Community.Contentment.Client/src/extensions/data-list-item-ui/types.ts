// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentListItem } from '../../property-editor-ui/types.ts';

export interface ContentmentDataListItemUiElement extends HTMLElement {
	item?: ContentmentListItem;
}
