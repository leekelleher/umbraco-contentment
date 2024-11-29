// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export interface ContentmentDataListItemUiExtentionManifestType extends ManifestElement {
	type: 'contentmentDataListItemUi';
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentDataListItemUi: ContentmentDataListItemUiExtentionManifestType;
	}
}
