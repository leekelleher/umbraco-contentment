// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
	contentment: {
		name: 'Contentment',

		// Data List
		labelDataSource: 'Data source',
		labelListEditor: 'List editor',
		configureDataSource: 'Select and configure a data source',
		configureListEditor: 'Select and configure a list editor',

		// Checkbox List
		checkboxListCheckAll: 'Check all',
		checkboxListUncheckAll: 'Uncheck all',

		// List Items
		addItem: 'Add item',

		// Social Links
		configureSocialNetwork: 'Configure a social network',
		addSocialLink: 'Add social link',
		selectSocialNetwork: 'Select a social network...',
		emptySocialNetworks: 'There are no social networks to select.',
	},
	placeholders: {
		enterValue: 'Enter a value...',
	},
} as UmbLocalizationDictionary;
