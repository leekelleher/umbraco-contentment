// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { classMap, css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { UmbInputEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import type { UmbCodeEditorElement } from '@umbraco-cms/backoffice/code-editor';
import type {
	UmbPropertyEditorConfigCollection,
	UmbPropertyEditorUiElement,
} from '@umbraco-cms/backoffice/property-editor';
import { UMB_PROPERTY_CONTEXT } from '@umbraco-cms/backoffice/property';

type CodeEditorLanguage = UmbCodeEditorElement['language'];

@customElement('contentment-property-editor-ui-code-editor')
export class ContentmentPropertyEditorUICodeEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	#language?: CodeEditorLanguage;

	@state()
	private _loading = true;

	@state()
	private _hideMargin = false;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorConfigCollection | undefined) {
		if (!config) return;

		this.#language = config.getValueByAlias<CodeEditorLanguage>('mode');
	}

	constructor() {
		super();

		this.#loadCodeEditor();

		this.consumeContext(UMB_PROPERTY_CONTEXT, (context) => {
			this.observe(context?.appearance, (appearance) => {
				this._hideMargin = appearance?.labelOnTop ?? false;
			});
		});
	}

	async #loadCodeEditor() {
		try {
			await import('@umbraco-cms/backoffice/code-editor');
			this._loading = false;
		} catch (error) {
			console.error(error);
		}
	}

	#onChange(event: UmbInputEvent & { target: UmbCodeEditorElement }) {
		if (!(event instanceof UmbInputEvent)) return;
		this.value = event.target.code;
		this.dispatchEvent(new UmbChangeEvent());
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`
			<div id="code-editor" class=${classMap({ margin: !this._hideMargin })}>
				<umb-code-editor language=${this.#language ?? 'razor'} .code=${this.value ?? ''} @input=${this.#onChange}>
				</umb-code-editor>
			</div>
		`;
	}

	static override styles = [
		css`
			#code-editor {
				display: flex;
				height: 200px;

				&.margin {
					margin-left: -30px;
				}

				> umb-code-editor {
					width: 100%;
				}
			}
		`,
	];
}

export { ContentmentPropertyEditorUICodeEditorElement as element };

declare global {
	interface HTMLElementTagNameMap {
		'contentment-property-editor-ui-code-editor': ContentmentPropertyEditorUICodeEditorElement;
	}
}
