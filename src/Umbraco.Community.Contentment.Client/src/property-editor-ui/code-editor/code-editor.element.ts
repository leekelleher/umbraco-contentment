// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import {
	createRef,
	css,
	customElement,
	html,
	property,
	ref,
	state,
	type Ref,
} from '@umbraco-cms/backoffice/external/lit';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import { UMB_THEME_CONTEXT } from '@umbraco-cms/backoffice/themes';
import type { PrismEditor } from 'prism-code-editor';
import { defaultCommands } from 'prism-code-editor/commands';
import { indentGuides } from 'prism-code-editor/guides';
import { highlightBracketPairs } from 'prism-code-editor/highlight-brackets';
import { matchBrackets } from 'prism-code-editor/match-brackets';
import { matchTags } from 'prism-code-editor/match-tags';
import { searchWidget } from 'prism-code-editor/search';

import 'prism-code-editor/layout.css';
import 'prism-code-editor/search.css';
import 'prism-code-editor/guides.css';

const MODE_TO_PRISM: Record<string, string> = {
	csharp: 'csharp',
	css: 'css',
	html: 'markup',
	javascript: 'javascript',
	json: 'json',
	liquid: 'liquid',
	markdown: 'markdown',
	razor: 'cshtml',
	sql: 'sql',
	typescript: 'typescript',
	xml: 'xml',
	yaml: 'yaml',
};

@customElement('contentment-property-editor-ui-code-editor')
export class ContentmentPropertyEditorUICodeEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _mode: string = 'razor';

	@state()
	private _loading = true;

	@state()
	private _isDark = false;

	@property()
	public value?: string;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this._mode = config.getValueByAlias<string>('mode') ?? 'razor';
	}

	#containerRef: Ref<HTMLDivElement> = createRef();
	#editor?: PrismEditor;

	constructor() {
		super();

		this.consumeContext(UMB_THEME_CONTEXT, (context) => {
			this.observe(context?.theme, (themeAlias) => {
				this._isDark = themeAlias === 'umb-dark-theme';
			});
		});

		this.#loadEditor();
	}

	async #loadEditor() {
		try {
			const { createEditor } = await import('prism-code-editor');
			this._loading = false;
			await this.updateComplete;
			if (!this.#containerRef.value) return;

			const language = MODE_TO_PRISM[this._mode] ?? 'plain';
			await this.#loadGrammar(language);

			this.#editor = createEditor(this.#containerRef.value, {
				language,
				value: this.value ?? '',
			});

			this.#editor.addExtensions(
				indentGuides(),
				matchBrackets(),
				highlightBracketPairs(),
				matchTags(),
				searchWidget(),
				defaultCommands(),
			);

			this.#editor.on('update', (value) => {
				this.value = value;
				this.dispatchEvent(new UmbChangeEvent());
			});
		} catch (error) {
			console.error(error);
		}
	}

	async #loadGrammar(language: string) {
		if (language === 'plain') return;
		try {
			await import(/* @vite-ignore */ `prism-code-editor/prism/languages/${language}`);
		} catch (error) {
			// Unknown grammar → fall back silently to plain text.
			// (Spec: unknown `mode` values render as plain text, no error toast.)
		}
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`<div id="code-editor" data-theme=${this._isDark ? 'dark' : 'light'} ${ref(this.#containerRef)}></div>`;
	}

	static override styles = [
		css`
			#code-editor {
				display: block;
				height: 200px;
			}

			#code-editor > .prism-code-editor {
				height: 100%;
				width: 100%;
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
