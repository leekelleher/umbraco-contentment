// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestBase } from '@umbraco-cms/backoffice/extension-api';
import type { ContentmentConfigurationEditorModel } from '../../property-editor-ui/types.js';

export interface ContentmentDisplayModeExtentionManifestType extends ManifestBase {
	type: 'contentmentDisplayMode';
	meta?: ContentmentConfigurationEditorModel;
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentDisplayMode: ContentmentDisplayModeExtentionManifestType;
	}
}
