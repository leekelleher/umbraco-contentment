// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, when } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

@customElement('contentment-property-editor-ui-render-macro')
export class ContentmentPropertyEditorUIRenderMacroElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#json = '';

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this.#json = JSON.stringify(config, null, 2);
	}

	override render() {
		return html`
			<contentment-info-box type="warning" icon="icon-alert" headline="Render Macro has been deprecated">
				<p><em>Support for Macros were deprecated in Umbraco 14. Please consider alternative functionality.</em></p>
				${when(
					this.#json,
					() =>
						html`
							<details>
								<summary>Macro configuration</summary>
								<umb-code-block copy>${this.#json}</umb-code-block>
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
				font-weight: bold;
			}
		`,
	];
}

export { ContentmentPropertyEditorUIRenderMacroElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-render-macro': ContentmentPropertyEditorUIRenderMacroElement;
	}
}
