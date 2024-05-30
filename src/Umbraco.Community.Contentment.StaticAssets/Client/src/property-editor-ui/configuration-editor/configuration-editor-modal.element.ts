// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import {
	css,
	customElement,
	html,
	ifDefined,
	nothing,
	repeat,
	state,
	when,
} from '@umbraco-cms/backoffice/external/lit';
import { debounce } from '@umbraco-cms/backoffice/utils';
import { umbFocus } from '@umbraco-cms/backoffice/lit-element';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UUIInputEvent } from '@umbraco-cms/backoffice/external/uui';
import { UmbPropertyDatasetElement, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';

interface ContentmentConfigurationEditorModalData {
	items: Array<ContentmentConfigurationEditorModel>;
}

export const CONTENTMENT_CONFIGURATION_EDITOR_MODAL = new UmbModalToken<
	ContentmentConfigurationEditorModalData,
	ContentmentConfigurationEditorValue
>('Umb.Contentment.Modal.ConfigurationEditor', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});

const elementName = 'contentment-property-editor-ui-configuration-editor-modal';

@customElement(elementName)
export class ContentmentPropertyEditorUIConfigurationEditorModalElement extends UmbModalBaseElement<
	ContentmentConfigurationEditorModalData,
	ContentmentConfigurationEditorValue
> {
	#emptyGroup = (Math.random() + 1).toString(36).substring(7);

	@state()
	private _grouped?: Array<{ alias: string; items: Array<ContentmentConfigurationEditorModel> }>;

	@state()
	private _editMode = false;

	@state()
	private _item?: ContentmentConfigurationEditorModel;

	@state()
	private _propertyData?: Array<UmbPropertyValueData>;

	constructor() {
		super();
	}

	connectedCallback() {
		super.connectedCallback();

		this.#groupItems(this.data?.items);
	}

	#debouncedFilter = debounce((query: string) => {
		if (!this.data) return;
		const items = query ? this.data.items.filter((item) => item.name.toLowerCase().includes(query)) : this.data.items;
		this.#groupItems(items);
	}, 500);

	#groupItems(items: Array<ContentmentConfigurationEditorModel> | null | undefined) {
		if (!items) return;

		// eslint-disable-next-line @typescript-eslint/ban-ts-comment
		// @ts-expect-error
		const grouped = Object.groupBy(
			items,
			(item: ContentmentConfigurationEditorModel) => item.group ?? this.#emptyGroup
		);
		const entries = Object.entries(grouped) as Array<[string, Array<ContentmentConfigurationEditorModel>]>;
		this._grouped = entries.map(([alias, items]) => ({ alias, items }));
	}

	#onChange(event: Event & { target: UmbPropertyDatasetElement }) {
		console.log('#onChange', event.target.value);
		this._propertyData = event.target.value;
	}

	#onChoose(item: ContentmentConfigurationEditorModel) {
		console.log('#onChoose', item);

		if (!item.fields?.length) {
			this.value = { key: item.key, value: {} };
			this._submitModal();
			return;
		}

		this._propertyData = item.defaultValues
			? Object.entries(item.defaultValues).map(([alias, value]) => ({ alias, value }))
			: [];

		this._item = item;
		this._editMode = true;
	}

	#onInput(event: UUIInputEvent) {
		const query = (event.target.value as string) || '';
		this.#debouncedFilter(query.toLowerCase());
	}

	#onSubmit(item: ContentmentConfigurationEditorModel) {
		console.log('#onSubmit', item, this._propertyData);
		this.value = { key: item.key, value: Object.fromEntries(this._propertyData?.map((x) => [x.alias, x.value]) ?? []) };
		this._submitModal();
	}

	render() {
		return this._editMode ? this.#renderEdit() : this.#renderChoose();
	}

	#renderChoose() {
		return html`
			<umb-body-layout headline="Choose...">
				${this.#renderFilter()} ${this.#renderGroups()}
				<div slot="actions">
					<uui-button label=${this.localize.term('general_close')} @click=${this._rejectModal}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderEdit() {
		if (!this._item) return nothing;
		const fields = this._item.fields ?? [];
		if (!this._propertyData) this._propertyData = [];
		return html`
			<umb-body-layout headline="Configure ${this._item.name}">
				<uui-box>
					${when(
						fields.length,
						() => html`
							<umb-property-dataset .value=${this._propertyData!} @change=${this.#onChange}>
								${repeat(
									fields,
									(field) => field.key,
									(field) => html`
										<umb-property
											alias=${field.key}
											label=${field.name}
											description=${field.description}
											property-editor-ui-alias=${field.propertyEditorUiAlias}
											.value=${this._propertyData?.find((x) => x.alias === field.key)?.value}
											.config=${field.config
												? Object.entries(field.config).map(([alias, value]) => ({ alias, value }))
												: []}>
										</umb-property>
									`
								)}
							</umb-property-dataset>
						`,
						() => html`<p>There are no fields for this item.</p>`
					)}
				</uui-box>

				<details>
					<summary style="cursor:pointer;">Fields</summary>
					<uui-textarea auto-height .value=${JSON.stringify(this._item, null, 4)}></uui-textarea>
				</details>

				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
					<uui-button
						color="positive"
						look="primary"
						label=${this.localize.term('bulk_done')}
						@click=${() => this.#onSubmit(this._item!)}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderFilter() {
		return html`
			<uui-input
				type="search"
				id="filter"
				@input=${this.#onInput}
				placeholder="Type to filter..."
				label="Type to filter"
				${umbFocus()}>
				<uui-icon name="search" slot="prepend" id="filter-icon"></uui-icon>
			</uui-input>
		`;
	}

	#renderGroups() {
		if (!this._grouped?.length) return html`<uui-box><p>No items found</p></uui-box>`;
		return html`
			<uui-box>
				${repeat(
					this._grouped,
					(group) => group.alias,
					(group) => html`
						${when(group.alias !== this.#emptyGroup, () => html`<h4>${group.alias}</h4>`)}
						<uui-ref-list>
							${repeat(
								group.items,
								(item) => item.key,
								(item) => html`
									<uui-ref-node
										name=${item.name}
										detail=${ifDefined(item.description)}
										@open=${() => this.#onChoose(item)}>
										${when(item.icon, () => html`<uui-icon slot="icon" name=${item.icon!}></uui-icon>`)}
									</uui-ref-node>
								`
							)}
						</uui-ref-list>
					`
				)}
			</uui-box>
		`;
	}

	static styles = [
		UmbTextStyles,
		css`
			#filter {
				width: 100%;
				margin-bottom: var(--uui-size-space-4);
			}

			#filter-icon {
				display: flex;
				color: var(--uui-color-border);
				height: 100%;
				padding-left: var(--uui-size-space-2);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[elementName]: ContentmentPropertyEditorUIConfigurationEditorModalElement;
	}
}
