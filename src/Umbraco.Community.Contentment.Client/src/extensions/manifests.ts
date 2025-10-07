// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { manifests as dataListItemUis } from './data-list-item-ui/manifests.js';
import { manifests as dataSources } from './data-source/manifests.js';
import { manifests as displayModes } from './display-mode/manifests.js';
import { manifests as listEditors } from './list-editor/manifests.js';

export const manifests: Array<UmbExtensionManifest> = [
	...dataListItemUis,
	...dataSources,
	...displayModes,
	...listEditors,
];
