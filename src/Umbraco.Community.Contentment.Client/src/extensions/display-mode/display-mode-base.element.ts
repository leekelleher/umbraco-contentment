// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { state } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { CONTENTMENT_DISPLAY_MODE_CONTEXT } from './display-mode.context-token.js';
import type { ContentmentListItem } from '../../property-editor-ui/types.js';

export abstract class ContentmentDisplayModeElement extends UmbLitElement {
	#context?: typeof CONTENTMENT_DISPLAY_MODE_CONTEXT.TYPE;

	addButtonLabelKey: string = 'general_add';

	@state()
	protected allowAdd = false;

	@state()
	protected allowEdit = false;

	@state()
	protected allowRemove = false;

	@state()
	protected allowSort = false;

	@state()
	protected items?: Array<ContentmentListItem> = [];

	constructor() {
		super();

		this.consumeContext(CONTENTMENT_DISPLAY_MODE_CONTEXT, (context) => {
			this.#context = context;

			this.observe(context?.allowAdd, (allowAdd) => (this.allowAdd = allowAdd ?? false));
			this.observe(context?.allowEdit, (allowEdit) => (this.allowEdit = allowEdit ?? false));
			this.observe(context?.allowRemove, (allowRemove) => (this.allowRemove = allowRemove ?? false));
			this.observe(context?.allowSort, (allowSort) => (this.allowSort = allowSort ?? false));
			this.observe(context?.items, (items) => (this.items = items));
		});
	}

	readonly canEdit = (item: ContentmentListItem, index: number): boolean =>
		this.#context?.canEdit(item, index) ?? this.allowEdit;

	getConfigByAlias<T>(alias: string): T | undefined {
		return this.#context?.getConfigByAlias<T>(alias);
	}

	readonly getItems = () => this.#context?.getItems();
}
