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
	unsafeCSS,
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

import prismLayout from 'prism-code-editor/layout.css?inline';
import prismSearch from 'prism-code-editor/search.css?inline';
import prismGuides from 'prism-code-editor/guides.css?inline';
import vsCodeLight from 'prism-code-editor/themes/vs-code-light.css?inline';
import vsCodeDark from 'prism-code-editor/themes/vs-code-dark.css?inline';

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
		// Static specifiers so Vite can pre-resolve and emit a chunk per grammar.
		// Unknown languages fall through and render as plain text (per spec).
		switch (language) {
			case 'csharp': await import('prism-code-editor/prism/languages/csharp'); break;
			case 'cshtml': await import('prism-code-editor/prism/languages/cshtml'); break;
			case 'css': await import('prism-code-editor/prism/languages/css'); break;
			case 'javascript': await import('prism-code-editor/prism/languages/javascript'); break;
			case 'json': await import('prism-code-editor/prism/languages/json'); break;
			case 'liquid': await import('prism-code-editor/prism/languages/liquid'); break;
			case 'markdown': await import('prism-code-editor/prism/languages/markdown'); break;
			case 'markup': await import('prism-code-editor/prism/languages/markup'); break;
			case 'sql': await import('prism-code-editor/prism/languages/sql'); break;
			case 'typescript': await import('prism-code-editor/prism/languages/typescript'); break;
			case 'xml': await import('prism-code-editor/prism/languages/xml'); break;
			case 'yaml': await import('prism-code-editor/prism/languages/yaml'); break;
		}
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`<div id="code-editor" data-theme=${this._isDark ? 'dark' : 'light'} ${ref(this.#containerRef)}></div>`;
	}

	static override styles = [
		css`
			${unsafeCSS(prismLayout)}
		`,
		css`
			${unsafeCSS(prismSearch)}
		`,
		css`
			${unsafeCSS(prismGuides)}
		`,
		css`
			#code-editor {
				display: block;
				height: auto;
			}

			#code-editor > .prism-code-editor {
				height: 100%;
				width: 100%;
			}

			#code-editor[data-theme='light'] {
				${unsafeCSS(vsCodeLight)}
			}

			#code-editor[data-theme='dark'] {
				${unsafeCSS(vsCodeDark)}
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
