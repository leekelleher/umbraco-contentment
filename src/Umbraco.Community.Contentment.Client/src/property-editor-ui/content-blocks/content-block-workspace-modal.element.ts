// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import type { ContentBlock, ContentBlockType } from './types.js';
import {
	css,
	customElement,
	html,
	nothing,
	state,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbModalBaseElement, UmbModalToken } from '@umbraco-cms/backoffice/modal';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';

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
 * Simple modal for editing content blocks.
 * 
 * This provides a basic structure for content block editing. The full implementation
 * of element property editing would require deep integration with Umbraco's content type
 * and property editing systems, which adds significant complexity.
 * 
 * For now, this modal provides:
 * - A consistent UI for block creation/editing
 * - Proper modal token registration
 * - A foundation for future enhancements
 * 
 * To fully implement property editing, consider:
 * - Using Umbraco's UmbPropertyDatasetElement for property rendering
 * - Fetching element type scaffold from content type repository
 * - Handling property value serialization/deserialization
 * - Supporting variant content and cultures
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

	override connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._elementType = this.data.elementType;
		this._block = this.data.item;
	}

	#onSubmit() {
		if (!this.data || !this._block) return;

		// Return the block (potentially with edited properties in a full implementation)
		this.value = this._block;
		this._submitModal();
	}

	override render() {
		if (!this._elementType || !this._block) return nothing;

		const isNew = Object.keys(this._block.value).length === 0;

		return html`
			<umb-body-layout headline="${isNew ? 'Create' : 'Edit'} ${this._elementType.name}">
				<uui-box>
					<div class="element-info">
						<h3>Element Type Information</h3>
						<dl>
							<dt>Name:</dt>
							<dd>${this._elementType.name}</dd>
							${this._elementType.description
								? html`
										<dt>Description:</dt>
										<dd>${this._elementType.description}</dd>
								  `
								: nothing}
							<dt>Alias:</dt>
							<dd><code>${this._elementType.alias}</code></dd>
							<dt>Key:</dt>
							<dd><code>${this._elementType.key}</code></dd>
						</dl>
					</div>

					<contentment-info-box type="info" heading="Implementation Note">
						<p>
							This is a simplified content block modal. Full property editing would require integration with
							Umbraco's content type and property editing systems.
						</p>
						<p>
							To implement full editing, the workspace would need to:
						</p>
						<ul>
							<li>Fetch the element type structure with property definitions</li>
							<li>Render each property using appropriate property editor UIs</li>
							<li>Handle property value changes and validation</li>
							<li>Support content variations if needed</li>
						</ul>
						<p>
							For now, blocks can be created with their element type association, and the structure is
							preserved for use in rendering.
						</p>
					</contentment-info-box>
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

	static override styles = [
		UmbTextStyles,
		css`
			.element-info {
				margin-bottom: var(--uui-size-space-4);
			}

			.element-info h3 {
				margin-top: 0;
				margin-bottom: var(--uui-size-space-3);
			}

			.element-info dl {
				display: grid;
				grid-template-columns: auto 1fr;
				gap: var(--uui-size-space-2) var(--uui-size-space-4);
				margin: 0;
			}

			.element-info dt {
				font-weight: bold;
			}

			.element-info dd {
				margin: 0;
			}

			.element-info code {
				background-color: var(--uui-color-surface-alt);
				padding: 2px 6px;
				border-radius: 3px;
				font-family: monospace;
				font-size: 0.9em;
			}

			contentment-info-box {
				margin-top: var(--uui-size-space-4);
			}

			contentment-info-box ul {
				margin: var(--uui-size-space-2) 0;
				padding-left: var(--uui-size-space-5);
			}

			contentment-info-box li {
				margin: var(--uui-size-space-1) 0;
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
