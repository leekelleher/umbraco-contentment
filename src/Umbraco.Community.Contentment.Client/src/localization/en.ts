// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbLocalizationDictionary } from '@umbraco-cms/backoffice/localization-api';

export default {
	contentment: {
		title: 'Contentment',

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

		// Input List
		addColumn: 'Add column',
		enterLabel: 'Enter a label...',

		// Property Actions
		editJson: 'Edit raw value',

		// Dashboard
		dashboard_links_headline: 'Useful links',
		dashboard_links_documentation_label: 'Documentation',
		dashboard_links_documentation_description: 'How to use each of the property editors.',
		dashboard_links_video_label: 'Video demonstrations',
		dashboard_links_video_description: 'Demos, guides and tutorials on YouTube.',
		dashboard_links_support_label: 'Support forum',
		dashboard_links_support_description: 'Ask for help, the community is your friend.',
		dashboard_links_source_label: 'Source code',
		dashboard_links_source_description: 'See the code, all free and open-source.',
		dashboard_links_issues_label: 'Issue tracker',
		dashboard_links_issues_description: 'Found a bug? Suggest a feature? Let me know.',
		dashboard_links_license_label: 'License',
		dashboard_links_license_description: 'Licensed under the MIT License.',

		dashboard_features_headline: 'Feature options',
		dashboard_sponsorship_headline: 'Sponsor continued development',
	},
	placeholders: {
		enterValue: 'Enter a value...',
	},
} as UmbLocalizationDictionary;
