// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, html, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

import '../../components/property-editor-ui/property-editor-ui.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-render-macro';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIRenderMacroElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property()
	public value?: string;

	@property({ attribute: false })
	config?: UmbPropertyEditorConfigCollection;

	#notesConfig?: UmbPropertyEditorConfigCollection;

	connectedCallback() {
		super.connectedCallback();

		const json = JSON.stringify(this.config, null, 2);
		const message = `
<p><em>Support for Macros were deprecated in Umbraco 14. Please consider alternative functionality.</em></p>
${json ? `<details><summary>JSON configuration</summary><pre><code>${json}</code></pre></details>` : ''}
    `;

		this.#notesConfig = new UmbPropertyEditorConfigCollection([
			{ alias: 'alertType', value: 'warning' },
			{ alias: 'icon', value: 'icon-alert' },
			{ alias: 'heading', value: 'Render Macro has been deprecated' },
			{ alias: 'message', value: message },
		]);
	}

	render() {
		return html`
			<contentment-property-editor-ui
				.config=${this.#notesConfig}
				property-editor-ui-alias="Umb.Contentment.PropertyEditorUi.EditorNotes">
			</contentment-property-editor-ui>
		`;
	}
}

export { ContentmentPropertyEditorUIRenderMacroElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIRenderMacroElement;
	}
}
