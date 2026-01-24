// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

export class ContentmentSortEndEvent extends Event {
	constructor(newIndex?: number, oldIndex?: number) {
		super('sort-end');
		this.newIndex = newIndex;
		this.oldIndex = oldIndex;
	}

	newIndex?: number;
	oldIndex?: number;
}
