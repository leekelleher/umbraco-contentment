// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentBlock, ContentBlockType } from './types.js';
import type { UmbPropertyDatasetElement, UmbPropertyValueData } from '@umbraco-cms/backoffice/property';
import type { UmbPropertyTypeModel, UmbPropertyTypeContainerModel } from '@umbraco-cms/backoffice/content-type';
import { css, customElement, html, nothing, repeat, state, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbContentTypeStructureManager, UmbContentTypeContainerStructureHelper } from '@umbraco-cms/backoffice/content-type';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UMB_DOCUMENT_TYPE_DETAIL_REPOSITORY_ALIAS } from '@umbraco-cms/backoffice/document-type';
import { ContentmentContentBlockWorkspaceContext } from './content-block-workspace.context.js';
import { CONTENTMENT_CONTENT_BLOCK_WORKSPACE_CONTEXT } from './content-block-workspace.context-token.js';

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
	private _tabs: Array<UmbPropertyTypeContainerModel> = [];

	@state()
	private _groups: Map<string, Array<UmbPropertyTypeContainerModel>> = new Map();

	@state()
	private _properties: Map<string | null, Array<UmbPropertyTypeModel>> = new Map();

	@state()
	private _activeTabId?: string;

	@state()
	private _values?: Array<UmbPropertyValueData>;

	@state()
	private _loading = true;

	@state()
	private _error?: string;

	#structureManager?: UmbContentTypeStructureManager;
	#tabsStructureHelper = new UmbContentTypeContainerStructureHelper(this);
	#workspaceContext?: ContentmentContentBlockWorkspaceContext;

	constructor() {
		super();

		// Create and provide the workspace context
		this.#workspaceContext = new ContentmentContentBlockWorkspaceContext(this);
		this.provideContext(CONTENTMENT_CONTENT_BLOCK_WORKSPACE_CONTEXT, this.#workspaceContext);

		// Setup tabs structure helper
		this.#tabsStructureHelper.setIsRoot(true);
		this.#tabsStructureHelper.setContainerChildType('Tab');

		// Observe tabs from structure helper (handles compositions)
		this.observe(this.#tabsStructureHelper.mergedContainers, (tabs) => {
			this._tabs = tabs;
			// Set first tab as active if we have tabs
			if (tabs.length > 0 && !this._activeTabId) {
				this._activeTabId = tabs[0].id;
			}
		});
	}

	override async connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._elementType = this.data.elementType;
		this._block = this.data.item;

		// Set the block data on the workspace context
		if (this.#workspaceContext) {
			this.#workspaceContext.setData(this._block);
		}

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

			// Set structure manager on tabs helper to handle compositions
			this.#tabsStructureHelper.setStructureManager(this.#structureManager);

			// Load the element type structure by its key (GUID)
			const response = await this.#structureManager.loadType(this._elementType.key);

			if (!response.data) {
				this._error = `Could not load element type: ${this._elementType.name}`;
				this._loading = false;
				return;
			}

			// Wait for structure to be loaded
			await this.#structureManager.whenLoaded();

			// Observe content type properties (handles compositions being loaded)
			this.observe(this.#structureManager.contentTypeProperties, (contentTypeProperties) => {
				this.#updatePropertiesStructure(contentTypeProperties);
			});

			this._loading = false;
		} catch (error) {
			console.error('Error loading element type:', error);
			this._error = `Failed to load element type: ${error}`;
			this._loading = false;
		}
	}

	async #updatePropertiesStructure(allProperties: Array<UmbPropertyTypeModel>) {
		// Sort properties by sort order
		const sortedProperties = allProperties.sort((a, b) => a.sortOrder - b.sortOrder);

		// Clear existing properties map
		this._properties.clear();

		// Load groups and properties for each tab
		for (const tab of this._tabs) {
			// Get groups for this tab
			const groups = await this.#structureManager?.getOwnerContainers('Group', tab.id);
			if (groups) {
				this._groups.set(tab.id, groups);

				// Get properties for each group
				for (const group of groups) {
					const groupProps = await this.#getPropertiesForContainer(group.id);
					this._properties.set(group.id, groupProps);
				}
			}

			// Get properties directly under the tab (not in any group)
			const tabProps = await this.#getPropertiesForContainer(tab.id);
			this._properties.set(tab.id, tabProps);
		}

		// Get root properties (not in any tab)
		const rootProps = await this.#getPropertiesForContainer(null);
		this._properties.set(null, rootProps);

		// Initialize property values from the block or set defaults (only if not already initialized)
		if (!this._values || this._values.length === 0) {
			this._values = sortedProperties.map((property) => ({
				alias: property.alias,
				value: this._block?.value[property.alias] ?? null,
			}));
		} else {
			// Update values array with any new properties from compositions
			const existingAliases = new Set(this._values.map(v => v.alias));
			const newProperties = sortedProperties.filter(p => !existingAliases.has(p.alias));
			
			if (newProperties.length > 0) {
				this._values = [
					...this._values,
					...newProperties.map((property) => ({
						alias: property.alias,
						value: this._block?.value[property.alias] ?? null,
					}))
				];
			}
		}
	}

	async #getPropertiesForContainer(containerId: string | null): Promise<Array<UmbPropertyTypeModel>> {
		if (!this.#structureManager) return [];

		const obs =
			containerId === null
				? this.#structureManager!.rootPropertyStructures()
				: this.#structureManager!.propertyStructuresOf(containerId);

		return this.observe(obs, (properties) => properties).asPromise();
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

		// Update the workspace context
		if (this.#workspaceContext) {
			this.#workspaceContext.setData(result);
		}

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
				<umb-property-dataset .value=${this._values!} @change=${this.#onChange}>
					${when(
						this._tabs.length > 0,
						() => this.#renderTabs(),
						() => this.#renderRootProperties()
					)}
				</umb-property-dataset>
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

	#renderTabs() {
		if (!this._tabs.length) return nothing;

		return html`
			<uui-tab-group>
				${repeat(
					this._tabs,
					(tab) => tab.id,
					(tab) => html`
						<uui-tab
							.label=${tab.name}
							.active=${this._activeTabId === tab.id}
							@click=${() => (this._activeTabId = tab.id)}>
							${tab.name}
						</uui-tab>
					`
				)}
			</uui-tab-group>
			${repeat(
				this._tabs,
				(tab) => tab.id,
				(tab) => html`<div ?active=${this._activeTabId === tab.id}>${this.#renderTab(tab)}</div>`
			)}
		`;
	}

	#renderTab(tab: UmbPropertyTypeContainerModel) {
		const groups = this._groups.get(tab.id) || [];
		const tabProperties = this._properties.get(tab.id) || [];

		return html`
			${when(
				groups.length > 0,
				() => html`
					${repeat(
						groups,
						(group) => group.id,
						(group) => this.#renderGroup(group)
					)}
				`
			)}
			${when(
				tabProperties.length > 0,
				() => html`
					<uui-box>
						${repeat(
							tabProperties,
							(property) => property.unique,
							(property) => this.#renderProperty(property)
						)}
					</uui-box>
				`
			)}
			${when(
				groups.length === 0 && tabProperties.length === 0,
				() => html`<uui-box><p>This tab has no properties.</p></uui-box>`
			)}
		`;
	}

	#renderGroup(group: UmbPropertyTypeContainerModel) {
		const properties = this._properties.get(group.id) || [];

		if (properties.length === 0) return nothing;

		return html`
			<uui-box .headline=${group.name}>
				${repeat(
					properties,
					(property) => property.unique,
					(property) => this.#renderProperty(property)
				)}
			</uui-box>
		`;
	}

	#renderRootProperties() {
		const properties = this._properties.get(null) || [];

		return html`
			${when(
				properties.length > 0,
				() => html`
					<uui-box>
						${repeat(
							properties,
							(property) => property.unique,
							(property) => this.#renderProperty(property)
						)}
					</uui-box>
				`,
				() => html`<uui-box><p>This element type has no properties to edit.</p></uui-box>`
			)}
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
			uui-tab-group {
				height: 60px;
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
