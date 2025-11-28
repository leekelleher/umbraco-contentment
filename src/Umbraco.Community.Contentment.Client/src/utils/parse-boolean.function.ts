// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export function parseBoolean(value: unknown): boolean {
	if (value === '0') return false;
	if (typeof value === 'string' && value.toLowerCase() === 'false') return false;
	return Boolean(value);
}
