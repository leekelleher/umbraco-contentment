// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

export function parseInt(input: unknown): number | undefined {
	if (input === undefined || input === null || input === '') return undefined;
	const num = Number(input);
	return Number.isNaN(num) ? undefined : num;
}
