import { parseBoolean, parseInt } from '../../utils/index.js';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { DataPickerService } from '../../api/sdk.gen.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UMB_CONTENT_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/content';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorConfig } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../components/property-editor-ui/property-editor-ui.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-data-picker';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDataPickerElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#listEditor?: ContentmentDataListEditor;

	#listEditorConfig?: UmbPropertyEditorConfig;

	@state()
	private _dataSource?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _dataTypeKey?: string;

	@state()
	private _displayMode?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _entityUnique?: string;

	@state()
	private _initialized = false;

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

		this._dataSource = config.getValueByAlias('dataSource');
		this._displayMode = config.getValueByAlias('displayMode');

		this.#listEditorConfig = [
			{ alias: 'allowDuplicates', value: parseBoolean(config.getValueByAlias('allowDuplicates') ?? true) },
			{ alias: 'hideSearch', value: parseBoolean(config.getValueByAlias('hideSearch')) },
			{ alias: 'maxItems', value: parseInt(config.getValueByAlias('maxItems')) || Infinity },
			{ alias: 'overlaySize', value: config.getValueByAlias<UUIModalSidebarSize>('overlaySize') ?? 'medium' },
			{ alias: 'pageSize', value: parseInt(config.getValueByAlias('pageSize')) || 12 },
		];
	}

	constructor() {
		super();

		this.consumeContext('UmbMenuStructureWorkspaceContext', (context: any) => {
			this.observe(context.structure, (structure: Array<{ unique: string }>) => {
				this._entityUnique = structure.at(-1)?.unique;
			});
		});

		this.consumeContext(UMB_CONTENT_PROPERTY_CONTEXT, (context) => {
			this.observe(context.dataType, (dataType) => {
				this._dataTypeKey = dataType?.unique;
			});
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
				id: this._entityUnique,
				dataTypeKey: this._dataTypeKey,
				dataSource: this._dataSource[0],
				displayMode: this._displayMode[0],
				values: this.value,
			};

			const { data } = await tryExecuteAndNotify(this, DataPickerService.postDataPickerEditor({ requestBody }));

			if (!data) return reject();

			const listEditor = {
				propertyEditorUiAlias: data.propertyEditorUiAlias,
				config: new UmbPropertyEditorConfigCollection([...(data.config ?? []), ...(this.#listEditorConfig ?? [])]),
			};

			resolve(listEditor);
		});
	}

	#onChange(event: UmbPropertyValueChangeEvent & { target: UmbPropertyEditorUiElement }) {
		var element = event.target;
		if (!element || element.value === this.value) return;
		this.value = element.value as any;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		if (!this._initialized || !this.#listEditor) return html`<uui-loader></uui-loader>`;
		if (!this.#listEditor.propertyEditorUiAlias) return html`<lee-was-here></lee-was-here>`;
		//console.log('data-picker', this.#listEditor);
		return html`
			<contentment-property-editor-ui
				.config=${this.#listEditor.config}
				.propertyEditorUiAlias=${this.#listEditor.propertyEditorUiAlias}
				.value=${this.value}
				@change=${this.#onChange}>
			</contentment-property-editor-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIDataPickerElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDataPickerElement;
	}
}
