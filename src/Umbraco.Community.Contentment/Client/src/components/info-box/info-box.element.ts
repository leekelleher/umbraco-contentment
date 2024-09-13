// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, styleMap, when, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import { UUIInterfaceColor } from '@umbraco-cms/backoffice/external/uui';

const ELEMENT_NAME = 'contentment-info-box';

@customElement(ELEMENT_NAME)
export class ContentmentInfoBoxElement extends UmbLitElement {
	@property()
	heading?: string;

	@property()
	icon?: string;

	@property()
	message?: string;

	@property()
	type?: UUIInterfaceColor | 'current';

	#getStyles() {
		return styleMap({
			backgroundColor: `var(--uui-color-${this.type})`,
			color: `var(--uui-color-${this.type}-contrast)`,
			borderColor: `var(--uui-color-${this.type}-standalone)`,
		});
	}

	override render() {
		return html`
			<div id="box" class="uui-text ${this.type}" style=${this.#getStyles()}>
				${when(
					this.icon,
					() => html`<umb-icon .name=${this.icon} style="color: var(--uui-color-${this.type}-contrast);"></umb-icon>`
				)}
				<div>
					${when(this.heading, () => html`<h5>${this.heading}</h5>`)}
					${when(this.message, () => html`<div>${unsafeHTML(this.message)}</div>`)}
					<slot></slot>
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
			}

			#box > div {
				flex: 1;
			}

			umb-icon {
				font-size: 3rem;
			}

			.uui-text p,
			.uui-text ::slotted(p) {
				margin: 0.5rem 0;
			}

			.uui-text p:last-child,
			.uui-text ::slotted(p:last-child) {
				margin-bottom: 0.25rem;
			}

			details > summary {
				cursor: pointer;
				font-weight: bold;
			}
		`,
	];
}

export { ContentmentInfoBoxElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentInfoBoxElement;
	}
}
