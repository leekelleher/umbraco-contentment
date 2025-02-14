// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentDisplayModeContext } from '../../extensions/display-mode/display-mode.context.js';
import { DataPickerService } from '../../api/sdk.gen.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { CONTENTMENT_DATA_PICKER_MODAL } from './data-picker-modal.element.js';
import { UMB_CONTENT_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/content';
import { UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';
import type {
	ContentmentConfigurationEditorValue,
	ContentmentDataListEditor,
	ContentmentDataListItem,
} from '../types.js';
import type { ContentmentDisplayModeElement } from '../../extensions/display-mode/display-mode.extension.js';
import type { UmbMenuStructureWorkspaceContext } from '@umbraco-cms/backoffice/menu';
import type { UmbPropertyEditorConfig } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../extensions/display-mode/display-mode-ui.element.js';

@customElement('contentment-property-editor-ui-data-picker')
export class ContentmentPropertyEditorUIDataPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#allowDuplicates = false;

	#context = new ContentmentDisplayModeContext(this);

	#defaultIcon?: string;

	#hideSearch = true;

	#listEditor?: ContentmentDataListEditor;

	#listEditorConfig?: UmbPropertyEditorConfig;

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

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
	private _propertyAlias?: string;

	@state()
	private _variantId?: string;

	@property({ type: Array })
	public set value(value: Array<string> | string | undefined) {
		this.#value = Array.isArray(value) ? value : value ? [value] : [];
		this.#context.setItems(this.#value.map((x) => ({ unique: x })));
	}
	public get value(): Array<string> | undefined {
		return this.#value;
	}
	#value?: Array<string> = [];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this._dataSource = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource')?.[0];
		this._displayMode = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('displayMode')?.[0];

		this.#allowDuplicates = parseBoolean(config.getValueByAlias('allowDuplicates') ?? true);
		this.#defaultIcon = config.getValueByAlias<string>('defaultIcon');
		this.#hideSearch = parseBoolean(config.getValueByAlias('hideSearch') ?? false);
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#overlaySize = config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'medium';
		this.#pageSize = parseInt(config.getValueByAlias('pageSize')) || 12;

		this.#listEditorConfig = [
			{ alias: 'confirmRemoval', value: false },
			{ alias: 'maxItems', value: this.#maxItems },
		];
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

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
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
				config: new UmbPropertyEditorConfigCollection([...(data.config ?? []), ...(this.#listEditorConfig ?? [])]),
			};

			const items = listEditor.config.getValueByAlias<Array<ContentmentDataListItem>>('items') ?? [];
			this.#context.populateItemLookup(items.map((x) => ({ ...x, unique: x.value })));

			resolve(listEditor);
		});
	}

	#onChange(event: UmbPropertyValueChangeEvent & { target: ContentmentDisplayModeElement }) {
		var element = event.target;
		if (!element || element.items === this.value) return;
		this.value = element.items as any;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	async #onOpenModal(event: CustomEvent) {
		if (!this.#modalManager) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_DATA_PICKER_MODAL, {
			data: {
				allowDuplicates: this.#allowDuplicates,
				defaultIcon: this.#defaultIcon,
				enableMultiple: this.#maxItems !== 1,
				hideSearch: this.#hideSearch,
				listType: event.detail ?? 'list',
				maxItems: this.#maxItems === 0 ? this.#maxItems : this.#maxItems - (this.value?.length ?? 0),
				pageSize: this.#pageSize,
				value: this.value ?? [],
			},
			modal: { size: this.#overlaySize },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data?.selection, this.value?.length ?? 0);
	}

	#setValue(value: Array<ContentmentDataListItem> | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		this.#context.populateItemLookup(value.map((x) => ({ ...x, unique: x.value })));

		const tmp = [...this.value];
		tmp.splice(index, 0, ...value.map((x) => x.value));
		this.value = tmp;

		this.#context.setItems(this.value.map((x) => ({ unique: x })));

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<lee-was-here></lee-was-here>`;
		//console.log('data-picker', this.#listEditor);
		return html`
			<contentment-display-mode-ui
				allow-add
				allow-remove
				.config=${this.#listEditor.config}
				.items=${[]}
				.uiAlias=${this.#listEditor.propertyEditorUiAlias}
				@open=${this.#onOpenModal}
				@change=${this.#onChange}>
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
