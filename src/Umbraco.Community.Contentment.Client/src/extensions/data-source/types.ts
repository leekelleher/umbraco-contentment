// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbApi } from '@umbraco-cms/backoffice/extension-api';
import type { ContentmentListItem } from '../../property-editor-ui/types.js';

export interface ContentmentDataSourceApi extends UmbApi {
	getItems(config?: Record<string, unknown>): Promise<Array<ContentmentListItem>>;
}
