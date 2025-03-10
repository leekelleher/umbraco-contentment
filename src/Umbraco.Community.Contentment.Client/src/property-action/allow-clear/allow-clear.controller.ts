// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbActionBase } from '@umbraco-cms/backoffice/action';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyAction } from '@umbraco-cms/backoffice/property-action';

export class ContentmentPropertyActionAllowClearElement<ArgsMetaType = never>
	extends UmbActionBase<ArgsMetaType>
	implements UmbPropertyAction<ArgsMetaType>
{
	override args: any;

	async getHref(): Promise<string | undefined> {
		return Promise.resolve(undefined);
	}

	async execute(): Promise<void> {
		const propertyContext = await this.getContext(UMB_PROPERTY_CONTEXT);

		propertyContext.clearValue();

		this.dispatchEvent(new UmbChangeEvent());

		return Promise.resolve();
	}
}

export { ContentmentPropertyActionAllowClearElement as api };
