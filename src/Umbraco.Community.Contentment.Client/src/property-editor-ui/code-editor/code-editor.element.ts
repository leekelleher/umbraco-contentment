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
	styleMap,
	unsafeCSS,
} from '@umbraco-cms/backoffice/external/lit';
import { defaultKeymap, editorCommands } from 'prism-code-editor/commands';
import { highlightBracketPairs } from 'prism-code-editor/highlight-brackets';
import { indentGuides } from 'prism-code-editor/guides';
import { matchBrackets } from 'prism-code-editor/match-brackets';
import { matchTags } from 'prism-code-editor/match-tags';
import { searchWidget } from 'prism-code-editor/search';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import { UMB_THEME_CONTEXT } from '@umbraco-cms/backoffice/themes';
import type { PrismEditor } from 'prism-code-editor';
import type { Ref } from '@umbraco-cms/backoffice/external/lit';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';

import prismLayout from 'prism-code-editor/layout.css?inline';
import prismSearch from 'prism-code-editor/search.css?inline';
import prismGuides from 'prism-code-editor/guides.css?inline';
import vsCodeLight from 'prism-code-editor/themes/vs-code-light.css?inline';
import vsCodeDark from 'prism-code-editor/themes/vs-code-dark.css?inline';

// Static specifiers so Vite can pre-resolve and emit a chunk per grammar.
// Unknown syntaxes no-op and render as plain text (per spec).
// prettier-ignore
const SYNTAXES: Record<string, { id: string; load: () => Promise<unknown> }> = {
	csharp:     { id: 'csharp',     load: () => import('prism-code-editor/prism/languages/csharp') },
	css:        { id: 'css',        load: () => import('prism-code-editor/prism/languages/css') },
	html:       { id: 'markup',     load: () => import('prism-code-editor/prism/languages/markup') },
	javascript: { id: 'javascript', load: () => import('prism-code-editor/prism/languages/javascript') },
	json:       { id: 'json',       load: () => import('prism-code-editor/prism/languages/json') },
	liquid:     { id: 'liquid',     load: () => import('prism-code-editor/prism/languages/liquid') },
	markdown:   { id: 'markdown',   load: () => import('prism-code-editor/prism/languages/markdown') },
	razor:      { id: 'cshtml',     load: () => import('prism-code-editor/prism/languages/cshtml') },
	sql:        { id: 'sql',        load: () => import('prism-code-editor/prism/languages/sql') },
	typescript: { id: 'typescript', load: () => import('prism-code-editor/prism/languages/typescript') },
	xml:        { id: 'xml',        load: () => import('prism-code-editor/prism/languages/xml') },
	yaml:       { id: 'yaml',       load: () => import('prism-code-editor/prism/languages/yaml') },
};

@customElement('contentment-property-editor-ui-code-editor')
export class ContentmentPropertyEditorUICodeEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _language: string = 'razor';

	@state()
	private _fontSize: string = 'small';

	@state()
	private _lineNumbers: boolean = true;

	@state()
	private _wordWrap: boolean = false;

	@state()
	private _loading = true;

	@state()
	private _isDark = false;

	@property()
	public value?: string;

	@property({ type: Boolean, reflect: true })
	public readonly = false;

	public set config(config: UmbPropertyEditorUiElement['config']) {
		if (!config) return;
		this._language = config.getValueByAlias<string>('mode') ?? 'razor';
		this._fontSize = config.getValueByAlias<string>('fontSize') ?? 'small';
		this._lineNumbers = config.getValueByAlias<boolean>('lineNumbers') ?? true;
		this._wordWrap = config.getValueByAlias<boolean>('wordWrap') ?? false;
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

			const syntax = SYNTAXES[this._language];
			await syntax?.load();

			this.#editor = createEditor(this.#containerRef.value, {
				language: syntax?.id ?? 'plain',
				value: this.value ?? '',
				wordWrap: this._wordWrap,
				lineNumbers: this._lineNumbers,
				readOnly: this.readonly,
			});

			this.#editor.addExtensions(
				indentGuides(),
				matchBrackets(),
				highlightBracketPairs(),
				matchTags(),
				searchWidget(),
				editorCommands(defaultKeymap),
			);

			this.#editor.on('update', (value) => {
				this.value = value;
				this.dispatchEvent(new UmbChangeEvent());
			});
		} catch (error) {
			console.error(error);
		}
	}

	override update(changedProperties: Map<string, unknown>) {
		super.update(changedProperties);

		if (changedProperties.has('readonly') && this.#editor) {
			this.#editor.setOptions({ readOnly: this.readonly });
		}
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`
			<div
				id="code-editor"
				data-line-numbers=${this._lineNumbers}
				data-theme=${this._isDark ? 'dark' : 'light'}
				style=${styleMap({ fontSize: this._fontSize })}
				${ref(this.#containerRef)}></div>
		`;
	}

	static override styles = [
		unsafeCSS(prismLayout),
		unsafeCSS(prismSearch),
		unsafeCSS(prismGuides),
		css`
			#code-editor {
				display: block;
				height: auto;
				margin-left: -15px;

				> .prism-code-editor {
					width: 100%;
				}

				&[data-line-numbers='true'] {
					margin-left: -30px;
				}

				&[data-theme='light'] {
					${unsafeCSS(vsCodeLight)};
				}

				&[data-theme='dark'] {
					${unsafeCSS(vsCodeDark)};
				}
			}

			@container (width < 700px) {
				#code-editor[data-line-numbers] {
					margin-left: 0;
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
