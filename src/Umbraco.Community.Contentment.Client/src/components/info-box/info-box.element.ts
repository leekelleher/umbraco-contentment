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

export type ContentmentInfoBoxElementType = 'default' | 'info' | 'positive' | 'warning' | 'danger';

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
	type?: ContentmentInfoBoxElementType = 'default';

	#getClasses() {
		return classMap({
			'uui-text': true,
			compact: this.compact || !this.headline || (!this.message && !this._hasSlottedContent),
		});
	}

	#getStyles() {
		return styleMap({
			'--lk-info-box-background': `var(--lk-info-box-background-${this.type})`,
			'--lk-info-box-foreground': `var(--lk-info-box-foreground-${this.type})`,
		});
	}

	#onSlotChange(event: Event & { target: HTMLSlotElement }) {
		this._hasSlottedContent = event.target.assignedNodes({ flatten: true }).length > 0;
	}

	override render() {
		return html`
			<div id="box" class=${this.#getClasses()} style=${this.#getStyles()}>
				${when(this.icon, (icon) => html`<umb-icon name=${icon}></umb-icon>`)}
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
				--lk-info-box-background: var(--uui-color-background);
				--lk-info-box-foreground: var(--uui-color-text);

				--lk-info-box-background-default: var(--uui-color-surface);
				--lk-info-box-foreground-default: var(--uui-color-text);

				--lk-info-box-background-info: var(--uui-color-default);
				--lk-info-box-foreground-info: var(--uui-color-default-contrast);

				--lk-info-box-background-positive: var(--uui-color-positive);
				--lk-info-box-foreground-positive: var(--uui-color-positive-contrast);

				--lk-info-box-background-warning: var(--uui-color-warning);
				--lk-info-box-foreground-warning: var(--uui-color-warning-contrast);

				--lk-info-box-background-danger: var(--uui-color-danger);
				--lk-info-box-foreground-danger: var(--uui-color-danger-contrast);

				display: block;
			}

			#box {
				--uui-color-interactive: var(--lk-info-box-foreground);
				--uui-color-interactive-emphasis: var(--lk-info-box-foreground);

				display: flex;
				align-items: flex-start;
				justify-content: flex-start;
				gap: 1rem;

				background-color: var(--lk-info-box-background);
				color: var(--lk-info-box-foreground);

				border-radius: calc(var(--uui-border-radius) * 2);

				box-shadow: var(--uui-shadow-depth-1);

				padding: 1rem;

				> div {
					flex: 1;
				}

				> umb-icon {
					color: var(--lk-info-box-foreground);
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
