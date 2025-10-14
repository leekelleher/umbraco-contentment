// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentBlock } from './types.js';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { UmbWorkspaceContext } from '@umbraco-cms/backoffice/workspace';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import { UmbObjectState } from '@umbraco-cms/backoffice/observable-api';

export class ContentmentContentBlockWorkspaceContext extends UmbContextBase implements UmbWorkspaceContext {
	readonly IS_CONTENTMENT_CONTENT_BLOCK_WORKSPACE_CONTEXT = true;

	#data = new UmbObjectState<ContentBlock | undefined>(undefined);
	readonly data = this.#data.asObservable();

	#workspaceAlias = 'Contentment.ContentBlock.Workspace';

	constructor(host: UmbControllerHost) {
		super(host, 'ContentmentContentBlockWorkspaceContext');
	}

	get workspaceAlias(): string {
		return this.#workspaceAlias;
	}

	getEntityType(): string {
		return 'content-block';
	}

	setData(data: ContentBlock | undefined): void {
		this.#data.setValue(data);
	}

	getData(): ContentBlock | undefined {
		return this.#data.getValue();
	}

	getUnique(): string | undefined {
		return this.#data.getValue()?.key;
	}
}
