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

export type ContentmentDataListItem = {
	name: string;
	value: string;
	description?: string;
	icon?: string;
	group?: string;
	disabled?: boolean;
	[alias: string]: unknown;
};

export type ContentmentDataListOption = ContentmentDataListItem & { selected: boolean };

export type ContentmentDataListEditor = {
	propertyEditorUiAlias?: string | null;
	config?: UmbPropertyEditorConfigCollection;
};

export type ContentmentListItemValue = {
	name: string;
	value: string;
	icon?: string;
	description?: string;
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
