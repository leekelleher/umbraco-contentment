// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL } from './configuration-editor-selection-modal.element.js';
import { CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL } from './configuration-editor-workspace-modal.element.js';
import { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { ConfigurationEditorModel, ContentmentService } from '../../api/index.js';
import { css, customElement, html, nothing, property, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UUIModalSidebarSize } from '@umbraco-cms/backoffice/external/uui';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-configuration-editor';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIConfigurationEditorElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	@property({ type: Array })
	public value?: Array<ContentmentConfigurationEditorValue>;

	@state()
	_items?: Array<ContentmentConfigurationEditorModel>;

	#buttonLabelKey: string = 'general_add';

	#configurationType?: string;

	#lookup: Record<string, ContentmentConfigurationEditorModel> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#buttonLabelKey = config.getValueByAlias('addButtonLabelKey') ?? 'general_choose';
		this.#configurationType = config.getValueByAlias('configurationType');
		this.#maxItems = config.getValueByAlias('maxItems') ?? Infinity;
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

		const requestData = { type: this.#configurationType };
		const { data } = await tryExecuteAndNotify(
			this,
			ContentmentService.getContentmentConfigurationEditorModels(requestData)
		);

		if (!data) return;

		this._items = data.items.map((item: ConfigurationEditorModel) => ({
			key: item.key,
			name: item.name,
			description: item.description ?? undefined,
			icon: item.icon ?? undefined,
			group: item.group ?? undefined,
			defaultValues: item.defaultValues ?? undefined,
			expressions: item.expressions ?? undefined,
			fields: item.fields,
			overlaySize: item.overlaySize.toLowerCase() as UUIModalSidebarSize,
		}));

		this.#populateItemLookup();
	}

	#getItemByKey(key: string): ContentmentConfigurationEditorModel | undefined {
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

		const modal = this.#modalManager.open(this, CONTENTMENT_CONFIGURATION_EDITOR_SELECTION_MODAL, {
			data: { items: this._items ?? [] },
		});

		const data = await modal.onSubmit().catch(() => undefined);

		this.#setValue(data, this.value?.length ?? 0);
	}

	async #onEdit(item: ContentmentConfigurationEditorValue, index: number) {
		const model = this.#getItemByKey(item.key);

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

	render() {
		return html`${this.#renderItems()}${this.#renderButton()}`;
	}

	#renderButton() {
		if (this.value && this.value.length >= this.#maxItems) return nothing;
		return html`
			<uui-button
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
		const model = this.#getItemByKey(item.key);
		return html`
			<uui-ref-node
				name=${model?.name ?? item.key}
				detail=${model?.description ?? item.key}
				?standalone=${this.#maxItems === 1}
				@open=${() => this.#onEdit(item, index)}>
				${when(model?.icon, () => html`<uui-icon slot="icon" name=${model!.icon!}></uui-icon>`)}
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

	static styles = [
		css`
			uui-button {
				width: 100%;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIConfigurationEditorElement;
	}
}
