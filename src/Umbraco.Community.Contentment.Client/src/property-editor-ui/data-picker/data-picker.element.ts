// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { DataPickerService } from '../../api/sdk.gen.js';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { CONTENTMENT_DATA_PICKER_MODAL } from './data-picker-modal.element.js';
import { UMB_CONTENT_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/content';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor, ContentmentListItem } from '../types.js';
import type { UmbMenuStructureWorkspaceContext } from '@umbraco-cms/backoffice/menu';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import '../../extensions/display-mode/display-mode-ui.element.js';

@customElement('contentment-property-editor-ui-data-picker')
export class ContentmentPropertyEditorUIDataPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#allowDuplicates = false;

	#config?: UmbPropertyEditorConfigCollection;

	#confirmRemoval = false;

	#defaultIcon?: string;

	#disableSorting = false;

	#hideSearch = true;

	#listEditor?: ContentmentDataListEditor;

	#maxItems = Infinity;

	#overlaySize: UUIModalSidebarSize = 'small';

	#pageSize = 12;

	@state()
	private _dataSource?: ContentmentConfigurationEditorValue;

	@state()
	private _dataTypeKey?: string;

	@state()
	private _displayMode?: ContentmentConfigurationEditorValue;

	@state()
	private _entityUnique?: string | null;

	@state()
	private _initialized = false;

	@state()
	private _items?: Array<ContentmentListItem>;

	@state()
	private _propertyAlias?: string;

	@state()
	private _variantId?: string;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#config = config;

		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		this._displayMode = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('displayMode')?.[0];

		this.#allowDuplicates = parseBoolean(config.getValueByAlias('allowDuplicates') ?? true);
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon');
		this.#hideSearch = parseBoolean(config.getValueByAlias('hideSearch') ?? false);
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#overlaySize = config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'medium';
		this.#pageSize = parseInt(config.getValueByAlias('pageSize')) || 12;
	}

	constructor() {
		super();

		this.consumeContext(
			'UmbMenuStructureWorkspaceContext',
			(menuStructureWorkspaceContext: UmbMenuStructureWorkspaceContext) => {
				this.observe(menuStructureWorkspaceContext.structure, (structure) => {
					this._entityUnique = structure.at(-1)?.unique;
				});
			}
		);

		this.consumeContext(UMB_CONTENT_PROPERTY_CONTEXT, (context) => {
			this.observe(context.dataType, (dataType) => (this._dataTypeKey = dataType?.unique));
		});

		this.consumeContext(UMB_PROPERTY_CONTEXT, (propertyContext) => {
			this.observe(propertyContext.alias, (alias) => (this._propertyAlias = alias));
			this.observe(propertyContext.variantId, (variantId) => (this._variantId = variantId?.toString() || 'invariant'));
		});
	}

	override async firstUpdated() {
		await Promise.all([await this.#init().catch(() => undefined)]);
		this._initialized = true;
	}

	async #init() {
		this.#listEditor = await new Promise<ContentmentDataListEditor>(async (resolve, reject) => {
			if (!this._entityUnique || !this._dataTypeKey || !this._dataSource || !this._displayMode) return reject();

			const requestBody = {
				alias: this._propertyAlias,
				dataTypeKey: this._dataTypeKey,
				dataSource: this._dataSource,
				displayMode: this._displayMode,
				id: this._entityUnique,
				values: this.value,
				variant: this._variantId,
			};

			const { data } = await tryExecuteAndNotify(this, DataPickerService.postDataPickerEditor({ requestBody }));

			if (!data) return reject();

			const listEditor = {
				propertyEditorUiAlias: data.propertyEditorUiAlias,
				config: new UmbPropertyEditorConfigCollection([...(data.config ?? []), ...(this.#config ?? [])]),
			};

			const items = listEditor.config.getValueByAlias<Array<ContentmentListItem>>('items') ?? [];
			this._items = items;

			this.#disableSorting =
				this.#maxItems === 1 ? true : parseBoolean(listEditor.config.getValueByAlias('disableSorting'));

			this.#confirmRemoval = parseBoolean(listEditor.config.getValueByAlias('confirmRemoval'));

			resolve(listEditor);
		});
	}

	async #onAdd(event: CustomEvent<{ listType: string }>) {
		const modalManager = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);
		if (!modalManager) return;

		const modal = modalManager.open(this, CONTENTMENT_DATA_PICKER_MODAL, {
			data: {
				allowDuplicates: this.#allowDuplicates,
				defaultIcon: this.#defaultIcon,
				enableMultiple: this.#maxItems !== 1,
				hideSearch: this.#hideSearch,
				listType: event.detail.listType ?? 'list',
				maxItems: this.#maxItems === 0 ? this.#maxItems : this.#maxItems - (this.value?.length ?? 0),
				pageSize: this.#pageSize,
				value: this.value ?? [],
			},
			modal: { size: this.#overlaySize },
		});

		const data = await modal.onSubmit().catch(() => undefined);
		if (!data) return;

		const { selection } = data;
		if (!selection) return;

		const index = this.value?.length ?? 0;
		if (index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const items = [...(this._items ?? [])];
		items.splice(index, 0, ...selection);
		this._items = items;

		this.value = this._items.map((x) => x.value);

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onRemove(event: CustomEvent<{ item: ContentmentListItem; index: number }>) {
		if (!event.detail.item || !this._items || event.detail.index == -1) return;

		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: this.localize.term('contentment_removeItemHeadline', [event.detail.item.name]),
				content: this.localize.term('contentment_removeItemMessage'),
				confirmLabel: this.localize.term('contentment_removeItemButton'),
			});
		}

		const items = [...this._items];
		items.splice(event.detail.index, 1);
		this._items = items;

		this.value = this._items.map((x) => x.value);

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSort(event: CustomEvent<{ newIndex: number; oldIndex: number }>) {
		const items = [...(this._items ?? [])];
		items.splice(event.detail.newIndex, 0, items.splice(event.detail.oldIndex, 1)[0]);
		this._items = items;

		this.value = this._items.map((x) => x.value);

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<lee-was-here></lee-was-here>`;
		return html`
			<contentment-display-mode-ui
				?allowAdd=${this.value && this.value.length < this.#maxItems}
				?allowRemove=${true}
				?allowSort=${!this.#disableSorting}
				.config=${this.#listEditor.config}
				.items=${this._items}
				.uiAlias=${this.#listEditor.propertyEditorUiAlias}
				@add=${this.#onAdd}
				@remove=${this.#onRemove}
				@sort=${this.#onSort}>
			</contentment-display-mode-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIDataPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-data-picker': ContentmentPropertyEditorUIDataPickerElement;
	}
}
