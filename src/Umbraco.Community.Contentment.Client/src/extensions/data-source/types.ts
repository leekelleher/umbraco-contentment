// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbApi } from '@umbraco-cms/backoffice/extension-api';

export interface ContentmentDataSourceApi extends UmbApi {
	getItems(config: any): Promise<any[]>;
}
