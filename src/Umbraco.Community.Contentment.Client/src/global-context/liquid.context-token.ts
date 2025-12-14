// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import type { ContentmentLiquidContext } from './liquid.context.js';

export const CONTENTMENT_LIQUID_CONTEXT = new UmbContextToken<ContentmentLiquidContext>(
	'Contentment.LiquidContext'
);
