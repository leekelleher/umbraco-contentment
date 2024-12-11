// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { createExtensionApi } from '@umbraco-cms/backoffice/extension-api';
import { request as __request } from '../../api/core/request.js';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { DataListService } from '../../api/sdk.gen.js';
import { OpenAPI } from '@umbraco-cms/backoffice/external/backend-api';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { UmbRepositoryBase } from '@umbraco-cms/backoffice/repository';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../../property-editor-ui/types.js';
import type { ContentmentDataSourceExtentionManifestType } from '../../extensions/data-source/data-source.extension.js';
import type { UmbApi } from '@umbraco-cms/backoffice/extension-api';
import type { UmbControllerHost, UmbControllerAlias } from '@umbraco-cms/backoffice/controller-api';
import type { UmbPropertyEditorConfig } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentDataListItem } from '../types.js';
import type { ContentmentListEditorExtentionManifestType } from '../../extensions/list-editor/list-editor.extension.js';

export class ContentmentDataListRepository extends UmbRepositoryBase implements UmbApi {
	#dataSourceLookup: Record<string, ContentmentDataSourceExtentionManifestType> = {};
	#listEditorLookup: Record<string, ContentmentListEditorExtentionManifestType> = {};

	constructor(host: UmbControllerHost, controllerAlias?: UmbControllerAlias) {
		super(host, controllerAlias);

		this.observe(umbExtensionsRegistry.byType('contentmentDataSource'), (manifests) => {
			manifests.forEach((manifest) => {
				if (manifest.api && manifest.meta?.key) {
					this.#dataSourceLookup[manifest.meta.key] = manifest;
				}
			});
		});

		this.observe(umbExtensionsRegistry.byType('contentmentListEditor'), (manifests) => {
			manifests.forEach((manifest) => {
				if (manifest.meta?.key) {
					this.#listEditorLookup[manifest.meta.key] = manifest;
				}
			});
		});
	}

	public async getEditor(
		dataSource: ContentmentConfigurationEditorValue | null | undefined,
		listEditor: ContentmentConfigurationEditorValue | null | undefined
	): Promise<ContentmentDataListEditor | undefined> {
		if (!dataSource || !listEditor) return;

		let propertyEditorUiAlias = '';
		let config: UmbPropertyEditorConfig = [];

		const dataSourceKey = dataSource.key;
		const manifest = dataSourceKey ? this.#dataSourceLookup[dataSourceKey] : null;
		if (manifest) {
			const api = await createExtensionApi(this, manifest, [this, this.controllerAlias]);
			const items = (await api?.getItems(dataSource.value)) ?? [];
			config.push({ alias: 'items', value: items });

			// NOTE: Sets `dataSource` to null, as we don't want the server to attempt to process it.
			dataSource = null;
		}

		// TODO: [LK] Implement the `listEditor` lookup (for client-side only registrations).

		const requestBody = { dataSource: dataSource, listEditor: listEditor };

		const { data } = await tryExecuteAndNotify(this, DataListService.postDataListEditor({ requestBody }));

		if (data?.propertyEditorUiAlias) {
			propertyEditorUiAlias = data.propertyEditorUiAlias;
		}

		if (data?.config) {
			config.push(...data.config);
		}

		return { propertyEditorUiAlias, config: new UmbPropertyEditorConfigCollection(config) };
	}

	public async getItemsByUrl(url: string): Promise<ContentmentDataListItem[]> {
		const { data } = await tryExecuteAndNotify(
			this,
			__request(OpenAPI, {
				method: 'GET',
				url: url,
				errors: {
					401: 'The resource is protected and requires an authentication token',
					403: 'The authenticated user do not have access to this resource',
					404: 'Not Found',
				},
			})
		);

		return data as Array<ContentmentDataListItem>;
	}
}

export { ContentmentDataListRepository as api };
