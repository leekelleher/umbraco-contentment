// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { parseBoolean, parseInt } from '../../utils/index.js';
import { css, customElement, html, nothing, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { umbConfirmModal } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentSortEndEvent } from '../../components/sortable-list/sort-end.event.js';
import type { UmbPropertyDatasetElement } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyEditorConfig, UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import '../../components/sortable-list/sortable-list.element.js';
import './input-list-property-editor.element.js';

type ContentmentInputListColumnConfig = {
	key: string; // stable column id (UUID)
	dataType: string; // data-type guid
	label: string; // display label (config-only, not used at runtime)
};

interface ContentmentInputListProperty {
	key: string; // column key (from config column.key)
	dataTypeKey: string;
	propertyEditorUiAlias: string;
	propertyEditorUiConfig?: UmbPropertyEditorConfig;
}

interface ContentmentInputListRow {
	key: string;
	values: Array<ContentmentInputListItemValue>;
}

interface ContentmentInputListItemValue {
	dataType: string;
	key: string;
	value?: unknown;
}

type ContentmentInputListValue = Array<ContentmentInputListRow>;

type UmbPropertyDatasetElementChangeEvent = Event & { target: UmbPropertyDatasetElement };

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#confirmRemoval = false;

	#disableSorting = false;

	#inputs: Array<ContentmentInputListProperty> = [];

	#loadGeneration = 0;

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

		const columns = config.getValueByAlias<Array<ContentmentInputListColumnConfig>>('columns');
		if (columns?.length) {
			this.#loadDataTypes(columns);
		}
	}

	async #loadDataTypes(columns: Array<ContentmentInputListColumnConfig>) {
		const generation = ++this.#loadGeneration;

		const results = (await Promise.all(
			columns.map(async (column): Promise<ContentmentInputListProperty | null> => {
				const { data } = await this.#repository.requestByUnique(column.dataType);
				if (!data) return null;
				if (!data.editorUiAlias) return null;

				return {
					key: column.key,
					dataTypeKey: data.unique,
					propertyEditorUiAlias: data.editorUiAlias,
					propertyEditorUiConfig: data.values,
				};
			}),
		)) as Array<ContentmentInputListProperty | null>;

		// Discard stale results if config was updated while this load was in flight.
		if (generation !== this.#loadGeneration) return;

		this.#inputs = results.filter((control) => !!control) as Array<ContentmentInputListProperty>;

		this._loading = false;
	}

	#onAdd() {
		this.value = [
			...(this.value ?? []),
			{
				key: UmbId.new(),
				values: this.#inputs.map((input) => ({ key: input.key, dataType: input.dataTypeKey, value: undefined })),
			},
		];
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onChange(event: UmbPropertyDatasetElementChangeEvent, index: number) {
		const value = [...(this.value ?? [])];
		const savedValues = value[index].values;

		// Rebuild from current inputs (handles new/removed columns), updating each value from the dataset.
		// umb-property-dataset emits the full dataset state on every change, so event.target.value
		// contains all columns — use it as the authoritative source, falling back to saved values.
		// Spread the row into a new object — the original is frozen by Lit's property binding.
		value[index] = {
			...value[index],
			values: this.#inputs.map((input) => {
				const existing = savedValues.find((item) => item.key === input.key);
				const fromDataset = event.target.value.find((v) => v.alias === input.key);
				return {
					key: input.key,
					dataType: input.dataTypeKey,
					value: fromDataset?.value ?? existing?.value,
				};
			}),
		};

		this.value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onRemove(index: number) {
		if (this.#confirmRemoval) {
			try {
				await umbConfirmModal(this, {
					color: 'danger',
					headline: this.localize.term('contentment_removeItemHeadline'),
					content: this.localize.term('contentment_removeItemMessage'),
					confirmLabel: this.localize.term('contentment_removeItemButton'),
				});
			} catch {
				return; // user cancelled
			}
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
		if ((this.value?.length ?? 0) >= this.#maxItems) return nothing;
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
			<contentment-sortable-list
				id="list"
				item-selector=".item"
				handle-selector=".handle"
				?disabled=${this.#disableSorting}
				@sort-end=${this.#onSortEnd}>
				${repeat(
					this.value,
					(row) => row.key,
					(_, index) => this.#renderItem(index),
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(index: number) {
		const savedValues = this.value?.[index]?.values ?? [];

		// Merge against current inputs: add missing columns, drop orphan columns
		const mergedValues = this.#inputs.map((input) => {
			const existing = savedValues.find((item) => item.key === input.key);
			return existing ?? { key: input.key, dataType: input.dataTypeKey, value: undefined };
		});

		const data = mergedValues.map((item) => ({ alias: item.key, value: item.value }));

		return html`
			<contentment-sortable-list-item
				class="item"
				?hideActions=${this.readonly}
				?hideHandle=${this.#disableSorting}
				@delete=${() => this.#onRemove(index)}>
				<umb-property-dataset
					class="inputs"
					.value=${data}
					@change=${(event: UmbPropertyDatasetElementChangeEvent) => this.#onChange(event, index)}>
					${repeat(
						this.#inputs,
						(input) => input.key,
						(input) => html`
							<contentment-input-list-property-editor
								.alias=${input.key}
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
				--uui-button-border-radius: var(--uui-border-radius);
				--uui-button-padding-top-factor: 2;
				--uui-button-padding-bottom-factor: 2;
				display: block;
			}

			#list {
				display: flex;
				flex-direction: column;
				gap: var(--uui-size-1);
				margin-bottom: var(--uui-size-1);
			}

			.inputs {
				display: flex;
				flex-direction: row;
				gap: var(--uui-size-4);

				& > contentment-input-list-property-editor:last-child {
					flex: 1;
				}
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
