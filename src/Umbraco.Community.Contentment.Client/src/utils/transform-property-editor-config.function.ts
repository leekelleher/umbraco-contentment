// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { UmbPropertyEditorConfigProperty } from '@umbraco-cms/backoffice/property-editor';

export function transformPropertyEditorConfig<ValueType = Record<string, unknown>>(
	input: Array<UmbPropertyEditorConfigProperty>
): ValueType {
	return input.reduce((acc, item) => {
		acc[item.alias] = item.value;
		return acc;
	}, {} as { [key: string]: unknown }) as ValueType;
}
