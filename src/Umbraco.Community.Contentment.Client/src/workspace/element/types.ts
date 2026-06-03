// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { UmbElementValueModel } from '@umbraco-cms/backoffice/content';

/** The canonical persisted shape of a single element entry. */
export type ContentmentElementValue = {
	elementType: string;
	key: string;
	value: Record<string, unknown>;
};

/**
 * Internal runtime data model used by ContentmentElementManager and
 * ContentmentElementPropertyDatasetContext. Must satisfy
 * `UmbElementDetailModel` (i.e. `{ values: Array<UmbElementValueModel> }`).
 */
export type ContentmentElementDataModel = {
	key?: string;
	values: Array<UmbElementValueModel>;
};
