import { UmbBlockManagerContext } from '@umbraco-cms/backoffice/block';
import type {
	UmbBlockDataModel,
	UmbBlockDataObjectModel,
	UmbBlockWorkspaceOriginData,
} from '@umbraco-cms/backoffice/block';
import type {
	UmbBlockListLayoutModel,
	UmbBlockListTypeModel,
	UmbBlockListWorkspaceOriginData,
} from '@umbraco-cms/backoffice/block-list';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';

export class ContentmentContentBlocksManagerContext<
	BlockLayoutType extends UmbBlockListLayoutModel = UmbBlockListLayoutModel
> extends UmbBlockManagerContext<UmbBlockListTypeModel, BlockLayoutType, UmbBlockListWorkspaceOriginData> {
	readonly IS_CONTENTMENT = true;

	/** @deprecated Use `createWithPresets`. */
	override create(
		_contentElementTypeKey: string,
		_partialLayoutEntry?: Omit<BlockLayoutType, 'contentKey'> | undefined,
		_originData?: UmbBlockWorkspaceOriginData | undefined
	): never {
		throw new Error('Method not implemented.');
	}

	override async createWithPresets(
		contentElementTypeKey: string,
		partialLayoutEntry?: Omit<BlockLayoutType, 'contentKey'> | undefined,
		_originData?: UmbBlockWorkspaceOriginData | undefined
	): Promise<UmbBlockDataObjectModel<BlockLayoutType> | undefined> {
		return await super._createBlockData(contentElementTypeKey, partialLayoutEntry);
	}

	override insert(
		layoutEntry: BlockLayoutType,
		content: UmbBlockDataModel,
		settings: UmbBlockDataModel | undefined,
		originData: ContentmentContentBlocksWorkspaceOriginData
	): boolean {
		this._layouts.appendOneAt(layoutEntry, originData.index ?? -1);

		this.insertBlockData(layoutEntry, content, settings, originData);

		return true;
	}
}

export const CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT = new UmbContextToken<
	ContentmentContentBlocksManagerContext,
	ContentmentContentBlocksManagerContext
>(
	'UmbBlockManagerContext',
	undefined,
	(context): context is ContentmentContentBlocksManagerContext => context.IS_CONTENTMENT
);

export interface ContentmentContentBlocksWorkspaceOriginData extends UmbBlockWorkspaceOriginData {
	index: number;
}
