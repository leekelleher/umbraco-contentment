// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { umbracoColors } from '@umbraco-cms/backoffice/resources';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentDataSourceApi } from '../types.js';

export class ContentmentUmbracoBackofficeColorsDataSourceApi
	extends UmbControllerBase
	implements ContentmentDataSourceApi
{
	async getItems(): Promise<Array<ContentmentListItem>> {
		return (
			umbracoColors?.map((color) => ({
				name: color.alias,
				value: color.varName,
			})) ?? []
		);
	}
}

export { ContentmentUmbracoBackofficeColorsDataSourceApi as api };
