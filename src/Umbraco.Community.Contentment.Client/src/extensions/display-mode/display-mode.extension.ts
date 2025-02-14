// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ManifestElement } from '@umbraco-cms/backoffice/extension-api';
import type { ContentmentConfigurationEditorModel } from '../../property-editor-ui/types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

export interface ContentmentDisplayModeExtentionManifestType extends ManifestElement<ContentmentDisplayModeElement> {
	type: 'contentmentDisplayMode';
	meta?: ContentmentConfigurationEditorModel;
}

export interface ContentmentDisplayModeElement extends HTMLElement {
	allowAdd: boolean;
	allowEdit: boolean;
	allowRemove: boolean;
	allowSort: boolean;
	items?: unknown;
	config?: UmbPropertyEditorConfigCollection;
	destroy?: () => void;
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentDisplayMode: ContentmentDisplayModeExtentionManifestType;
	}
}
