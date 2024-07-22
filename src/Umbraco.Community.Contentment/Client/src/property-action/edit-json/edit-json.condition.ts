// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbConditionBase } from '@umbraco-cms/backoffice/extension-registry';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbConditionConfigBase } from '@umbraco-cms/backoffice/extension-api';
import type { UmbConditionControllerArguments, UmbExtensionCondition } from '@umbraco-cms/backoffice/extension-api';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';

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

export const CONTENTMENT_DEVELOPER_MODE_CONDITION = 'Umb.Contentment.Condition.DeveloperMode';
