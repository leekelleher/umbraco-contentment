// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION } from './constants.js';
import type { UmbConditionConfigBase } from '@umbraco-cms/backoffice/extension-api';

export type ContentmentPropertyConfigFlagConditionConfig = UmbConditionConfigBase<
	typeof CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION
> & {
	propertyConfigAlias?: string;
};

declare global {
	interface UmbExtensionConditionConfigMap {
		ContentmentPropertyConfigFlagConditionConfig: ContentmentPropertyConfigFlagConditionConfig;
	}
}
