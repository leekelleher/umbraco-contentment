// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

export type ContentmentConfigurationEditorValue = {
	key: string;
	value: Record<string, unknown>;
};

export type ContentmentConfigurationEditorModel = {
	key: string;
	name: string;
	description?: string;
	icon?: string;
	group?: string;
	defaultValues?: Record<string, unknown>;
	expressions?: Record<string, unknown>;
	fields?: Array<any>;
	overlaySize?: UUIModalSidebarSize;
	[index: string]: unknown;
};

export type ContentmentListItemValue = {
	name: string;
	value: string;
	icon?: string | null;
	description?: string | null;
};

export type ContentmentListItem = ContentmentListItemValue & {
	group?: string | null;
	disabled?: boolean;
	[key: string]: unknown;
};

export type ContentmentDataListItem = ContentmentListItem;

export type ContentmentDataListOption = ContentmentDataListItem & { selected: boolean };

export type ContentmentDataListEditor = {
	propertyEditorUiAlias?: string | null;
	config?: UmbPropertyEditorConfigCollection;
};

export type ContentmentSocialLinkValue = {
	name: string;
	network: string;
	url: string;
};

export type ContentmentSocialNetworkModel = {
	network: string;
	name: string;
	url: string;
	icon: string;
	backgroundColor: string;
	iconColor: string;
};
