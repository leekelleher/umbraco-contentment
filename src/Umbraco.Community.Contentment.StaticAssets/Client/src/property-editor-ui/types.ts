import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

export type ContentmentConfigurationEditorItem = {
  key: string;
  value: any;
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
