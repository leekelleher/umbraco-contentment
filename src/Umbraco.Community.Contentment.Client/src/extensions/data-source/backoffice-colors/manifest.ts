// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentDataSourceExtentionManifestType } from '../data-source.extension.js';

export const manifest: ContentmentDataSourceExtentionManifestType = {
	type: 'contentmentDataSource',
	alias: 'Umb.Contentment.DataSource.UmbracoBackofficeColorsDataSource',
	name: '[Contentment] Umbraco Backoffice Colors Data Source',
	api: () => import('./backoffice-colors.data-source.js'),
	meta: {
		key: 'Umb.Contentment.DataSource.UmbracoBackofficeColorsDataSource',
		name: 'Umbraco Backoffice Colors',
		description: 'Use the backoffice color palette to populate the data source.',
		icon: 'icon-palette',
		group: 'Umbraco',
	},
};
