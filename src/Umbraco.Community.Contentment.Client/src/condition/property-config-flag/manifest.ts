// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION } from './constants.js';
import type { ManifestCondition } from '@umbraco-cms/backoffice/extension-api';

export const manifest: ManifestCondition = {
	type: 'condition',
	name: '[Contentment] Property Config Flag Condition',
	alias: CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION,
	api: () => import('./property-config-flag.condition.js'),
};
