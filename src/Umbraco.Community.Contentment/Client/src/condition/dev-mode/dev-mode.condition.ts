// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_DEVELOPER_MODE_CONDITION } from './constants.js';
import { UmbConditionBase } from '@umbraco-cms/backoffice/extension-registry';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbConditionConfigBase } from '@umbraco-cms/backoffice/extension-api';
import type { UmbConditionControllerArguments, UmbExtensionCondition } from '@umbraco-cms/backoffice/extension-api';

export class ContentmentDeveloperModeCondition
	extends UmbConditionBase<ContentmentDeveloperModeConditionConfig>
	implements UmbExtensionCondition
{
	constructor(host: UmbControllerHost, args: UmbConditionControllerArguments<ContentmentDeveloperModeConditionConfig>) {
		super(host, args);
		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext.config, (config) => {
				this.permitted = Boolean(config?.getValueByAlias('enableDevMode'));
			});
		});
	}
}

export { ContentmentDeveloperModeCondition as api };

export type ContentmentDeveloperModeConditionConfig = UmbConditionConfigBase<
	typeof CONTENTMENT_DEVELOPER_MODE_CONDITION
>;
