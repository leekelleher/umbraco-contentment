// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, styleMap, when, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { UUIInterfaceColor } from '@umbraco-cms/backoffice/external/uui';

@customElement('contentment-info-box')
export default class ContentmentInfoBoxElement extends UmbLitElement {
	@property()
	heading?: string;

	@property()
	icon?: string;

	@property()
	message?: string;

	@property()
	type?: UUIInterfaceColor | 'border' | 'current' | 'disabled' | 'divider' | 'selected' = 'default';

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
					(icon) => html`<umb-icon name=${icon} style="color: var(--uui-color-${this.type}-contrast);"></umb-icon>`
				)}
				<div>
					${when(this.heading, (heading) => html`<h5>${heading}</h5>`)}
					${when(this.message, (message) => html`<div>${unsafeHTML(message)}</div>`)}
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

declare global {
	interface HTMLElementTagNameMap {
		'contentment-info-box': ContentmentInfoBoxElement;
	}
}
