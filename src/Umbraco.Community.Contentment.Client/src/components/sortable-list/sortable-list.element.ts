// SPDX-License-Identifier: MPL-2.0
// Copyright © 2025 Lee Kelleher
// Adapted from: https://lit.dev/playground/#gist=242f45fd2dbe21ecb6902f144686aae8

import { css, customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { ContentmentSortEndEvent } from './sort-end.event.js';
import { Sortable } from '../../external/sortablejs/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UUIBlinkAnimationValue, UUIBlinkKeyframes } from '@umbraco-cms/backoffice/external/uui';
import type { SortableEvent } from '../../external/sortablejs/index.js';

@customElement('contentment-sortable-list')
export default class ContentmentSortableListElement extends UmbLitElement {
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
			ghostClass: 'umb-drag-placeholder',
			onStart: (event: SortableEvent) => {
				before = event.item.previousSibling;
			},
			onEnd: (event: SortableEvent) => {
				before?.after(event.item);
				this.dispatchEvent(new ContentmentSortEndEvent(event.newIndex, event.oldIndex));
			},
		});
	}

	override render() {
		return html`<slot></slot>`;
	}

	static override styles = [
		UUIBlinkKeyframes,
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

			::slotted(.umb-drag-placeholder) {
				animation: ${UUIBlinkAnimationValue};
				background-color: color-mix(in srgb, var(--uui-color-interactive-emphasis) 15%, transparent) !important;
				border-radius: var(--uui-border-radius);
				border: 2px solid var(--uui-color-interactive-emphasis);
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-sortable-list': ContentmentSortableListElement;
	}
}
