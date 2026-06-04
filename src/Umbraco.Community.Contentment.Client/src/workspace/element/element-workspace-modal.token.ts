// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { ContentmentElementValue } from './types.js';
import { UmbModalToken } from '@umbraco-cms/backoffice/modal';

export type ContentmentElementWorkspaceModalData = {
	element: ContentmentElementValue;
	readonly?: boolean;
};

export type ContentmentElementWorkspaceModalValue = {
	element: ContentmentElementValue;
};

export const CONTENTMENT_ELEMENT_WORKSPACE_MODAL = new UmbModalToken<
	ContentmentElementWorkspaceModalData,
	ContentmentElementWorkspaceModalValue
>('Umb.Contentment.Modal.ElementWorkspace', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});
