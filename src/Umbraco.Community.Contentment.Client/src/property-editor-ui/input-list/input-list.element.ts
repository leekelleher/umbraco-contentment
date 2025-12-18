// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, property, repeat, state } from '@umbraco-cms/backoffice/external/lit';
import { createExtensionElement } from '@umbraco-cms/backoffice/extension-api';
import { firstValueFrom } from '@umbraco-cms/backoffice/external/rxjs';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbDataTypeDetailRepository } from '@umbraco-cms/backoffice/data-type';
import { umbExtensionsRegistry } from '@umbraco-cms/backoffice/extension-registry';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbDataTypeDetailModel } from '@umbraco-cms/backoffice/data-type';
import type {
	ManifestPropertyEditorUi,
	UmbPropertyEditorConfigCollection as UmbPropertyEditorConfigCollectionType,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

interface InputListField {
	dataType: UmbDataTypeDetailModel;
	manifest: ManifestPropertyEditorUi | undefined;
}

interface InputListRow {
	key: string;
	values: Record<string, unknown>;
	elements: Map<string, UmbPropertyEditorUiElement>;
}

type InputListValue = Array<Record<string, unknown>>;

@customElement('contentment-property-editor-ui-input-list')
export class ContentmentPropertyEditorUIInputListElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#repository = new UmbDataTypeDetailRepository(this);

	@property({ type: Array })
	public set value(value: InputListValue | undefined) {
		this.#value = value ?? [];
		this.#syncRowsWithValue();
	}
	public get value(): InputListValue | undefined {
		return this.#value;
	}
	#value: InputListValue = [];

	@state()
	private _fields: Array<InputListField> = [];

	@state()
	private _rows: Array<InputListRow> = [];

	@state()
	private _ready = false;

	public set config(config: UmbPropertyEditorConfigCollectionType | undefined) {
		if (!config) return;

		const dataTypes = config.getValueByAlias<Array<string>>('dataTypes');

		if (dataTypes?.length) {
			this.#loadDataTypes(dataTypes);
		}
	}

	async #loadDataTypes(uniques: Array<string>) {
		const results = await Promise.all(
			uniques.map(async (unique) => {
				const { data: dataType } = await this.#repository.requestByUnique(unique);
				if (!dataType) return null;

				const manifest = dataType.editorUiAlias
					? ((await firstValueFrom(
							umbExtensionsRegistry.byTypeAndAlias('propertyEditorUi', dataType.editorUiAlias)
					  )) as ManifestPropertyEditorUi | undefined)
					: undefined;

				return { dataType, manifest };
			})
		);

		this._fields = results.filter((r): r is InputListField => r !== null);
		this._ready = true;

		// Initialize rows from existing value
		await this.#syncRowsWithValue();
	}

	async #syncRowsWithValue() {
		if (!this._ready || this._fields.length === 0) return;

		// Build rows from value, reusing existing row elements where possible
		const existingRowsByKey = new Map(this._rows.map((row) => [row.key, row]));
		const newRows: Array<InputListRow> = [];

		for (let i = 0; i < this.#value.length; i++) {
			const rowValue = this.#value[i];
			// Use index as key for now (could use a UUID stored in value for more stability)
			const key = `row-${i}`;

			const existingRow = existingRowsByKey.get(key);
			if (existingRow) {
				// Update existing row's values and element values
				existingRow.values = rowValue;
				this.#updateRowElementValues(existingRow);
				newRows.push(existingRow);
			} else {
				// Create new row
				const row = await this.#createRow(key, rowValue);
				newRows.push(row);
			}
		}

		this._rows = newRows;
	}

	async #createRow(key: string, values: Record<string, unknown>): Promise<InputListRow> {
		const elements = new Map<string, UmbPropertyEditorUiElement>();

		for (const field of this._fields) {
			if (!field.manifest) continue;

			const element = await createExtensionElement(field.manifest);
			if (!element) continue;

			// Configure the element
			element.value = values[field.dataType.unique];
			element.config = new UmbPropertyEditorConfigCollection(field.dataType.values);

			// Listen for changes
			element.addEventListener('change', () => this.#onFieldChange(key, field.dataType.unique, element.value));
			element.addEventListener('property-value-change', () =>
				this.#onFieldChange(key, field.dataType.unique, element.value)
			);

			elements.set(field.dataType.unique, element);
		}

		return { key, values, elements };
	}

	#updateRowElementValues(row: InputListRow) {
		for (const field of this._fields) {
			const element = row.elements.get(field.dataType.unique);
			if (element) {
				element.value = row.values[field.dataType.unique];
			}
		}
	}

	#onFieldChange(rowKey: string, fieldKey: string, value: unknown) {
		const rowIndex = this._rows.findIndex((r) => r.key === rowKey);
		if (rowIndex === -1) return;

		// Update internal state - create new objects to avoid mutation
		const updatedRow = {
			...this._rows[rowIndex],
			values: { ...this._rows[rowIndex].values, [fieldKey]: value },
		};
		const newRows = [...this._rows];
		newRows[rowIndex] = updatedRow;
		this._rows = newRows;

		// Update external value
		const newValue = [...this.#value];
		newValue[rowIndex] = { ...newValue[rowIndex], [fieldKey]: value };
		this.#value = newValue;

		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onAdd() {
		// Create empty row value
		const rowValue: Record<string, unknown> = {};
		this._fields.forEach((f) => (rowValue[f.dataType.unique] = undefined));

		// Create new row
		const key = `row-${this._rows.length}`;
		const row = await this.#createRow(key, rowValue);

		this._rows = [...this._rows, row];
		this.#value = [...this.#value, rowValue];

		this.dispatchEvent(new UmbChangeEvent());
	}

	#onRemove(index: number) {
		const rows = [...this._rows];
		rows.splice(index, 1);
		this._rows = rows;

		const value = [...this.#value];
		value.splice(index, 1);
		this.#value = value;

		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (!this._ready) return html`<uui-loader></uui-loader>`;
		return html`
			${this.#renderRows()}${this.#renderAddButton()}
			<umb-code-block language="value">${JSON.stringify(this.value, null, 2)}</umb-code-block>
		`;
	}

	#renderAddButton() {
		return html`
			<uui-button id="btn-add" label=${this.localize.term('general_add')} look="placeholder" @click=${this.#onAdd}></uui-button>
		`;
	}

	#renderRows() {
		if (this._rows.length === 0) return nothing;
		return html`
			<div id="rows">
				${repeat(
					this._rows,
					(row) => row.key,
					(row, index) => this.#renderRow(row, index)
				)}
			</div>
		`;
	}

	#renderRow(row: InputListRow, index: number) {
		return html`
			<div class="row">
				<div class="fields">
					${this._fields.map((field) => {
						const element = row.elements.get(field.dataType.unique);
						return html`
							<div class="field">
								<label>${field.dataType.name}</label>
								${element ?? html`<span>Loading...</span>`}
							</div>
						`;
					})}
				</div>
				<div class="actions">
					<uui-button label=${this.localize.term('general_remove')} @click=${() => this.#onRemove(index)}>
						<uui-icon name="delete"></uui-icon>
					</uui-button>
				</div>
			</div>
		`;
	}

	static override readonly styles = [
		css`
			#btn-add {
				display: block;
			}

			#rows {
				display: flex;
				flex-direction: column;
				gap: 1px;
				margin-bottom: var(--uui-size-1);
			}

			.row {
				display: flex;
				flex-direction: row;
				align-items: center;
				gap: var(--uui-size-6);

				padding: var(--uui-size-3) var(--uui-size-6);
				background-color: var(--uui-color-surface-alt);
				border-radius: var(--uui-border-radius);
			}

			.row > .fields {
				flex: 1;
				display: flex;
				flex-direction: row;
				gap: var(--uui-size-4);
			}

			.row > .fields > .field {
				flex: 1;
				display: flex;
				flex-direction: column;
				gap: var(--uui-size-1);
			}

			.row > .fields > .field > label {
				font-size: var(--uui-type-small-size);
				color: var(--uui-color-text-alt);
			}

			.row > .actions {
				flex: 0 0 auto;
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
