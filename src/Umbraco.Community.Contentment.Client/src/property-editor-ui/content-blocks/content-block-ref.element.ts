// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher

import { css, customElement, html, nothing, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { ContentmentContentBlockValue } from '../types.js';

import '@umbraco-cms/backoffice/ufm';

@customElement('contentment-content-block-ref')
export class ContentmentContentBlockRefElement extends UmbLitElement {
	@property({ attribute: false })
	public item?: ContentmentContentBlockValue;

	@property({ type: Number })
	public index?: number;

	@property({ type: String })
	public name?: string;

	@property({ type: String })
	public icon?: string;

	@property({ type: String })
	public nameTemplate?: string;

	@property({ type: Boolean })
	public unsupported = false;

	@property({ type: Boolean, reflect: true })
	public readonly = false;

	override render() {
		if (!this.item) return nothing;

		return html`
			<uui-ref-node standalone .readonly=${this.unsupported || this.readonly} @open=${this.#onEdit}>
				<umb-icon slot="icon" .name=${this.unsupported ? 'icon-alert' : (this.icon ?? 'icon-document')}></umb-icon>
				<umb-ufm-render
					slot="name"
					inline
					.markdown=${this.nameTemplate || this.name || this.item.elementType}
					.value=${{ ...this.item.value, $index: this.index }}></umb-ufm-render>
				${when(
					!this.readonly,
					() => html`
						<uui-action-bar slot="actions">
							${when(
								!this.unsupported,
								() => html`
									<uui-button label=${this.localize.term('general_edit')} @click=${this.#onEdit}>
										<uui-icon name="icon-edit"></uui-icon>
									</uui-button>
								`,
							)}
							<uui-button label=${this.localize.term('general_delete')} @click=${this.#onDelete}>
								<uui-icon name="icon-remove"></uui-icon>
							</uui-button>
						</uui-action-bar>
					`,
				)}
			</uui-ref-node>
		`;
	}

	#onEdit(e: Event) {
		e.stopPropagation();
		this.dispatchEvent(new CustomEvent('edit', { bubbles: false }));
	}

	#onDelete(e: Event) {
		e.stopPropagation();
		this.dispatchEvent(new CustomEvent('delete', { bubbles: false }));
	}

	static override styles = [
		css`
			:host {
				display: block;
				margin-bottom: 1px;
				--contentment-entry-actions-opacity: 0;
			}

			:host(:hover),
			:host(:focus-within) {
				--contentment-entry-actions-opacity: 1;
			}

			uui-action-bar {
				opacity: var(--contentment-entry-actions-opacity, 0);
				transition: opacity 120ms;
			}
		`,
	];
}

export { ContentmentContentBlockRefElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-content-block-ref': ContentmentContentBlockRefElement;
	}
}
