// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataSourceExtentionManifestType } from '../data-source.extension.js';

export const manifest: ContentmentDataSourceExtentionManifestType = {
	type: 'contentmentDataSource',
	alias: 'Umb.Contentment.DataSource.UmbracoBackofficeSectionsDataSource',
	name: '[Contentment] Umbraco Backoffice Sections Data Source',
	api: () => import('./backoffice-sections.data-source.js'),
	meta: {
		key: 'Umbraco.Community.Contentment.DataEditors.UmbracoBackofficeSectionsDataSource, Umbraco.Community.Contentment',
		name: 'Umbraco Backoffice Sections',
		description: 'Use the backoffice sections to populate the data source.',
		icon: 'icon-app',
		group: 'Umbraco',
	},
};
