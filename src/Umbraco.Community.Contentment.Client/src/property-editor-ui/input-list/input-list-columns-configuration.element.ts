// SPDX-License-Identifier: MIT
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, property, repeat } from '@umbraco-cms/backoffice/external/lit';
import { umbConfirmModal, UMB_MODAL_MANAGER_CONTEXT } from '@umbraco-cms/backoffice/modal';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbId } from '@umbraco-cms/backoffice/id';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import {
	UmbDataTypeItemRepository,
	UMB_DATA_TYPE_PICKER_MODAL,
	UMB_DATATYPE_WORKSPACE_MODAL,
} from '@umbraco-cms/backoffice/data-type';
import { UmbModalRouteRegistrationController } from '@umbraco-cms/backoffice/router';
import type { ContentmentSortEndEvent } from '../../components/sortable-list/sort-end.event.js';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';

import '../../components/sortable-list/sortable-list.element.js';

interface InputListColumnConfig {
	key: string;
	dataType: string;
	label: string;
}

@customElement('contentment-property-editor-ui-input-list-columns-configuration')
export class ContentmentPropertyEditorUIInputListColumnsConfigurationElement
	extends UmbLitElement
	implements UmbPropertyEditorUiElement
{
	#editDataTypeModal = new UmbModalRouteRegistrationController(this, UMB_DATATYPE_WORKSPACE_MODAL);

	#modalManager?: typeof UMB_MODAL_MANAGER_CONTEXT.TYPE;

	@property({ type: Array })
	public value?: Array<InputListColumnConfig>;

	public config?: UmbPropertyEditorUiElement['config'];

	constructor() {
		super();
		this.consumeContext(UMB_MODAL_MANAGER_CONTEXT, (modalManager) => (this.#modalManager = modalManager));
	}

	async #onAdd() {
		if (!this.#modalManager) return;

		const pickerModal = this.#modalManager.open(this, UMB_DATA_TYPE_PICKER_MODAL, {
			data: { expandTreeRoot: true, multiple: false },
		});
		const pickerResult = await pickerModal.onSubmit().catch(() => undefined);
		const dataTypeKey = pickerResult?.selection?.[0];
		if (!dataTypeKey) return;

		const repo = new UmbDataTypeItemRepository(this);
		const { data } = await repo.requestItems([dataTypeKey]);
		const label = data?.[0]?.name ?? dataTypeKey;

		this.value = [...(this.value ?? []), { key: UmbId.new(), dataType: dataTypeKey, label }];
		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onChangeDataType(event: Event, index: number) {
		event.stopPropagation();

		if (!this.#modalManager) return;

		const item = this.value?.[index];
		if (!item) return;

		const pickerModal = this.#modalManager.open(this, UMB_DATA_TYPE_PICKER_MODAL, {
			data: { expandTreeRoot: true, multiple: false },
			value: { selection: [item.dataType] },
		});
		const pickerResult = await pickerModal.onSubmit().catch(() => undefined);
		const newKey = pickerResult?.selection?.[0];
		if (!newKey || newKey === item.dataType) return;

		const updated = [...(this.value ?? [])];
		updated[index] = { ...item, dataType: newKey };
		this.value = updated;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onOpenWorkspace(dataTypeKey: string) {
		this.#editDataTypeModal.open({}, 'edit/' + dataTypeKey);
	}

	#onLabelChange(event: UUIInputEvent, index: number) {
		const updated = [...(this.value ?? [])];
		updated[index] = { ...updated[index], label: event.target.value as string };
		this.value = updated;
		this.dispatchEvent(new UmbChangeEvent());
	}

	async #onRemove(index: number) {
		const confirm = await umbConfirmModal(this, {
			color: 'danger',
			headline: this.localize.term('contentment_removeItemHeadline'),
			content: this.localize.term('contentment_removeItemMessage'),
			confirmLabel: this.localize.term('contentment_removeItemButton'),
		}).catch(() => false);

		if (confirm === false) return;

		const updated = [...(this.value ?? [])];
		updated.splice(index, 1);
		this.value = updated;
		this.dispatchEvent(new UmbChangeEvent());
	}

	#onSortEnd(event: ContentmentSortEndEvent) {
		if (event.newIndex === undefined || event.oldIndex === undefined) return;
		const updated = [...(this.value ?? [])];
		updated.splice(event.newIndex, 0, updated.splice(event.oldIndex, 1)[0]);
		this.value = updated;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		return html`${this.#renderList()}${this.#renderAddButton()}`;
	}

	#renderAddButton() {
		return html`
			<uui-button
				id="btn-add"
				label=${this.localize.term('contentment_addColumn')}
				look="placeholder"
				@click=${this.#onAdd}></uui-button>
		`;
	}

	#renderList() {
		if (!this.value?.length) return nothing;
		return html`
			<contentment-sortable-list id="list" item-selector=".item" handle-selector=".handle" @sort-end=${this.#onSortEnd}>
				${repeat(
					this.value,
					(item) => item.key,
					(item, index) => this.#renderItem(item, index),
				)}
			</contentment-sortable-list>
		`;
	}

	#renderItem(item: InputListColumnConfig, index: number) {
		return html`
			<contentment-sortable-list-item class="item" @delete=${() => this.#onRemove(index)}>
				<umb-ref-data-type .dataTypeId=${item.dataType} standalone @open=${() => this.#onOpenWorkspace(item.dataType)}>
					<uui-action-bar slot="actions">
						<uui-button
							compact
							label=${this.localize.term('general_change')}
							@click=${(e: Event) => this.#onChangeDataType(e, index)}></uui-button>
					</uui-action-bar>
				</umb-ref-data-type>
				<uui-input
					placeholder=${this.localize.term('contentment_enterLabel')}
					.value=${item.label}
					@change=${(e: UUIInputEvent) => this.#onLabelChange(e, index)}>
				</uui-input>
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
		`,
	];
}

export { ContentmentPropertyEditorUIInputListColumnsConfigurationElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-input-list-columns-configuration': ContentmentPropertyEditorUIInputListColumnsConfigurationElement;
	}
}
