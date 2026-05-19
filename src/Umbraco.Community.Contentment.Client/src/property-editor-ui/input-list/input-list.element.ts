// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { css, customElement, html, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentSortEndEvent } from '../../components/sortable-list/sort-end.event.js';
import type { UmbPropertyDatasetElement, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyEditorConfig, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import './input-list-property-editor.element.js';

interface ContentmentInputListProperty {
	dataTypeKey: string;
	propertyEditorUiAlias: string;
	propertyEditorUiConfig?: UmbPropertyEditorConfig;
}

type ContentmentInputListValue = Array<Array<UmbPropertyValueData>>;

type UmbPropertyDatasetElementChangeEvent = Event & { target: UmbPropertyDatasetElement };

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#confirmRemoval = false;

	#disableSorting = false;

	#inputs: Array<ContentmentInputListProperty> = [];

	#maxItems = Infinity;

	#repository = new UmbDataTypeDetailRepository(this);

	@property({ type: Boolean, reflect: true })
	readonly = false;

	@property({ type: Array })
	value?: ContentmentInputListValue;

	@state()
	private _loading = true;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;

		this.#confirmRemoval = parseBoolean(config.getValueByAlias('confirmRemoval'));
		this.#maxItems = parseInt(config.getValueByAlias('maxItems')) || Infinity;
		this.#disableSorting = this.#maxItems === 1;

		const dataTypes = config.getValueByAlias<Array<string>>('dataTypes');
		if (dataTypes?.length) {
			this.#loadDataTypes(dataTypes);
		}
	}

	async #loadDataTypes(uniques: Array<string>) {
		const results = (await Promise.all(
			uniques.map(async (unique): Promise<ContentmentInputListProperty | null> => {
				const { data } = await this.#repository.requestByUnique(unique);
				if (!data) return null;
				if (!data.editorUiAlias) return null;

				return {
					propertyEditorUiConfig: data.values,
					propertyEditorUiAlias: data.editorUiAlias,
					dataTypeKey: data.unique,
				};
			}),
		)) as Array<ContentmentInputListProperty | null>;

		this.#inputs = results.filter((control) => !!control) as Array<ContentmentInputListProperty>;

		this._loading = false;
	}

	#onAdd() {
		this.value = [...(this.value ?? []), []];
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onChange(event: UmbPropertyDatasetElementChangeEvent, index: number) {
		const value = [...(this.value ?? [])];
		value[index] = event.target.value;
		this.value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onRemove(index: number) {
		if (this.#confirmRemoval) {
			await umbConfirmModal(this, {
				color: 'danger',
				headline: this.localize.term('contentment_removeItemHeadline'),
				content: this.localize.term('contentment_removeItemMessage'),
				confirmLabel: this.localize.term('contentment_removeItemButton'),
			});
		}

		const value = [...(this.value ?? [])];
		value.splice(index, 1);
		this.value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSortEnd(event: ContentmentSortEndEvent) {
		if (event.newIndex === undefined || event.oldIndex === undefined) return;

		const value = [...(this.value ?? [])];
		value.splice(event.newIndex, 0, value.splice(event.oldIndex, 1)[0]);
		this.value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`${this.#renderItems()}${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		if ((this.value?.length ?? 0) >= this.#maxItems) return;
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('contentment_addItem')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderItems() {
		if (!this.value || this.value?.length === 0) return;
		return html`
			<contentment-sortable-list
				id="list"
				item-selector=".item"
				handle-selector=".handle"
				?disabled=${this.#disableSorting}
				@sort-end=${this.#onSortEnd}>
				${repeat(
					this.value,
					(_, index) => index,
					(_, index) => this.#renderItem(index),
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(index: number) {
		return html`
			<contentment-sortable-list-item
				class="item"
				?hideActions=${this.readonly}
				?hideHandle=${this.#disableSorting}
				@delete=${() => this.#onRemove(index)}>
				<umb-property-dataset
					class="inputs"
					.value=${this.value?.[index] ?? []}
					@change=${(event: UmbPropertyDatasetElementChangeEvent) => this.#onChange(event, index)}>
					${repeat(
						this.#inputs,
						(input) => input.dataTypeKey,
						(input) => html`
							<contentment-input-list-property-editor
								.alias=${input.dataTypeKey}
								.config=${input.propertyEditorUiConfig}
								.propertyEditorUiAlias=${input.propertyEditorUiAlias}>
							</contentment-input-list-property-editor>
						`,
					)}
				</umb-property-dataset>
			</contentment-sortable-list-item>
		`;
	}

	static override readonly styles = [
		css`
			#btn-add {
				display: block;
			}

			#list {
				display: flex;
				flex-direction: column;
				gap: 1px;
				margin-bottom: var(--uui-size-1);
			}

			.inputs {
				display: flex;
				flex-direction: row;
				gap: var(--uui-size-4);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIInputListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list': ContentmentPropertyEditorUIInputListElement;
	}
}
