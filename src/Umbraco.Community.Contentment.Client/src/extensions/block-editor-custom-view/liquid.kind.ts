// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import type { ManifestBlockEditorCustomView } from '@umbraco-cms/backoffice/block-custom-view';

export interface ContentmentLiquidTemplateModule {
	default: string;
}

export interface ContentmentBlockEditorCustomViewLiquidManifestKind extends ManifestBlockEditorCustomView {
	type: 'blockEditorCustomView';
	kind: 'liquid';
	/** Path to fetch or dynamic import loader */
	template?: string | (() => Promise<string | ContentmentLiquidTemplateModule>) | null | undefined;
	/** Inline Liquid template markup (fallback if template fails) */
	templateContent?: string | null | undefined;
}

declare global {
	interface UmbExtensionManifestMap {
		contentmentBlockEditorCustomViewLiquid: ContentmentBlockEditorCustomViewLiquidManifestKind;
	}
}
