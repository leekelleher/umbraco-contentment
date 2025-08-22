// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export interface ContentmentTemplatedLabelUiExtentionManifestType extends ManifestElement {
	type: 'contentmentTemplatedLabelUi';
	meta?: {
		label?: string;
		description?: string;
		icon?: string;
	};
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentTemplatedLabelUi: ContentmentTemplatedLabelUiExtentionManifestType;
	}
}
