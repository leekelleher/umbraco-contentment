// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentBlock, ContentBlockType } from './types.js';
import type { UmbPropertyDatasetElement, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyTypeModel } from '@umbraco-cms/backoffice/content-type';
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
import { UmbContentTypeStructureManager } from '@umbraco-cms/backoffice/content-type';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UMB_DOCUMENT_TYPE_DETAIL_REPOSITORY_ALIAS } from '@umbraco-cms/backoffice/document-type';

interface ContentBlockWorkspaceModalData {
	item: ContentBlock;
	elementType: ContentBlockType;
}

export const CONTENTMENT_CONTENT_BLOCK_WORKSPACE_MODAL = new UmbModalToken<
	ContentBlockWorkspaceModalData,
	ContentBlock
>('Umb.Contentment.Modal.ContentBlock.Workspace', {
	modal: {
		type: 'sidebar',
		size: 'medium',
	},
});

/**
 * Modal for editing content blocks with full property support.
 *
 * This implementation:
 * - Loads the element type structure using UmbContentTypeStructureManager
 * - Renders properties using umb-property elements
 * - Handles property value changes through property dataset
 * - Supports element type compositions
 *
 * Note: This implementation does not include Block List settings or variants,
 * focusing on simple element editing for Content Blocks.
 */
@customElement('contentment-property-editor-ui-content-block-workspace-modal')
export class ContentmentPropertyEditorUIContentBlockWorkspaceModalElement extends UmbModalBaseElement<
	ContentBlockWorkspaceModalData,
	ContentBlock
> {
	@state()
	private _elementType?: ContentBlockType;

	@state()
	private _block?: ContentBlock;

	@state()
	private _properties: Array<UmbPropertyTypeModel> = [];

	@state()
	private _values?: Array<UmbPropertyValueData>;

	@state()
	private _loading = true;

	@state()
	private _error?: string;

	#structureManager?: UmbContentTypeStructureManager;

	override async connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._elementType = this.data.elementType;
		this._block = this.data.item;

		await this.#loadElementType();
	}

	async #loadElementType() {
		if (!this._elementType) {
			this._error = 'No element type provided';
			this._loading = false;
			return;
		}

		try {
			// Create structure manager for the element type
			this.#structureManager = new UmbContentTypeStructureManager(this, UMB_DOCUMENT_TYPE_DETAIL_REPOSITORY_ALIAS);

			// Load the element type structure by its key (GUID)
			const response = await this.#structureManager.loadType(this._elementType.key);

			if (!response.data) {
				this._error = `Could not load element type: ${this._elementType.name}`;
				this._loading = false;
				return;
			}

			// Get all properties including from compositions
			const allProperties = await this.#structureManager.getContentTypeProperties();
			this._properties = allProperties;

			// Initialize property values from the block or set defaults
			this._values = allProperties.map((property) => ({
				alias: property.alias,
				value: this._block?.value[property.alias] ?? null,
			}));

			this._loading = false;
		} catch (error) {
			console.error('Error loading element type:', error);
			this._error = `Failed to load element type: ${error}`;
			this._loading = false;
		}
	}

	#onChange(event: Event & { target: UmbPropertyDatasetElement }) {
		this._values = event.target.value;
	}

	#onSubmit() {
		if (!this.data || !this._block || !this._values) return;

		// Build the updated block with property values
		const result: ContentBlock = {
			elementType: this.data.elementType.key,
			key: this._block.key,
			value: Object.fromEntries(this._values.map((v) => [v.alias, v.value])),
		};

		this.value = result;
		this._submitModal();
	}

	override render() {
		if (this._loading) {
			return html`
				<umb-body-layout headline="Loading...">
					<uui-box>
						<uui-loader-bar></uui-loader-bar>
						<p>Loading element type structure...</p>
					</uui-box>
				</umb-body-layout>
			`;
		}

		if (this._error || !this._elementType || !this._block) {
			return html`
				<umb-body-layout headline="Error">
					<uui-box>
						<p><strong>Error:</strong> ${this._error ?? 'Failed to initialize'}</p>
					</uui-box>
					<div slot="actions">
						<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
					</div>
				</umb-body-layout>
			`;
		}

		const isNew = Object.keys(this._block.value).length === 0;
		const headline = `${isNew ? 'Create' : 'Edit'} ${this._elementType.name}`;

		return html`
			<umb-body-layout headline=${headline}>
				<uui-box>
					${when(
						this._properties.length > 0,
						() => html`
							<umb-property-dataset .value=${this._values!} @change=${this.#onChange}>
								${repeat(
									this._properties,
									(property) => property.id,
									(property) => this.#renderProperty(property)
								)}
							</umb-property-dataset>
						`,
						() => html`<p>This element type has no properties to edit.</p>`
					)}
				</uui-box>
				<div slot="actions">
					<uui-button label=${this.localize.term('general_cancel')} @click=${this._rejectModal}></uui-button>
					<uui-button
						color="positive"
						look="primary"
						label=${this.localize.term('buttons_save')}
						@click=${this.#onSubmit}></uui-button>
				</div>
			</umb-body-layout>
		`;
	}

	#renderProperty(property: UmbPropertyTypeModel) {
		return html`
			<umb-property-type-based-property .ownerEntityType=${this._elementType?.key} .property=${property}>
			</umb-property-type-based-property>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			umb-property {
				font-size: 14px;
			}

			uui-box {
				padding: var(--uui-size-space-5);
			}
		`,
	];
}

export { ContentmentPropertyEditorUIContentBlockWorkspaceModalElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-content-block-workspace-modal': ContentmentPropertyEditorUIContentBlockWorkspaceModalElement;
	}
}
