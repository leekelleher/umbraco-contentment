// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { css, customElement, html, nothing, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UmbPropertyDatasetElement } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import { parseBoolean, parseInt } from '../../utils/index.js';

interface ContentmentConfigurationEditorWorkspaceModalData {
	item: ContentmentConfigurationEditorValue;
	model: ContentmentConfigurationEditorModel;
}

export const CONTENTMENT_CONFIGURATION_EDITOR_WORKSPACE_MODAL = new UmbModalToken<
	ContentmentConfigurationEditorWorkspaceModalData,
	ContentmentConfigurationEditorValue
>('Umb.Contentment.Modal.ConfigurationEditor.Workspace', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});

@customElement('contentment-property-editor-ui-configuration-editor-workspace-modal')
export class ContentmentPropertyEditorUIConfigurationEditorWorkspaceModalElement extends UmbModalBaseElement<
	ContentmentConfigurationEditorWorkspaceModalData,
	ContentmentConfigurationEditorValue
> {
	// In v13, some boolean values were stored as numeric strings ("0" or "1") instead of booleans.
	// or numeric values were stores as strings. These may be unsupported by the new property-editors,
	// this is a workaround to convert those values to the correct object-type. [LK]
	#legacyValueConverters: Record<string, (x: unknown) => unknown> = {
		'Umb.PropertyEditorUi.Integer': (x) => parseInt(x),
		'Umb.PropertyEditorUi.Toggle': (x) => parseBoolean(x),
		'Umb.Contentment.PropertyEditorUi.NumberInput': (x) => parseInt(x),
	};

	@state()
	private _item?: ContentmentConfigurationEditorModel;

	@state()
	private _values?: Array<UmbPropertyValueData>;

	override connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._item = this.data.model;

		const values = { ...this.data.item.value };
		(this._item.fields ?? []).forEach((field) => {
			if (!field.key) return;
			const value = values[field.key];
			if (!field.propertyEditorUiAlias) return;
			const converter = this.#legacyValueConverters[field.propertyEditorUiAlias];
			if (converter) {
				values[field.key] = converter(value);
			}
		});

		this._values = Object.entries(values).map(([alias, value]) => ({ alias, value }));
	}

	#onChange(event: Event & { target: UmbPropertyDatasetElement }) {
		this._values = event.target.value;
	}

	#onSubmit(item: ContentmentConfigurationEditorModel) {
		this.value = { key: item.key, value: Object.fromEntries(this._values?.map((x) => [x.alias, x.value]) ?? []) };
		this._submitModal();
	}

	override render() {
		if (!this._item) return nothing;
		const fields = this._item.fields ?? [];
		if (!this._values) this._values = [];
		return html`
			<umb-body-layout headline="Configure ${this._item.name}">
				<uui-box>
					${when(
						fields.length,
						() => html`
							<umb-property-dataset .value=${this._values!} @change=${this.#onChange}>
								${repeat(
									fields,
									(field) => field.key,
									(field) => html`
										<umb-property
											alias=${field.key}
											label=${field.name}
											description=${field.description}
											property-editor-ui-alias=${field.propertyEditorUiAlias}
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

	static override styles = [
		UmbTextStyles,
		css`
			umb-property {
				font-size: 14px;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIConfigurationEditorWorkspaceModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-configuration-editor-workspace-modal': ContentmentPropertyEditorUIConfigurationEditorWorkspaceModalElement;
	}
}
