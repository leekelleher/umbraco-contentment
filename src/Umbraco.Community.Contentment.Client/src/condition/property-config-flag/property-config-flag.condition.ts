// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean } from '../../utils/index.js';
import type { ContentmentPropertyConfigFlagConditionConfig } from './types.js';
import { UmbConditionBase } from '@umbraco-cms/backoffice/extension-registry';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbConditionControllerArguments, UmbExtensionCondition } from '@umbraco-cms/backoffice/extension-api';

export class ContentmentPropertyConfigFlagCondition
	extends UmbConditionBase<ContentmentPropertyConfigFlagConditionConfig>
	implements UmbExtensionCondition
{
	constructor(
		host: UmbControllerHost,
		args: UmbConditionControllerArguments<ContentmentPropertyConfigFlagConditionConfig>,
	) {
		super(host, args);
		const alias = this.config.propertyConfigAlias;
		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext?.config, (config) => {
				this.permitted = !!alias && parseBoolean(config?.getValueByAlias(alias));
			});
		});
	}
}

export { ContentmentPropertyConfigFlagCondition as api };
