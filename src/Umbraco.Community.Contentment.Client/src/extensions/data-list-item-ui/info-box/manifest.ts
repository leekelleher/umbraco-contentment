// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import type { ContentmentDataListItemUiExtentionManifestType } from '../data-list-item-ui.extension.js';

export const manifest: ContentmentDataListItemUiExtentionManifestType = {
	type: 'contentmentDataListItemUi',
	alias: 'Umb.Contentment.DataListItemUi.InfoBox',
	name: '[Contentment] Info Box Data List Item UI',
	element: () => import('./info-box.element.js'),
	meta: {
		label: 'Info Box',
		icon: 'icon-alert',
	},
};
