// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { ConfigurationEditorModel, ContentmentService } from '../../api/index.js';
import { css, customElement, html, nothing, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { tryExecuteAndNotify } from '@umbraco-cms/backoffice/resources';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import { UUIModalSidebarSize, UUITextareaElement } from '@umbraco-cms/backoffice/external/uui';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';
import { CONTENTMENT_CONFIGURATION_EDITOR_MODAL } from './configuration-editor-modal.element.js';

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

	#populateItemLookup() {
		if (!this._items) return;
		this._items.forEach((item) => {
			this.#lookup[item.key] = item;
		});
	}

	async #onChoose() {
		console.log('#onChoose');

		const modalContext = await this.getContext(UMB_MODAL_MANAGER_CONTEXT);

		const modal = modalContext.open(this, CONTENTMENT_CONFIGURATION_EDITOR_MODAL, {
			data: { items: this._items ?? [] },
		});

		const data = await modal.onSubmit();

		console.log('modal submitted', data);

		this.value = [...(this.value ?? []), data];
    this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onChange(event: Event & { target: UUITextareaElement }) {
		this.value = JSON.parse(event.target.value as string);
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onEdit(item: ContentmentConfigurationEditorValue) {
		console.log('#onEdit', [item]);
	}

	async #onRemove(item: ContentmentConfigurationEditorValue) {
		if (!item || !this.value) return;

		await umbConfirmModal(this, {
			color: 'danger',
			headline: 'Remove item?',
			content: 'Are you sure you want to remove this item',
			confirmLabel: 'Remove',
		});

		const index = this.value.indexOf(item, 0);
		if (index == -1) return;

		const tmp = [...this.value];
		tmp.splice(index, 1);
		this.value = tmp;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		return html`
			<!-- <details>
				<summary style="cursor:pointer;">Value</summary>
				<uui-textarea
					auto-height
					.value=${JSON.stringify(this.value, null, 4)}
					@change=${this.#onChange}></uui-textarea>
			</details> -->
			${this.#renderItems()} ${this.#renderButton()}
		`;
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
					(item) => html`
						<uui-ref-node
							@open=${this.#onEdit}
							standalone
							name=${this.#renderLabel(item, 'name')}
							detail=${this.#renderLabel(item, 'description')}>
							${this.#renderIcon(item)}
							<uui-action-bar slot="actions">
								<uui-button
									type="button"
									label=${this.localize.term('general_edit')}
									@click=${() => this.#onEdit(item)}></uui-button>
								<uui-button
									type="button"
									label=${this.localize.term('general_remove')}
									@click=${() => this.#onRemove(item)}></uui-button>
							</uui-action-bar>
						</uui-ref-node>
					`
				)}
			</uui-ref-list>
		`;
	}

	#renderIcon(item: ContentmentConfigurationEditorValue) {
		const icon = this.#renderLabel(item, 'icon');
		if (!icon) return nothing;
		return html`<uui-icon slot="icon" name=${icon}></uui-icon>`;
	}

	#renderLabel(item: ContentmentConfigurationEditorValue, attr: string): string {
		const model = this.#lookup[item.key] as any;
		return model && model.hasOwnProperty(attr) ? model[attr] : item.key;
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
