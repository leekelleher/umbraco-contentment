// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
	contentment: {
		// General
		removeItemHeadline: (name: string | null | undefined) => `Remove ${name || 'item'}?`,
		removeItemMessage: 'Are you sure you want to remove this item?',
		removeItemButton: 'Yes, remove',

		// Configuration Editor
		missingItemName: 'This item is no longer available',
		missingItemDescription: 'Please remove this configuration and select another item.',

		// Content Blocks
		labelDisplayMode: 'Display mode',
		configureDisplayMode: 'Select and configure a display mode',
		configureElementType: 'Select and configure an element type',
		missingElementType: 'This content is not supported for this configuration.',
		copyBlock: 'Copy content block',
		copyBlocks: 'Copy all blocks',

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
		changeSocialNetworkTo: (label: string | null | undefined) => `Change social network to ${label || 'this'}?`,

		// Property Actions
		editJson: 'Edit raw value',
	},
	placeholders: {
		enterValue: 'Enter a value...',
	},
} as UmbLocalizationDictionary;
