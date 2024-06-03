// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { customElement, html, nothing, property } from '@umbraco-cms/backoffice/external/lit';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

import '../../property-editor-ui/editor-notes/editor-notes.element.js';

const ELEMENT_NAME = 'contentment-property-editor-ui-render-macro';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUIRenderMacroElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		const json = JSON.stringify(config, null, 2);
		const markup = `
<p><em>Support for Macros were deprecated in Umbraco 14. Please consider alternative functionality.</em></p>
${json ? `<details><summary>Macro configuration</summary><umb-code-block copy>${json}</umb-code-block></details>` : ''}
    `;

		this.#notesConfig = new UmbPropertyEditorConfigCollection([
			{ alias: 'alertType', value: 'warning' },
			{ alias: 'icon', value: 'icon-alert' },
			{ alias: 'heading', value: 'Render Macro has been deprecated' },
			{ alias: 'message', value: { markup } },
		]);
	}

	#notesConfig?: UmbPropertyEditorConfigCollection;

	render() {
		if (!this.#notesConfig) return nothing;
		return html`
			<contentment-property-editor-ui-editor-notes .config=${this.#notesConfig}>
			</contentment-property-editor-ui-editor-notes>
		`;
	}
}

export { ContentmentPropertyEditorUIRenderMacroElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUIRenderMacroElement;
	}
}
