import { CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT } from './content-blocks-manager.context.js';
import type { ContentmentContentBlocksWorkspaceOriginData } from './content-blocks-manager.context.js';
import { UMB_BLOCK_CATALOGUE_MODAL, UmbBlockEntriesContext } from '@umbraco-cms/backoffice/block';
import { UmbBooleanState } from '@umbraco-cms/backoffice/observable-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import { UmbModalRouteRegistrationController } from '@umbraco-cms/backoffice/router';
import type {
	UmbBlockDataModel,
	UmbBlockDataObjectModel,
	UmbBlockLayoutBaseModel,
	UmbBlockWorkspaceOriginData,
} from '@umbraco-cms/backoffice/block';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import {
	UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS,
	UMB_BLOCK_LIST_WORKSPACE_MODAL,
	type UmbBlockListLayoutModel,
	type UmbBlockListTypeModel,
	type UmbBlockListValueModel,
	type UmbBlockListWorkspaceOriginData,
} from '@umbraco-cms/backoffice/block-list';

export class ContentmentContentBlocksEntriesContext extends UmbBlockEntriesContext<
	typeof CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT,
	typeof CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT.TYPE,
	UmbBlockListTypeModel,
	UmbBlockListLayoutModel,
	UmbBlockListWorkspaceOriginData
> {
	readonly IS_CONTENTMENT = true;

	public readonly canCreate = new UmbBooleanState(true).asObservable();

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT);

		new UmbModalRouteRegistrationController(this, UMB_BLOCK_CATALOGUE_MODAL)
			.addAdditionalPath('_catalogue/:view/:index')
			.onSetup(async (routingInfo) => {
				await this._retrieveManager;
				if (!this._manager) return false;
				const index = routingInfo.index ? parseInt(routingInfo.index) : -1;

				return {
					data: {
						blocks: this._manager?.getBlockTypes() ?? [],
						blockGroups: [],
						originData: { index: index },
						createBlockInWorkspace: true,
					},
				};
			})
			.onSubmit(async (value, data) => {
				if (value?.create && data) {
					const created = await this.create(
						value.create.contentElementTypeKey,
						{},
						data.originData as ContentmentContentBlocksWorkspaceOriginData
					);
					if (created) {
						this.insert(
							created.layout,
							created.content,
							created.settings,
							data.originData as ContentmentContentBlocksWorkspaceOriginData
						);
					} else {
						throw new Error('Failed to create block');
					}
				}
			})
			.observeRouteBuilder((routeBuilder) => {
				this._catalogueRouteBuilderState.setValue(routeBuilder);
			});

		new UmbModalRouteRegistrationController(this, UMB_BLOCK_LIST_WORKSPACE_MODAL)
			.addAdditionalPath('block')
			.onSetup(() => {
				return {
					data: { entityType: 'block', preset: {}, baseDataPath: this._dataPath },
					modal: { size: 'medium' },
				};
			})
			.observeRouteBuilder((routeBuilder) => {
				const newPath = routeBuilder({});
				this._workspacePath.setValue(newPath);
			});
	}

	protected override _gotBlockManager(): void {
		if (!this._manager) return;
		this.observe(this._manager.layouts, (layouts) => this._layoutEntries.setValue(layouts), 'observeParentLayouts');
		this.observe(this.layoutEntries, (layouts) => this._manager?.setLayouts(layouts), 'observeThisLayouts');
	}

	override getPathForCreateBlock(index: number): string | undefined {
		return this._catalogueRouteBuilderState.getValue()?.({ view: 'create', index: index });
	}

	override getPathForClipboard(index: number): string | undefined {
		return this._catalogueRouteBuilderState.getValue()?.({ view: 'clipboard', index: index });
	}

	override async setLayouts(layouts: Array<UmbBlockListLayoutModel>) {
		await this._retrieveManager;
		this._manager?.setLayouts(layouts);
	}

	override async create(
		contentElementTypeKey: string,
		layoutEntry?: Omit<UmbBlockLayoutBaseModel, 'contentKey'> | undefined,
		originData?: UmbBlockWorkspaceOriginData | undefined
	): Promise<UmbBlockDataObjectModel<UmbBlockLayoutBaseModel> | undefined> {
		await this._retrieveManager;
		return await this._manager?.createWithPresets(contentElementTypeKey, layoutEntry, originData);
	}

	override async insert(
		layoutEntry: UmbBlockLayoutBaseModel,
		content: UmbBlockDataModel,
		settings: UmbBlockDataModel | undefined,
		originData: ContentmentContentBlocksWorkspaceOriginData
	): Promise<boolean> {
		await this._retrieveManager;
		return this._manager?.insert(layoutEntry, content, settings, originData) ?? false;
	}

	protected override async _insertFromPropertyValue(
		value: UmbBlockListValueModel,
		originData: UmbBlockListWorkspaceOriginData
	): Promise<UmbBlockListWorkspaceOriginData> {
		const layoutEntries = value.layout[UMB_BLOCK_LIST_PROPERTY_EDITOR_SCHEMA_ALIAS];
		if (!layoutEntries) {
			throw new Error('No layout entries found');
		}

		await Promise.all(
			layoutEntries.map(async (layoutEntry) => {
				this._insertBlockFromPropertyValue(layoutEntry, value, originData);
				if (originData.index !== -1) {
					originData = { ...originData, index: originData.index + 1 };
				}
			})
		);

		return originData;
	}
}

export const CONTENTMENT_CONTENT_BLOCKS_ENTRIES_CONTEXT = new UmbContextToken<ContentmentContentBlocksEntriesContext>(
	'UmbBlockEntriesContext',
	undefined,
	(context): context is ContentmentContentBlocksEntriesContext => context.IS_CONTENTMENT
);
