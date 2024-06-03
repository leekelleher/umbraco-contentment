// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { ContentmentConfigurationEditorModel, ContentmentConfigurationEditorValue } from '../types.js';
import { css, customElement, html, nothing, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UmbPropertyDatasetElement, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';

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

const elementName = 'contentment-property-editor-ui-configuration-editor-workspace-modal';

@customElement(elementName)
export class ContentmentPropertyEditorUIConfigurationEditorWorkspaceModalElement extends UmbModalBaseElement<
	ContentmentConfigurationEditorWorkspaceModalData,
	ContentmentConfigurationEditorValue
> {
	@state()
	private _item?: ContentmentConfigurationEditorModel;

	@state()
	private _values?: Array<UmbPropertyValueData>;

	constructor() {
		super();
	}

	connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._item = this.data.model;
		this._values = Object.entries(this.data.item.value).map(([alias, value]) => ({ alias, value }));
	}

	#onChange(event: Event & { target: UmbPropertyDatasetElement }) {
		this._values = event.target.value;
	}

	#onSubmit(item: ContentmentConfigurationEditorModel) {
		this.value = { key: item.key, value: Object.fromEntries(this._values?.map((x) => [x.alias, x.value]) ?? []) };
		this._submitModal();
	}

	render() {
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

	static styles = [
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
		[elementName]: ContentmentPropertyEditorUIConfigurationEditorWorkspaceModalElement;
	}
}
