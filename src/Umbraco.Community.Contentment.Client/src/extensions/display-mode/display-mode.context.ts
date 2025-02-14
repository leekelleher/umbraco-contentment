// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { UmbArrayState } from '@umbraco-cms/backoffice/observable-api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';

export class ContentmentDisplayModeContext<
	ItemTypeMetadata extends { unique: string },
	ItemTypeValue extends { unique: string }
> extends UmbContextBase<ContentmentDisplayModeContext<ItemTypeMetadata, ItemTypeValue>> {
	#lookup: Record<string, ItemTypeMetadata> = {};

	#items = new UmbArrayState<ItemTypeValue>([], (x) => x.unique);
	items = this.#items.asObservable();

	constructor(host: UmbControllerHost) {
		super(host, 'ContentmentDisplayModeContext');
	}

	getItem(unique: string): ItemTypeMetadata | undefined {
		return this.#lookup[unique];
	}

	populateItemLookup(items?: Array<ItemTypeMetadata>) {
		if (!items) return;
		items.forEach((item) => {
			this.#lookup[item.unique] = item;
		});
	}

	setItems(items?: Array<ItemTypeValue>) {
		if (!items) return;
		this.#items.setValue(items);
	}
}
