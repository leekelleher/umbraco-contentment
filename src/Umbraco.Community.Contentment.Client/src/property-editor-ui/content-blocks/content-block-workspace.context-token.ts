// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentContentBlockWorkspaceContext } from './content-block-workspace.context.js';
import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import type { UmbWorkspaceContext } from '@umbraco-cms/backoffice/workspace';

export const CONTENTMENT_CONTENT_BLOCK_WORKSPACE_CONTEXT = new UmbContextToken<
	UmbWorkspaceContext,
	ContentmentContentBlockWorkspaceContext
>('ContentmentContentBlockWorkspaceContext');
