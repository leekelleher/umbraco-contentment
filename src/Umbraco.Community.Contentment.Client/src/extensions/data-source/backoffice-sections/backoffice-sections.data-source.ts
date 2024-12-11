// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbControllerBase } from '@umbraco-cms/backoffice/class-api';
import type { ContentmentDataListItem } from '../../../property-editor-ui/types.js';
import type { ContentmentDataSourceApi } from '../types.js';
import type { ContentmentDataSourceExtentionManifestType } from '../data-source.extension.js';
import type { ManifestSection } from '@umbraco-cms/backoffice/section';
import type { UmbControllerAlias, UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class ContentmentUmbracoBackofficeSectionsDataSourceApi
	extends UmbControllerBase
	implements ContentmentDataSourceApi
{
	#sections?: Array<ManifestSection>;

	constructor(host: UmbControllerHost, controllerAlias: UmbControllerAlias) {
		super(host, controllerAlias);

		this.observe(umbExtensionsRegistry.byType('section'), (sections) => {
			this.#sections = sections;
		});
	}

	manifest?: ContentmentDataSourceExtentionManifestType;

	async getItems(): Promise<Array<ContentmentDataListItem>> {
		return (
			this.#sections?.map((section) => ({
				name: section.meta.label,
				value: section.meta.pathname,
			})) ?? []
		);
	}
}

export { ContentmentUmbracoBackofficeSectionsDataSourceApi as api };
