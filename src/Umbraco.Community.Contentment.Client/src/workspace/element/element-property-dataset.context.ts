// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { of } from '@umbraco-cms/backoffice/external/rxjs';
import { UmbElementPropertyDatasetContext } from '@umbraco-cms/backoffice/content';
import type { ContentmentElementDataModel } from './types.js';
import type { ContentmentElementManager } from './element-manager.context.js';
import type { UmbContentTypeModel } from '@umbraco-cms/backoffice/content-type';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbPropertyDatasetContext } from '@umbraco-cms/backoffice/property';
import type { UmbVariantId } from '@umbraco-cms/backoffice/variant';

export class ContentmentElementPropertyDatasetContext
	extends UmbElementPropertyDatasetContext<ContentmentElementDataModel, UmbContentTypeModel, ContentmentElementManager>
	implements UmbPropertyDatasetContext
{
	// Shown in the property dataset header (the element type name)
	readonly name = this._dataOwner.structure.ownerContentTypeObservablePart((x) => x?.name);
	// Invariant-only: culture and segment are always null
	readonly culture = of<string | null>(null);
	readonly segment = of<string | null>(null);

	constructor(host: UmbControllerHost, manager: ContentmentElementManager, variantId?: UmbVariantId) {
		super(host, manager, variantId);
	}

	getName(): string | undefined {
		return this._dataOwner.structure.getOwnerContentType()?.name;
	}
}
