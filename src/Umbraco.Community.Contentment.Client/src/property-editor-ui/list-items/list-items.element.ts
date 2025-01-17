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
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { umbFocus, UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbPropertyEditorConfigCollection,
	UmbPropertyValueChangeEvent,
} from '@umbraco-cms/backoffice/property-editor';
import { UmbSorterController } from '@umbraco-cms/backoffice/sorter';
import type { ContentmentListItemValue } from '../types.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';
import type { ContentmentIconPickerElement } from '../../components/icon-picker/icon-picker.element.js';
import type { UmbChangeEvent } from '@umbraco-cms/backoffice/event';

type UmbIconPickerChangeEvent = UmbChangeEvent & { target: ContentmentIconPickerElement };

import '../../components/icon-picker/icon-picker.element.js';

@customElement('contentment-property-editor-ui-list-items')
export class ContentmentPropertyEditorUIListItemsElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#sorter = new UmbSorterController<ContentmentListItemValue>(this, {
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

	#hideDescription = false;

	#hideIcon = false;

	#maxItems = Infinity;

	@property({ type: Array })
	public set value(value: Array<ContentmentListItemValue> | undefined) {
		this.#value = value ?? [];
		this.#sorter.setModel(this.#value);
	}
	public get value(): Array<ContentmentListItemValue> | undefined {
		return this.#value;
	}
	#value?: Array<ContentmentListItemValue> | undefined;

	set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#hideDescription = parseBoolean(config.getValueByAlias('hideDescription'));
		this.#hideIcon = parseBoolean(config.getValueByAlias('hideIcon'));
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
	}

	#getUnique(item: ContentmentListItemValue): string {
		return 'x' + simpleHashCode(item.value + item.name + item.icon).toString(16);
	}

	#onAdd() {
		this.value = [
			...(this.value ?? []),
			{
				name: '',
				value: '',
				icon: 'icon-stop',
				description: '',
			},
		];

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onChangeDescription(event: UUIInputEvent, index: number) {
		const description = event.target.value as string;
		this.#updateValue({ description }, index);
	}

	#onChange(event: UmbIconPickerChangeEvent, index: number) {
		const icon = event.target.value;
		if (!icon) return;

		const values = [...(this.value ?? [])];
		values[index] = { ...values[index], icon };
		this.value = values;

		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	#onChangeName(event: UUIInputEvent, index: number) {
		const name = event.target.value as string;
		this.#updateValue({ name }, index);
	}

	#onChangeValue(event: UUIInputEvent, index: number) {
		const value = event.target.value as string;
		this.#updateValue({ value }, index);
	}

	async #onRemove(item: ContentmentListItemValue, index: number) {
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

	#updateValue(partial: Partial<ContentmentListItemValue>, index: number) {
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
				label=${this.localize.term('contentment_addItem')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.value || this.value.length === 0) return nothing;
		return html`
			<div id="wrapper">
				${repeat(
					this.value,
					(item, index) => item.name + index,
					(item, index) => this.#renderItem(item, index)
				)}
			</div>
		`;
	}

	#renderItem(item: ContentmentListItemValue, index: number) {
		return html`
			<div class="item" id=${this.#getUnique(item)}>
				<div class="handle"><uui-icon name="icon-navigation"></uui-icon></div>

				${when(
					!this.#hideIcon,
					() => html`
						<contentment-icon-picker
							.value=${item.icon}
							@change=${(event: UmbIconPickerChangeEvent) => this.#onChange(event, index)}>
						</contentment-icon-picker>
					`
				)}

				<div class="inputs">
					<div>
						<uui-input
							label=${this.localize.term('placeholders_entername')}
							value=${ifDefined(item.name)}
							placeholder=${this.localize.term('placeholders_entername')}
							@change=${(e: UUIInputEvent) => this.#onChangeName(e, index)}
							${this.#hideIcon ? umbFocus() : nothing}></uui-input>
						<uui-input
							label=${this.localize.term('placeholders_enterValue')}
							value=${ifDefined(item.value)}
							placeholder=${this.localize.term('placeholders_enterValue')}
							@change=${(e: UUIInputEvent) => this.#onChangeValue(e, index)}></uui-input>
					</div>
					${when(
						!this.#hideDescription,
						() => html`
							<div>
								<uui-input
									label=${this.localize.term('placeholders_enterDescription')}
									value=${ifDefined(item.description)}
									placeholder=${this.localize.term('placeholders_enterDescription')}
									@change=${(e: UUIInputEvent) => this.#onChangeDescription(e, index)}></uui-input>
							</div>
						`
					)}
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

				&[drag-placeholder] {
					opacity: 0.5;
				}

				& > contentment-icon-picker {
					flex: 0 0 var(--uui-size-10);

					font-size: var(--uui-size-layout-2);
					height: var(--uui-size-layout-4);
					width: var(--uui-size-layout-4);
				}

				& > .inputs {
					flex: 1;

					display: flex;
					flex-direction: column;
					gap: var(--uui-size-1);

					& > div {
						display: flex;
						justify-content: space-between;
						gap: var(--uui-size-1);

						& > * {
							flex: 1;
						}
					}
				}

				& > .actions {
					flex: 0 0 auto;
					display: flex;
					justify-content: flex-end;
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUIListItemsElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-list-items': ContentmentPropertyEditorUIListItemsElement;
	}
}
