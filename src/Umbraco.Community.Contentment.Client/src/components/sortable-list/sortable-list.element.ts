// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher
// Credit: https://lit.dev/playground/#gist=242f45fd2dbe21ecb6902f144686aae8

import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { Sortable } from '@umbraco-community/backoffice/external/sortablejs';
import type { SortableEvent } from '@umbraco-community/backoffice/external/sortablejs';

@customElement('contentment-sortable-list')
export class ContentmentSortableListElement extends UmbLitElement {
	@property({ type: Boolean })
	disabled: boolean = false;

	@property({ attribute: 'handle-selector' })
	handleSelector?: string;

	@property({ attribute: 'item-selector' })
	itemSelector?: string;

	protected override firstUpdated() {
		let before: ChildNode | null;
		Sortable.create(this, {
			animation: 150,
			disabled: this.disabled,
			draggable: this.itemSelector,
			handle: this.handleSelector,
			onStart: (e: SortableEvent) => {
				before = e.item.previousSibling;
			},
			onEnd: (e: SortableEvent) => {
				before?.after(e.item);
				this.dispatchEvent(new CustomEvent('sort-end', { detail: e }));
			},
		});
	}

	override render() {
		return html`<slot></slot>`;
	}

	static override styles = [
		css`
			:host(.uui-ref-list) {
				/* Copied from https://github.com/umbraco/Umbraco.UI/blob/v1.12.2/packages/uui-ref-list/lib/uui-ref-list.element.ts#L19-L29 */
				::slotted(*:not(:first-child)) {
					margin-top: 1px;
				}
				::slotted(*:not(:first-child))::before {
					content: '';
					position: absolute;
					top: -1px;
					left: 0;
					right: 0;
					border-top: 1px solid var(--uui-color-border);
				}
			}
		`,
	];
}

export { ContentmentSortableListElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-sortable-list': ContentmentSortableListElement;
	}
}
