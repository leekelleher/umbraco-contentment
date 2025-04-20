// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL } from './configuration-editor-selection-modal.element.js';
import { CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL } from './configuration-editor-workspace-modal.element.js';
import type {
	ContentmentConfigurationEditorModel,
	ContentmentConfigurationEditorValue,
	ContentmentListItem,
} from '../types.js';
import { customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

import '../../components/lee-was-here/lee-was-here.element.js';
import '../../extensions/display-mode/display-mode-ui.element.js';

@customElement('contentment-property-editor-ui-configuration-editor')
export class ContentmentPropertyEditorUIConfigurationEditorElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#allowEditLookup = new Set<string>();

	#buttonLabelKey: string = 'general_add';

	#canEdit = (item: ContentmentListItem) => this.#allowEditLookup.has(item.value);

	#config?: UmbPropertyEditorConfigCollection;

	#configurationType?: string;

	#disableSorting = false;

	#lookup: Record<string, ContentmentConfigurationEditorModel> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	#uiAlias: string = 'Umb.Contentment.DisplayMode.List';

	@state()
	private _initialized = false;

	@state()
	private _items?: Array<ContentmentListItem>;

	protected models?: Array<ContentmentConfigurationEditorModel>;

	@property({ type: Array })
	public value?: Array<ContentmentConfigurationEditorValue>;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#config = config;

		this.#buttonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? 'general_choose';
		this.#configurationType = config.getValueByAlias('configurationType');
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1 ? true : parseBoolean(config.getValueByAlias('disableSorting'));
		this.#uiAlias = config.getValueByAlias('uiAlias') ?? 'Umb.Contentment.DisplayMode.List';

		// enableFilter
		// help

		this.models = config.getValueByAlias('items');

		if (this.models) {
			this.populateModelLookup();
		} else {
			this.getModels();
		}
	}

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	protected getModels() {
		if (this.models || !this.#configurationType) return;

		this.observe(umbExtensionsRegistry.byType(this.#configurationType), (manifests) => {
			this.models = manifests
				.map((manifest: any) => ({
					...manifest.meta,
					key: manifest.meta?.key ?? manifest.alias,
					name: manifest.meta?.name ?? manifest.name,
					overlaySize: (manifest.meta?.overlaySize?.toLowerCase() as UUIModalSidebarSize) ?? 'small',
				}))
				.sort((a, b) => a.name.localeCompare(b.name));

			this.populateModelLookup();
		});
	}

	#getModelByKey(key: string): ContentmentConfigurationEditorModel | undefined {
		return this.#lookup[key];
	}

	protected populateModelLookup() {
		if (!this.models) return;

		this.models.forEach((model) => {
			this.#lookup[model.key] = model;

			if (model.fields?.length) {
				this.#allowEditLookup.add(model.key);
			}
		});

		this.#populateItems();
	}

	#populateItems() {
		const getItemValue = (
			item: ContentmentConfigurationEditorValue,
			model: ContentmentConfigurationEditorModel,
			key: string
		) => {
			const expression = model.expressions?.[key];
			if (expression && typeof expression === 'function') {
				return expression(item.value);
			}

			return model[key] ?? item.value[key];
		};

		const items: Array<ContentmentListItem> = [];

		this.value?.forEach((item) => {
			const model = this.#getModelByKey(item.key);
			if (model) {
				items.push({
					name: getItemValue(item, model, 'name')?.toString() ?? item.key,
					icon: getItemValue(item, model, 'icon')?.toString(),
					description: getItemValue(item, model, 'description')?.toString(),
					value: item.key,
					cardStyle: getItemValue(item, model, 'cardStyle'),
					iconStyle: getItemValue(item, model, 'iconStyle'),
				});
			}
		});

		this._items = items;

		this._initialized = true;
	}

	#setValue(value: ContentmentConfigurationEditorValue | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp[index] = value;
		this.value = tmp;

		this.#populateItems();

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onAdd() {
		if (!this.#modalManager) return;

		if (this.models?.length === 1) {
			const model = this.models[0];

			const item = {
				key: model.key,
				value: model.defaultValues ?? {},
			};

			const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL, {
				data: { item, model },
			});

			const data = await modal.onSubmit().catch(() => undefined);

			this.#setValue(data, this.value?.length ?? 0);
		} else {
			const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL, {
				data: { items: this.models ?? [] },
			});

			const data = await modal.onSubmit().catch(() => undefined);

			this.#setValue(data, this.value?.length ?? 0);
		}
	}

	async #onEdit(event: CustomEvent<{ item: ContentmentListItem; index: number }>) {
		if (!this.#modalManager || !this.#canEdit(event.detail.item)) return;

		const model = this.#getModelByKey(event.detail.item.value);
		if (!model?.fields?.length) return;

		const value = this.value?.[event.detail.index];
		if (!value) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL, {
			data: { item: value, model },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data, event.detail.index);
	}

	async #onRemove(event: CustomEvent<{ item: ContentmentListItem; index: number }>) {
		if (!event.detail.item || !this.value || event.detail.index == -1) return;

		await umbConfirmModal(this, {
			color: 'danger',
			headline: this.localize.term('contentment_removeItemHeadline', [event.detail.item.name]),
			content: this.localize.term('contentment_removeItemMessage'),
			confirmLabel: this.localize.term('contentment_removeItemButton'),
		});

		const items = [...this.value];
		items.splice(event.detail.index, 1);
		this.value = items;

		this.#populateItems();

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSort(event: CustomEvent<{ newIndex: number; oldIndex: number }>) {
		const items = [...(this.value ?? [])];
		items.splice(event.detail.newIndex, 0, items.splice(event.detail.oldIndex, 1)[0]);
		this.value = items;

		this.#populateItems();

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._initialized) return html`<uui-loader></uui-loader>`;
		if (!this.#uiAlias) return html`<lee-was-here></lee-was-here>`;
		return html`
			<contentment-display-mode-ui
				?allowAdd=${!this.value || this.value.length < this.#maxItems}
				?allowRemove=${true}
				?allowSort=${!this.#disableSorting}
				.canEdit=${(item: ContentmentListItem) => this.#canEdit(item)}
				.addButtonLabelKey=${this.#buttonLabelKey}
				.config=${this.#config}
				.items=${this._items}
				.uiAlias=${this.#uiAlias}
				@add=${this.#onAdd}
				@edit=${this.#onEdit}
				@remove=${this.#onRemove}
				@sort=${this.#onSort}>
			</contentment-display-mode-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIConfigurationEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-configuration-editor': ContentmentPropertyEditorUIConfigurationEditorElement;
	}
}
