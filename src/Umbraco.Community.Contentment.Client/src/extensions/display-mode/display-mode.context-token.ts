// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { UmbContextToken } from '@umbraco-cms/backoffice/context-api';
import type { ContentmentDisplayModeContext } from './display-mode.context.js';

export const CONTENTMENT_DISPLAY_MODE_CONTEXT = new UmbContextToken<ContentmentDisplayModeContext>(
	'ContentmentDisplayModeContext'
);
