import { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

export type ContentmentConfigurationEditorValue = {
	key: string;
	value: any;
};

export type ContentmentConfigurationEditorModel = {
	key: string;
	name: string;
	description?: string;
	icon?: string;
	group?: string;
	defaultValues?: Record<string, unknown>;
	expressions?: Record<string, string>;
	fields?: Array<any>;
	overlaySize?: UUIModalSidebarSize;
};

export type ContentmentDataListItem = {
	name: string;
	value: string;
	description?: string;
	icon?: string;
};

export type ContentmentDataListEditor = {
	propertyEditorUiAlias?: string | null;
	config?: UmbPropertyEditorConfigCollection;
};
