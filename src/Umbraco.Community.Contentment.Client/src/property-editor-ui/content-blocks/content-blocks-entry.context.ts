import { CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT } from './content-blocks-manager.context.js';
import { CONTENTMENT_CONTENT_BLOCKS_ENTRIES_CONTEXT } from './content-blocks-entries.context.js';
import { UmbBlockEntryContext } from '@umbraco-cms/backoffice/block';
import { UmbBooleanState } from '@umbraco-cms/backoffice/observable-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';

export class ContentmentContentBlocksEntryContext extends UmbBlockEntryContext<
	typeof CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT,
	typeof CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT.TYPE,
	typeof CONTENTMENT_CONTENT_BLOCKS_ENTRIES_CONTEXT,
	typeof CONTENTMENT_CONTENT_BLOCKS_ENTRIES_CONTEXT.TYPE
> {
	readonly IS_CONTENTMENT = true;

	readonly actionsVisibility = new UmbBooleanState(true).asObservable();
	public readonly showContentEdit = new UmbBooleanState(true).asObservable();

	constructor(host: UmbControllerHost) {
		super(host, CONTENTMENT_CONTENT_BLOCKS_MANAGER_CONTEXT, CONTENTMENT_CONTENT_BLOCKS_ENTRIES_CONTEXT);
	}

	override _gotManager() {}

	override _gotEntries() {}

	override _gotContentType() {}
}

export const CONTENTMENT_CONTENT_BLOCKS_ENTRY_CONTEXT = new UmbContextToken<ContentmentContentBlocksEntryContext>(
	'UmbBlockEntryContext',
	undefined,
	(context): context is ContentmentContentBlocksEntryContext => context.IS_CONTENTMENT
);
