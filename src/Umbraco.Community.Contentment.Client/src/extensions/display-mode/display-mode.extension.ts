// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentConfigurationEditorModel } from '../../property-editor-ui/types.js';
import type { ContentmentDisplayModeElement } from './display-mode-base.element.js';
import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';

export interface ContentmentDisplayModeExtentionManifestType extends ManifestElement<ContentmentDisplayModeElement> {
	type: 'contentmentDisplayMode';
	meta?: ContentmentConfigurationEditorModel;
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentDisplayMode: ContentmentDisplayModeExtentionManifestType;
	}
}
