// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, styleMap, when, unsafeHTML } from '@umbraco-cms/backoffice/external/lit';
import { parseBoolean, tryHideLabel } from '../../utils/index.js';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbTextStyles } from '@umbraco-cms/backoffice/style';
import type { StyleInfo } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

const ELEMENT_NAME = 'contentment-property-editor-ui-editor-notes';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIEditorNotesElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#hideLabel: boolean = false;

	#alertType?: string;

	#icon?: string;

	#inlineStyles: StyleInfo = {};

	#heading?: string;

	#message?: string;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#alertType = config.getValueByAlias('alertType') || 'default';
		this.#icon = config.getValueByAlias('icon');
		this.#heading = config.getValueByAlias('heading');
		const message = config.getValueByAlias('message');
		this.#message = (message as unknown as any)?.markup ?? message;
		this.#hideLabel = parseBoolean(config.getValueByAlias('hideLabel'));

		if (this.#icon) {
			// HACK: To workaround the `color-text` part of the value. [LK]
			this.#icon = this.#icon.split(' ')[0];
		}

		this.#inlineStyles = {
			backgroundColor: `var(--uui-color-${this.#alertType})`,
			color: `var(--uui-color-${this.#alertType}-contrast)`,
			borderColor: `var(--uui-color-${this.#alertType}-standalone)`,
		};
	}

	connectedCallback() {
		super.connectedCallback();
		tryHideLabel(this, this.#hideLabel);
	}

	render() {
		return html`
			<div id="note" class="uui-text ${this.#alertType}" style=${styleMap(this.#inlineStyles)}>
				${when(
					this.#icon,
					() =>
						html`<umb-icon name=${this.#icon!} style="color: var(--uui-color-${this.#alertType}-contrast);"></umb-icon>`
				)}
				<div>
					${when(this.#heading, () => html`<h5>${this.#heading}</h5>`)}
					${when(this.#message, () => html`<div>${unsafeHTML(this.#message)}</div>`)}
				</div>
			</div>
		`;
	}

	static styles = [
		UmbTextStyles,
		css`
			#note {
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

			#note > div {
				flex: 1;
			}

			umb-icon {
				font-size: 3rem;
			}

			.uui-text p {
				margin: 0.5rem 0;
			}
			.uui-text p:last-child {
				margin-bottom: 0.25rem;
			}

			details > summary {
				cursor: pointer;
				font-weight: bold;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIEditorNotesElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIEditorNotesElement;
	}
}
