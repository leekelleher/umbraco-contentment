// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { createExtensionApi } from '@umbraco-cms/backoffice/extension-api';
import { tryExecute } from '@umbraco-cms/backoffice/resources';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { umbHttpClient } from '@umbraco-cms/backoffice/http-client';
import { DataListService } from '../../api/sdk.gen.js';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { UmbRepositoryBase } from '@umbraco-cms/backoffice/repository';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../../property-editor-ui/types.js';
import type { ContentmentDataSourceExtentionManifestType } from '../../extensions/data-source/data-source.extension.js';
import type { UmbApi } from '@umbraco-cms/backoffice/extension-api';
import type { UmbControllerHost, UmbControllerAlias } from '@umbraco-cms/backoffice/controller-api';
import type { UmbPropertyEditorConfig } from '@umbraco-cms/backoffice/property-editor';
import type { ContentmentListItem } from '../types.js';
//import type { ContentmentListEditorExtentionManifestType } from '../../extensions/list-editor/list-editor.extension.js';

export class ContentmentDataListRepository extends UmbRepositoryBase implements UmbApi {
	#clientSideDataSourceLookup: Map<string, ContentmentDataSourceExtentionManifestType> = new Map();
	//#clientSideListEditorLookup: Map<string, ContentmentListEditorExtentionManifestType> = new Map();

	constructor(host: UmbControllerHost, controllerAlias?: UmbControllerAlias) {
		super(host, controllerAlias);

		this.observe(umbExtensionsRegistry.byType('contentmentDataSource'), (manifests) => {
			manifests.forEach((manifest) => {
				if (manifest.api && manifest.meta?.key) {
					this.#clientSideDataSourceLookup.set(manifest.meta.key, manifest);
				}
			});
		});

		// this.observe(umbExtensionsRegistry.byType('contentmentListEditor'), (manifests) => {
		// 	manifests.forEach((manifest) => {
		// 		if (manifest.meta?.key) {
		// 			this.#clientSideListEditorLookup.set(manifest.meta.key, manifest);
		// 		}
		// 	});
		// });
	}

	public async getEditor(
		dataSource: ContentmentConfigurationEditorValue | null | undefined,
		listEditor: ContentmentConfigurationEditorValue | null | undefined,
		entityUnique?: string | null,
		propertyAlias?: string | null,
		variantId?: string | null
	): Promise<ContentmentDataListEditor | undefined> {
		if (!dataSource || !listEditor) return;

		let propertyEditorUiAlias = '';
		let config: UmbPropertyEditorConfig = [];

		const clientSideDataSourceManifest = this.#clientSideDataSourceLookup.get(dataSource.key);
		if (clientSideDataSourceManifest) {
			const api = await createExtensionApi(this, clientSideDataSourceManifest, [this, this.controllerAlias]);
			const items = (await api?.getItems(dataSource.value)) ?? [];
			config.push({ alias: 'items', value: items });

			// NOTE: Sets `dataSource` to null, as we don't want the server to attempt to process it.
			dataSource = null;
		}

		// TODO: [LK] Implement the `listEditor` lookup (for client-side only registrations).

		const body = {
			alias: propertyAlias,
			dataSource,
			id: entityUnique,
			listEditor,
			variant: variantId,
		};

		const { data } = await tryExecute(this, DataListService.postDataListEditor({ client: umbHttpClient, body }));

		if (data?.propertyEditorUiAlias) {
			propertyEditorUiAlias = data.propertyEditorUiAlias;
		}

		if (data?.config) {
			config.push(...data.config);
		}

		return { propertyEditorUiAlias, config: new UmbPropertyEditorConfigCollection(config) };
	}

	public async getItemsByUrl(url: string): Promise<ContentmentListItem[]> {
		// NOTE: The security needs to be set for the request, otherwise it returns a 401 unauthorized error! [LK]
		const { data } = await tryExecute(this, umbHttpClient.get({ security: [{ scheme: 'bearer', type: 'http' }], url }));

		return data as Array<ContentmentListItem>;
	}
}

export { ContentmentDataListRepository as api };
