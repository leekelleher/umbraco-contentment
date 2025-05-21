// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { UmbArrayState, UmbBooleanState } from '@umbraco-cms/backoffice/observable-api';
import { UmbContextBase } from '@umbraco-cms/backoffice/class-api';
import type { UmbControllerHost } from '@umbraco-cms/backoffice/controller-api';
import type { ContentmentListItem } from '../../property-editor-ui/types.js';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';

export class ContentmentDisplayModeContext extends UmbContextBase {
	#canEdit?: (item: ContentmentListItem, index: number) => boolean;

	#config?: UmbPropertyEditorConfigCollection;

	#allowAdd = new UmbBooleanState(false);
	allowAdd = this.#allowAdd.asObservable();

	#allowEdit = new UmbBooleanState(false);
	allowEdit = this.#allowEdit.asObservable();

	#allowRemove = new UmbBooleanState(false);
	allowRemove = this.#allowRemove.asObservable();

	#allowSort = new UmbBooleanState(false);
	allowSort = this.#allowSort.asObservable();

	#items = new UmbArrayState<ContentmentListItem>([], (x) => x.value);
	items = this.#items.asObservable();

	constructor(host: UmbControllerHost) {
		super(host, 'ContentmentDisplayModeContext');
	}

	readonly canEdit = (item: ContentmentListItem, index: number): boolean =>
		this.#canEdit?.(item, index) ?? this.#allowEdit.getValue();

	getConfigByAlias<T>(alias: string): T | undefined {
		return this.#config?.getValueByAlias<T>(alias);
	}

	readonly getItems = () => this.#items.getValue();

	readonly setAllowAdd = (value: boolean) => this.#allowAdd.setValue(value);

	readonly setAllowEdit = (value: boolean) => this.#allowEdit.setValue(value);

	readonly setAllowRemove = (value: boolean) => this.#allowRemove.setValue(value);

	readonly setAllowSort = (value: boolean) => this.#allowSort.setValue(value);

	readonly setCanEdit = (callback: (item: ContentmentListItem, index: number) => boolean) => (this.#canEdit = callback);

	readonly setConfig = (config?: UmbPropertyEditorConfigCollection) => (this.#config = config);

	readonly setItems = (items?: Array<ContentmentListItem>) => this.#items.setValue(items ?? []);
}
