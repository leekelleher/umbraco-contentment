// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import type { ContentmentListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentDataSourceApi } from '../types.js';

export class ContentmentUmbracoBackofficeSectionsDataSourceApi
	extends UmbControllerBase
	implements ContentmentDataSourceApi
{
	async getItems(): Promise<Array<ContentmentListItem>> {
		const sections = await this.observe(umbExtensionsRegistry.byType('section')).asPromise();
		return (
			sections.map((section) => ({
				name: section.meta.label,
				value: section.meta.pathname,
			})) ?? []
		);
	}
}

export { ContentmentUmbracoBackofficeSectionsDataSourceApi as api };
