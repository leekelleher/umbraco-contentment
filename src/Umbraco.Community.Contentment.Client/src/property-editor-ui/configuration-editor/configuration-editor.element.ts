// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseInt } from '../../utils/index.js';
import { CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL } from './configuration-editor-selection-modal.element.js';
import { CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL } from './configuration-editor-workspace-modal.element.js';
import type { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-configuration-editor')
export class ContentmentPropertyEditorUIConfigurationEditorElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@state()
	_items?: Array<ContentmentConfigurationEditorModel>;

	#buttonLabelKey: string = 'general_add';

	#configurationType?: string;

	#lookup: Record<string, ContentmentConfigurationEditorModel> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	@property({ type: Array })
	public value?: Array<ContentmentConfigurationEditorValue>;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#buttonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? 'general_choose';
		this.#configurationType = config.getValueByAlias('configurationType');
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		// disableSorting
		// enableFilter
		// help

		this._items = config.getValueByAlias('items');

		if (this._items) {
			this.#populateItemLookup();
		} else {
			this.#getItems();
		}
	}

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	async #getItems() {
		if (this._items || !this.#configurationType) return;

		this.observe(umbExtensionsRegistry.byType(this.#configurationType), (items) => {
			this._items = items
				.map((item: any) => ({
					...item.meta,
					key: item.meta?.key ?? item.alias,
					name: item.meta?.name ?? item.name,
					overlaySize: (item.meta?.overlaySize?.toLowerCase() as UUIModalSidebarSize) ?? 'small',
				}))
				.sort((a, b) => a.name.localeCompare(b.name));

			this.#populateItemLookup();
		});
	}

	#getModelByKey(key: string): ContentmentConfigurationEditorModel | undefined {
		return this.#lookup[key];
	}

	#populateItemLookup() {
		if (!this._items) return;
		this._items.forEach((item) => {
			this.#lookup[item.key] = item;
		});
	}

	#setValue(value: ContentmentConfigurationEditorValue | undefined, index: number) {
		if (!value || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const tmp = [...this.value];
		tmp[index] = value;
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	async #onChoose() {
		if (!this.#modalManager) return;

		if (this._items?.length === 1) {
			const model = this._items[0];

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
				data: { items: this._items ?? [] },
			});

			const data = await modal.onSubmit().catch(() => undefined);

			this.#setValue(data, this.value?.length ?? 0);
		}
	}

	async #onEdit(item: ContentmentConfigurationEditorValue, index: number) {
		const model = this.#getModelByKey(item.key);

		if (!model?.fields?.length || !this.#modalManager) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL, {
			data: { item, model },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data, index);
	}

	async #onRemove(item: ContentmentConfigurationEditorValue, index: number) {
		if (!item || !this.value || index == -1) return;

		await umbConfirmModal(this, {
			color: 'danger',
			headline: 'Remove item?',
			content: 'Are you sure you want to remove this item?',
			confirmLabel: this.localize.term('general_remove'),
		});

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	override render() {
		return html`${this.#renderItems()}${this.#renderButton()}`;
	}

	#renderButton() {
		if (this.value && this.value.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term(this.#buttonLabelKey)}
				look="placeholder"
				@click=${this.#onChoose}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.value) return nothing;
		return html`
			<uui-ref-list>
				${repeat(
					this.value,
					(item) => item.key,
					(item, index) => this.#renderItem(item, index)
				)}
			</uui-ref-list>
		`;
	}

	#renderItem(item: ContentmentConfigurationEditorValue, index: number) {
		const model = this.#getModelByKey(item.key);
		if (!model) return;
		const icon = this.#renderMetadata(item, model, 'icon');
		return html`
			<uui-ref-node
				name=${this.#renderMetadata(item, model, 'name') ?? item.key}
				detail=${this.#renderMetadata(item, model, 'description') ?? item.key}
				?standalone=${this.#maxItems === 1}
				@open=${() => this.#onEdit(item, index)}>
				${when(icon, () => html`<umb-icon slot="icon" name=${icon!}></umb-icon>`)}
				<uui-action-bar slot="actions">
					${when(
						model?.fields?.length,
						() =>
							html`
								<uui-button
									label=${this.localize.term('general_edit')}
									@click=${() => this.#onEdit(item, index)}></uui-button>
							`
					)}
					<uui-button
						label=${this.localize.term('general_remove')}
						@click=${() => this.#onRemove(item, index)}></uui-button>
				</uui-action-bar>
			</uui-ref-node>
		`;
	}

	#renderMetadata(
		item: ContentmentConfigurationEditorValue,
		model: ContentmentConfigurationEditorModel,
		key: string
	): string | unknown | undefined {
		const expression = model.expressions?.[key];
		if (expression && typeof expression === 'function') {
			return expression(item.value);
		}

		return model[key] ?? item.value[key];
	}

	static override styles = [
		css`
			#btn-add {
				display: block;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-configuration-editor': ContentmentPropertyEditorUIConfigurationEditorElement;
	}
}
