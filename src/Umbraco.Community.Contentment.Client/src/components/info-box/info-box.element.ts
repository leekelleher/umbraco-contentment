// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import {
	classMap,
	css,
	customElement,
	html,
	property,
	state,
	styleMap,
	when,
	unsafeHTML,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UUIInterfaceColor } from '@umbraco-cms/backoffice/external/uui';

// TODO: [LK] This is a work-in-progress, likely to change in the v6-beta.
export type ContentmentInfoBoxElementType =
	| Exclude<UUIInterfaceColor, '' | 'invalid'>
	| 'border'
	| 'current'
	| 'divider'
	| 'selected'
	| 'transparent';

@customElement('contentment-info-box')
export default class ContentmentInfoBoxElement extends UmbLitElement {
	@state()
	private _hasSlottedContent = false;

	@property({ type: Boolean })
	compact: boolean = false;

	/** @deprecated Use `headline` property. To be removed in Contentment 8.0. */
	@property()
	public set heading(value: string | undefined) {
		this.headline = value;
	}
	public get heading(): string | undefined {
		return this.headline;
	}

	@property()
	headline?: string;

	@property()
	icon?: string;

	@property()
	message?: string;

	@property()
	type?: ContentmentInfoBoxElementType = 'transparent';

	#getClasses() {
		return classMap({
			'uui-text': true,
			compact: this.compact || !this.headline || (!this.message && !this._hasSlottedContent),
		});
	}

	#getStyles() {
		return styleMap({
			backgroundColor: `var(--uui-color-${this.type})`,
			color: `var(--uui-color-${this.type}-contrast)`,
			borderColor: `var(--uui-color-${this.type}-standalone)`,
			'--uui-color-interactive': `var(--uui-color-${this.type}-contrast)`,
			'--uui-color-interactive-emphasis': `var(--uui-color-${this.type}-contrast)`,
		});
	}

	#onSlotChange(event: Event & { target: HTMLSlotElement }) {
		this._hasSlottedContent = event.target.assignedNodes({ flatten: true }).length > 0;
	}

	override render() {
		return html`
			<div id="box" class=${this.#getClasses()} style=${this.#getStyles()}>
				${when(
					this.icon,
					(icon) => html`<umb-icon name=${icon} style="color: var(--uui-color-${this.type}-contrast);"></umb-icon>`
				)}
				<div>
					${when(this.headline, (headline) => html`<h5>${headline}</h5>`)}
					${when(this.message, (message) => html`<div>${unsafeHTML(message)}</div>`)}
					<slot @slotchange=${this.#onSlotChange}></slot>
				</div>
			</div>
		`;
	}

	static override styles = [
		UmbTextStyles,
		css`
			:host {
				display: block;
			}

			#box {
				display: flex;
				align-items: flex-start;
				justify-content: flex-start;
				gap: 1rem;

				background-color: var(--uui-color-surface);
				color: var(--uui-color-text);

				border-color: var(--uui-color-surface);
				border-radius: calc(var(--uui-border-radius) * 2);

				box-shadow: var(--uui-shadow-depth-1);

				padding: 1rem;

				> div {
					flex: 1;
				}

				> umb-icon {
					font-size: 2.5rem;
				}

				&.compact {
					align-items: center;

					> umb-icon {
						font-size: 2rem;
					}
				}
			}

			.uui-text p,
			.uui-text ::slotted(p) {
				margin: 0.5rem 0;

				&:only-child {
					margin: 0;
				}
			}

			.uui-text p:last-child:not(:only-child),
			.uui-text ::slotted(p:last-child:not(:only-child)) {
				margin-bottom: 0.25rem;
			}

			details > summary {
				cursor: pointer;
				font-weight: bold;
			}
		`,
	];
}

declare global {
	interface HTMLElementTagNameMap {
		'contentment-info-box': ContentmentInfoBoxElement;
	}
}
