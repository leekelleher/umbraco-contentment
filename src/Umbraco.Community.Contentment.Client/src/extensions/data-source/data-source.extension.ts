// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestApi } from '@umbraco-cms/backoffice/extension-api';
import type { ContentmentConfigurationEditorModel } from '../../property-editor-ui/types.js';
import type { ContentmentDataSourceApi } from './types.js';

export interface ContentmentDataSourceExtentionManifestType extends ManifestApi<ContentmentDataSourceApi> {
	type: 'contentmentDataSource';
	meta?: ContentmentConfigurationEditorModel;
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentDataSource: ContentmentDataSourceExtentionManifestType;
	}
}
