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
 * This is a placeholder implementation that shows the structure.
 * A full implementation would require integrating with Umbraco's element editing APIs.
 */
@customElement('contentment-property-editor-ui-content-block-workspace-modal')
export class ContentmentPropertyEditorUIContentBlockWorkspaceModalElement extends UmbModalBaseElement<
	ContentBlockWorkspaceModalData,
	ContentBlock
> {
	@state()
	private _elementType?: ContentBlockType;

	override connectedCallback() {
		super.connectedCallback();

		if (!this.data) return;

		this._elementType = this.data.elementType;
	}

	#onSubmit() {
		if (!this.data) return;

		// For now, just return the item unchanged
		// In a full implementation, this would collect property values
		this.value = this.data.item;
		this._submitModal();
	}

	override render() {
		if (!this._elementType) return nothing;

		return html`
			<umb-body-layout headline="Edit ${this._elementType.name}">
				<uui-box>
					<p>Content block editing workspace</p>
					<p><em>Element type: ${this._elementType.name}</em></p>
					<p><em>This is a simplified implementation placeholder.</em></p>
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
			umb-property {
				font-size: 14px;
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
