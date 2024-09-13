// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION } from '../../condition/property-config-flag/constants.js';
import type { ManifestPropertyAction } from '@umbraco-cms/backoffice/extension-registry';

export const manifest: ManifestPropertyAction = {
	type: 'propertyAction',
	kind: 'default',
	alias: 'Umb.Contentment.PropertyAction.AllowClear',
	name: '[Contentment] Allow Clear Property Action',
	api: () => import('./allow-clear.controller.js'),
	forPropertyEditorUis: ['Umb.Contentment.PropertyEditorUi.DataList'],
	meta: {
		icon: 'icon-trash',
		label: '#buttons_clearSelection',
	},
	// @ts-ignore: Unable to extend core's `ConditionTypes` type [LK]
	conditions: [{ alias: CONTENTMENT_PROPERTY_CONFIG_FLAG_CONDITION, propertyConfigAlias: 'allowClear' }],
};
