// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { loadCodeEditor } from '@umbraco-cms/backoffice/code-editor';
import { UmbInputEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbPropertyValueChangeEvent } from '@umbraco-cms/backoffice/property-editor';
import type { UmbCodeEditorElement } from '@umbraco-cms/backoffice/code-editor';
import type { UmbPropertyEditorConfigCollection } from '@umbraco-cms/backoffice/property-editor';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/extension-registry';

type CodeEditorLanguage = UmbCodeEditorElement['language'];

const ELEMENT_NAME = 'contentment-property-editor-ui-code-editor';

@customElement(ELEMENT_NAME)
export class ContentmentPropertyEditorUICodeEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _loading = true;

	constructor() {
		super();
		this.#loadCodeEditor();
	}

	async #loadCodeEditor() {
		try {
			await loadCodeEditor();
			this._loading = false;
		} catch (error) {
			console.error(error);
		}
	}

	#language?: CodeEditorLanguage;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;
		this.#language = config.getValueByAlias<CodeEditorLanguage>('mode');
	}

	#onChange(event: UmbInputEvent & { target: UmbCodeEditorElement }) {
		if (!(event instanceof UmbInputEvent)) return;
		this.value = event.target.code;
		this.dispatchEvent(new UmbPropertyValueChangeEvent());
	}

	render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`
			<div id="code-editor">
				<umb-code-editor language=${this.#language ?? 'razor'} .code=${this.value ?? ''} @input=${this.#onChange}>
				</umb-code-editor>
			</div>
		`;
	}

	static styles = [
		css`
			#code-editor {
				display: flex;
				height: 200px;
				margin-left: -30px;
			}
			#code-editor > umb-code-editor {
				width: 100%;
			}
		`,
	];
}

export { ContentmentPropertyEditorUICodeEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		[ELEMENT_NAME]: ContentmentPropertyEditorUICodeEditorElement;
	}
}
