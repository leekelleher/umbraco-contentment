// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataListItemUiExtentionManifestType } from '../data-list-item-ui.extension.js';

export const manifest: ContentmentDataListItemUiExtentionManifestType = {
	type: 'contentmentDataListItemUi',
	alias: 'Umb.Contentment.DataListItemUi.CodeBlock',
	name: '[Contentment] Code Block Data List Item UI',
	element: () => import('./code-block.element.js'),
};
