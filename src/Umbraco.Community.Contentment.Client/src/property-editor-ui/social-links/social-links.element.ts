// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import {
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	property,
	repeat,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { simpleHashCode } from '@umbraco-cms/backoffice/observable-api';
import { UmbLitElement, umbFocus } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';
import { CONTENTMENT_SOCIAL_LINKS_SELECTION_MODAL } from './social-links-selection-modal.element.js';
import { UMB_MODAL_MANAGER_CONTEXT, umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import type {
	ContentmentConfigurationEditorValue,
	ContentmentSocialLinkValue,
	ContentmentSocialNetworkModel,
} from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-property-editor-ui-social-links')
export class ContentmentPropertyEditorUISocialLinksElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#sorter = new UmbSorterController<ContentmentSocialLinkValue>(this, {
		getUniqueOfElement: (element) => {
			return element.id;
		},
		getUniqueOfModel: (modelEntry) => {
			return this.#getUnique(modelEntry);
		},
		draggableSelector: '.handle',
		itemSelector: '.item',
		containerSelector: '#wrapper',
		onChange: ({ model }) => {
			this.value = model;
			this.dispatchEvent(new UmbPropertyValueChangeEvent());
		},
	});

	#confirmRemoval = false;

	#lookup: Record<string, ContentmentSocialNetworkModel> = {};

	#maxItems = Infinity;

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	#networks?: Array<ContentmentSocialNetworkModel>;

	@property({ type: Array })
	public set value(value: Array<ContentmentSocialLinkValue> | undefined) {
		this.#value = value ?? [];
		this.#sorter.setModel(this.#value);
	}
	public get value(): Array<ContentmentSocialLinkValue> | undefined {
		return this.#value;
	}
	#value?: Array<ContentmentSocialLinkValue> | undefined;

	set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;

		const networks = config.getValueByAlias<Array<ContentmentConfigurationEditorValue>>('networks');

		if (networks) {
			this.#networks = [];
			networks.forEach((network) => {
				const model = network.value as ContentmentSocialNetworkModel;
				this.#lookup[model.network] = model;
				this.#networks?.push(model);
			});
		}
	}

	constructor() {
		super();

		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => {
			this.#modalManager = modalManager;
		});
	}

	#getNetworkByKey(key: string): ContentmentSocialNetworkModel | undefined {
		return this.#lookup[key];
	}

	#getUnique(item: ContentmentSocialLinkValue): string {
		return 'x' + simpleHashCode(item.network + item.name + item.url).toString(16);
	}

	async #onChoose() {
		const value = await this.#openSelection();
		if (!value) return;

		const values = this.value ? [...this.value] : [];
		values.push(value);
		this.value = values;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onChangeName(event: UUIInputEvent, index: number) {
		const name = event.target.value as string;
		this.#updateValue({ name }, index);
	}

	async #onChangeNetwork(index: number) {
		const value = await this.#openSelection();
		if (!value) return;

		const network = value.network;
		this.#updateValue({ network }, index);
	}

	#onChangeUrl(event: UUIInputEvent, index: number) {
		const url = event.target.value as string;
		this.#updateValue({ url }, index);
	}

	async #onRemove(item: ContentmentSocialLinkValue, index: number) {
		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: `Remove ${item.name}?`,
				content: 'Are you sure you want to remove this item.',
				confirmLabel: 'Remove',
			});
		}

		const values = [...(this.value ?? [])];
		values.splice(index, 1);
		this.value = values;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	async #openSelection(): Promise<ContentmentSocialLinkValue | undefined> {
		if (!this.#modalManager) return;

		const modal = this.#modalManager.open(this, CONTENTMENT_SOCIAL_LINKS_SELECTION_MODAL, {
			data: { items: this.#networks ?? [] },
		});

		return await modal.onSubmit().catch(() => undefined);
	}

	#updateValue(partial: Partial<ContentmentSocialLinkValue>, index: number) {
		if (!partial || index === -1) return;

		if (!this.value) {
			this.value = [];
		}

		const values = [...this.value];
		const target = values[index];
		values[index] = { ...target, ...partial };
		this.value = values;

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
				label=${this.localize.term('contentment_addSocialLink')}
				look="placeholder"
				@click=${this.#onChoose}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.value || this.value.length === 0) return nothing;
		return html`
			<div id="wrapper">
				${repeat(
					this.value,
					(item) => this.#getUnique(item),
					(item, index) => this.#renderItem(item, index)
				)}
			</div>
		`;
	}

	#renderItem(item: ContentmentSocialLinkValue, index: number) {
		const network = this.#getNetworkByKey(item.network);
		return html`
			<div class="item" id=${this.#getUnique(item)}>
				<div class="handle"><uui-icon name="icon-navigation"></uui-icon></div>

				${when(
					!network,
					() => html`
						<uui-button
							look="placeholder"
							label=${this.localize.term('contentment_selectSocialNetwork')}
							@click=${() => this.#onChangeNetwork(index)}>
							<uui-icon name="add"></uui-icon>
						</uui-button>
					`,
					() => html`
						<uui-button
							look="default"
							label=${this.localize.term('contentment_selectSocialNetwork')}
							style="--uui-button-background-color: ${network!.backgroundColor};"
							@click=${() => this.#onChangeNetwork(index)}>
							<uui-icon name=${network!.icon} style="--uui-icon-color: ${network!.iconColor};"></uui-icon>
						</uui-button>
					`
				)}

				<div class="inputs">
					<uui-input
						label="Enter a social network name"
						value=${ifDefined(item.name)}
						placeholder=${this.localize.term('placeholders_entername')}
						${umbFocus()}
						@change=${(e: UUIInputEvent) => this.#onChangeName(e, index)}></uui-input>
					<uui-input
						label="Enter a social network URL"
						value=${ifDefined(item.url)}
						placeholder="Enter a URL..."
						@change=${(e: UUIInputEvent) => this.#onChangeUrl(e, index)}></uui-input>
				</div>

				<div class="actions">
					<uui-button
						label=${this.localize.term('general_remove')}
						look="secondary"
						@click=${() => this.#onRemove(item, index)}></uui-button>
				</div>
			</div>
		`;
	}

	static override styles = [
		css`
			#btn-add {
				display: block;
			}

			#wrapper {
				display: flex;
				flex-direction: column;
				gap: 1px;
				margin-bottom: var(--uui-size-1);
			}

			.item {
				display: flex;
				flex-direction: row;
				justify-content: space-between;
				align-items: center;
				gap: var(--uui-size-6);

				padding: var(--uui-size-3) var(--uui-size-6);
				background-color: var(--uui-color-surface-alt);
			}

			.item[drag-placeholder] {
				opacity: 0.5;
			}

			.item > uui-button {
				--uui-button-background-color-hover: var(--uui-button-background-color);

				flex: 0 0 var(--uui-size-10);

				font-size: var(--uui-size-layout-2);
				height: var(--uui-size-layout-4);
				width: var(--uui-size-layout-4);
			}

			.item > .inputs {
				flex: 1;

				display: flex;
				flex-direction: column;
				gap: var(--uui-size-1);
			}

			.item > .actions {
				flex: 0 0 auto;
				display: flex;
				justify-content: flex-end;
			}
		`,
	];
}

export { ContentmentPropertyEditorUISocialLinksElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-social-links': ContentmentPropertyEditorUISocialLinksElement;
	}
}
