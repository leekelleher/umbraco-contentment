// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { DataListService } from '../../api/sdk.gen.js';
import { css, customElement, html, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import { UMB_PROPERTY_DATASET_CONTEXT } from '@umbraco-cms/backoffice/property';
import type { ContentmentConfigurationEditorValue, ContentmentDataListEditor } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import './data-list.element.js';
import '../../components/info-box/info-box.element.js';

type ContentmentDataListPreviewTab = {
	alias: 'dataSource' | 'listEditor' | 'rawJson';
	label: string;
	active?: boolean;
};

const ELEMENT_NAME = 'contentment-property-editor-ui-data-list-preview';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIDataListPreviewElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#items: Array<any> = [];

	#listEditor?: ContentmentDataListEditor;

	#tabs: Array<ContentmentDataListPreviewTab> = [
		{ alias: 'listEditor', label: 'Editor preview' },
		{ alias: 'dataSource', label: 'Data source items' },
		{ alias: 'rawJson', label: 'JSON' },
	];

	@state()
	private _activeTab: ContentmentDataListPreviewTab['alias'] = 'listEditor';

	@state()
	private _dataSource?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _listEditor?: Array<ContentmentConfigurationEditorValue>;

	@state()
	private _state: 'init' | 'loading' | 'loaded' | 'dataSourceConfigured' | 'listEditorConfigured' | 'noItems' = 'init';

	@property()
	public value?: string | string[];

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
	}

	constructor() {
		super();

		this.consumeContext(UMB_PROPERTY_DATASET_CONTEXT, async (context) => {
			this.observe(
				await context.propertyValueByAlias<Array<ContentmentConfigurationEditorValue>>('dataSource'),
				(dataSource) => {
					//console.log('dataSource', dataSource);
					this._dataSource = dataSource;
					this.#fetch();
				},
				'_observeDataSource'
			);

			this.observe(
				await context.propertyValueByAlias<Array<ContentmentConfigurationEditorValue>>('listEditor'),
				(listEditor) => {
					//console.log('listEditor', listEditor);
					this._listEditor = listEditor;
					this.#fetch();
				},
				'_observeListEditor'
			);
		});
	}

	override firstUpdated() {
		this.#onTabChange(this.#tabs[0]);
	}

	async #fetch() {
		this._state = 'loading';

		if (this._dataSource && this._listEditor) {
			this.#listEditor = await new Promise<ContentmentDataListEditor>(async (resolve, reject) => {
				if (!this._dataSource || !this._listEditor) return reject();

				const requestBody = { dataSource: this._dataSource, listEditor: this._listEditor };

				const { data } = await tryExecuteAndNotify(this, DataListService.postDataListEditor({ requestBody }));

				if (!data) return reject();

				const listEditor = {
					propertyEditorUiAlias: data.propertyEditorUiAlias,
					config: new UmbPropertyEditorConfigCollection(data.config ?? []),
				};

				this.#items = listEditor.config?.getValueByAlias<any[]>('items') ?? [];

				resolve(listEditor);
			});

			this._state = this.#items.length > 0 ? 'loaded' : 'noItems';

			// HACK: Replaces data sources label to include the item count. There could be a nicer way of doing this. [LK]
			this.#tabs[1].label = 'Data source items (' + this.#items.length + ')';
		} else if (this._dataSource) {
			this._state = 'dataSourceConfigured';
		} else if (this._listEditor) {
			this._state = 'listEditorConfigured';
		} else {
			this._state = 'init';
		}
	}

	#onTabChange(tab: any) {
		this.#tabs.forEach((tab) => (tab.active = false));
		this._activeTab = tab.alias;
		tab.active = true;
	}

	override render() {
		switch (this._state) {
			case 'dataSourceConfigured':
				return html`<p>Please select and configure a list editor.</p>`;

			case 'listEditorConfigured':
				return html`<p>Please select and configure a data source.</p>`;

			case 'loading':
				return html`<uui-loader></uui-loader>`;

			case 'loaded':
				return this.#renderPanel();

			case 'noItems':
				return html`<p>The data source returned no items to preview.</p>`;

			case 'init':
			default:
				return html`<p>Please select and configure a data source and list editor.</p>`;
		}
	}

	#renderPanel() {
		return html`
			${when(
				this.#items.length > 30,
				() => html`
					<contentment-info-box type=${this.#items.length >= 50 ? 'danger' : 'warning'}>
						<details>
							<summary>There are <strong>${this.#items.length}</strong> items avaliable in this data source.</summary>
							<p>
								To avoid an unpleasant editor experience or lagging browser performance, please consider trying the
								<strong>Data Picker</strong> property editor, as it has been designed with improved performance in mind,
								with support for search querying and pagination.
							</p>
						</details>
					</contentment-info-box>
				`
			)}

			<uui-tab-group>
				${repeat(
					this.#tabs,
					(tab) => tab.alias,
					(tab) =>
						html`<uui-tab label=${tab.label} ?active=${tab.active} @click=${() => this.#onTabChange(tab)}></uui-tab>`
				)}
			</uui-tab-group>

			${when(
				this._activeTab === 'listEditor' && this.#listEditor,
				() => html`
					<contentment-property-editor-ui
						.config=${this.#listEditor!.config}
						.propertyEditorUiAlias=${this.#listEditor!.propertyEditorUiAlias!}>
					</contentment-property-editor-ui>
				`
			)}
			${when(
				this._activeTab === 'dataSource',
				() => html`
					<uui-table>
						<uui-table-head>
							<uui-table-head-cell></uui-table-head-cell>
							<uui-table-head-cell>Name</uui-table-head-cell>
							<uui-table-head-cell>Value</uui-table-head-cell>
							<uui-table-head-cell>Description</uui-table-head-cell>
							<uui-table-head-cell>Enabled</uui-table-head-cell>
						</uui-table-head>

						${repeat(
							this.#items,
							(item) => item.value,
							(item) => html`
								<uui-table-row>
									<uui-table-cell><umb-icon .name=${item.icon}></umb-icon></uui-table-cell>
									<uui-table-cell>${item.name}</uui-table-cell>
									<uui-table-cell>${item.value}</uui-table-cell>
									<uui-table-cell>${item.description}</uui-table-cell>
									<uui-table-cell><uui-icon name=${item.disabled ? 'remove' : 'check'}></uui-icon></uui-table-cell>
								</uui-table-row>
							`
						)}
					</uui-table>
				`
			)}
			${when(
				this._activeTab === 'rawJson',
				() => html` <umb-code-block language="JSON" copy>${JSON.stringify(this.#items, null, 2)}</umb-code-block> `
			)}
		`;
	}

	static override styles = [
		css`
			contentment-info-box > details > summary {
				cursor: pointer;
				font-weight: bold;
			}

			uui-tab-group {
				margin-bottom: 1rem;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIDataListPreviewElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIDataListPreviewElement;
	}
}
