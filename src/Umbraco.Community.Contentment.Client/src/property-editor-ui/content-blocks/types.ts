// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export type ContentBlockType = {
	alias: string;
	description?: string | null;
	icon?: string | null;
	name: string;
	key: string;
	nameTemplate?: string | null;
	overlaySize?: string | null;
	previewEnabled?: boolean;
};

export type ContentBlock = {
	elementType: string;
	key: string;
	value: Record<string, unknown>;
};
