// SPDX-License-Identifier: MPL-2.0
// Copyright © 2024 Lee Kelleher

import { css, customElement, html, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';

import '../../components/info-box/info-box.element.js';

@customElement('contentment-property-editor-ui-read-only')
export class ContentmentPropertyEditorUIReadOnlyElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#config?: string;
	#value?: string;

	@property({ attribute: false })
	public value?: unknown;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#config = JSON.stringify(config, null, 2);
		this.#value = JSON.stringify(this.value, null, 2);
	}

	override render() {
		return html`
			<contentment-info-box type="warning" icon="icon-alert" heading="This property editor is in read-only mode.">
				${when(
					this.#value,
					() =>
						html`
							<details>
								<summary>Value</summary>
								<umb-code-block copy>${this.#value}</umb-code-block>
							</details>
						`
				)}
				${when(
					this.#config,
					() =>
						html`
							<details>
								<summary>Configuration</summary>
								<umb-code-block copy>${this.#config}</umb-code-block>
							</details>
						`
				)}
			</contentment-info-box>
		`;
	}

	static override styles = [
		css`
			details > summary {
				cursor: pointer;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIReadOnlyElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-read-only': ContentmentPropertyEditorUIReadOnlyElement;
	}
}
