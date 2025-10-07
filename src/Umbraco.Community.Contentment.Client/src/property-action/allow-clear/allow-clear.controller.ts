// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { UmbPropertyActionBase } from '@umbraco-cms/backoffice/property-action';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { MetaPropertyActionDefaultKind } from '@umbraco-cms/backoffice/property-action';

export class ContentmentPropertyActionAllowClearElement extends UmbPropertyActionBase<MetaPropertyActionDefaultKind> {
	override async execute() {
		const propertyContext = await this.getContext(UMB_PROPERTY_CONTEXT);
		propertyContext?.clearValue();
	}
}

export { ContentmentPropertyActionAllowClearElement as api };
