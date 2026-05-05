# Prism Code Editor — Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Swap the internals of the Contentment Code Editor property editor UI from Umbraco's bundled Monaco wrapper to [`prism-code-editor`](https://prism-code-editor.netlify.app/), preserving the public alias, manifest schema, and value shape.

**Architecture:** A single Lit element rewrite plus a manifest dropdown extension. Editor core is dynamic-imported on first render; per-language Prism grammars are dynamic-imported once the configured `mode` is known. Backoffice theme is observed via `UMB_THEME_CONTEXT`, mapped to a `data-theme` attribute on the editor host so scoped Prism stylesheets can switch in CSS without re-importing.

**Tech Stack:** TypeScript, Lit (via `@umbraco-cms/backoffice/external/lit`), `UmbLitElement`, `prism-code-editor` 5.1.0, Vite (existing build, no config changes).

**Spec:** [`.claude/plans/2026-05-05-prism-code-editor-design.md`](./2026-05-05-prism-code-editor-design.md)

**Branch:** `dev/wip/prism-code-editor` (already checked out off `dev/v6.x`).

---

## Pre-flight

Before starting, verify:

- Current branch is `dev/wip/prism-code-editor`. Run `git branch --show-current`.
- Working tree is clean. Run `git status`.
- Working directory for all `npm`/build commands is `src/Umbraco.Community.Contentment.Client/`. The repo root is `C:\VCS\Umbraco\Projects\umbraco-contentment\`.

**Conventions for this plan:**

- No automated test suite exists. The spec replaces "write failing test" with manual verification using the design document's checklist. Each task ends in a manual verification step + a commit.
- Commit subjects use the `Client: ...` prefix to match recent history (e.g. `Client: drop Promise constructor anti-pattern in text-input`).
- No `Co-Authored-By` trailer (project memory).
- Run all commands from the repo root unless a step says otherwise.

---

## File map

| File | Action | Why |
|---|---|---|
| `src/Umbraco.Community.Contentment.Client/package.json` | Modify | Add `prism-code-editor` to `dependencies`. |
| `src/Umbraco.Community.Contentment.Client/package-lock.json` | Modify (auto) | Lockfile after `npm install`. |
| `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/manifest.ts` | Modify | Extend dropdown `items` from 7 → 12 languages. |
| `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts` | Rewrite | Replace `umb-code-editor` wrapper with a Prism Code Editor wrapper. |

No new files. No C# changes. Build output (`src/Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/`) regenerates automatically — do not hand-edit.

---

## Task list

### Task 1: Install `prism-code-editor`

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/package.json`
- Modify (auto): `src/Umbraco.Community.Contentment.Client/package-lock.json`

- [ ] **Step 1:** Add the dependency.

From `src/Umbraco.Community.Contentment.Client/`:

```bash
npm install prism-code-editor@^5.1.0
```

- [ ] **Step 2:** Verify `package.json` now contains the entry.

Open `src/Umbraco.Community.Contentment.Client/package.json`. The `dependencies` block should read:

```json
"dependencies": {
  "liquidjs": "^10.25.7",
  "prism-code-editor": "^5.1.0",
  "sortablejs": "^1.15.7"
}
```

- [ ] **Step 3:** Verify the install resolved cleanly.

```bash
npm ls prism-code-editor
```

Expected: a line like `prism-code-editor@5.1.0` with no `UNMET DEPENDENCY` or peer warnings.

- [ ] **Step 4:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/package.json src/Umbraco.Community.Contentment.Client/package-lock.json
git commit -m "Client: add prism-code-editor dependency"
```

---

### Task 2: Extend the Code Editor manifest dropdown

Add C#, Liquid, SQL, XML, YAML to the existing 7-entry list.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/manifest.ts`

- [ ] **Step 1:** Replace the `items` array.

Current `items` (lines 24–32 of `manifest.ts`):

```ts
value: [
  { name: 'CSS', value: 'css' },
  { name: 'HTML', value: 'html' },
  { name: 'JavaScript', value: 'javascript' },
  { name: 'JSON', value: 'json' },
  { name: 'Markdown', value: 'markdown' },
  { name: 'Razor (CSHTML)', value: 'razor' },
  { name: 'TypeScript', value: 'typescript' },
],
```

Replace with the 12-entry list, alphabetised by `name`:

```ts
value: [
  { name: 'C#', value: 'csharp' },
  { name: 'CSS', value: 'css' },
  { name: 'HTML', value: 'html' },
  { name: 'JavaScript', value: 'javascript' },
  { name: 'JSON', value: 'json' },
  { name: 'Liquid', value: 'liquid' },
  { name: 'Markdown', value: 'markdown' },
  { name: 'Razor (CSHTML)', value: 'razor' },
  { name: 'SQL', value: 'sql' },
  { name: 'TypeScript', value: 'typescript' },
  { name: 'XML', value: 'xml' },
  { name: 'YAML', value: 'yaml' },
],
```

The `defaultData` entry below it (`{ alias: 'mode', value: 'razor' }`) stays unchanged.

- [ ] **Step 2:** Verify the file still type-checks.

From `src/Umbraco.Community.Contentment.Client/`:

```bash
npx tsc --noEmit
```

Expected: no errors. (If `tsc` reports issues unrelated to this change, stop and surface them — they were pre-existing.)

- [ ] **Step 3:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/manifest.ts
git commit -m "Client: extend code editor language list with C#, Liquid, SQL, XML, YAML"
```

---

### Task 3: Replace the element with a Prism skeleton

This task swaps the implementation of `code-editor.element.ts` from an `umb-code-editor` wrapper to a Prism Code Editor wrapper. After this task the editor mounts as **plain text** (no grammar, no extensions, no theme yet) — those land in subsequent tasks.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Replace the entire file with the skeleton below.

```ts
// SPDX-License-Identifier: MPL-2.0
// Copyright © 2023 Lee Kelleher

import { css, customElement, html, property, state } from '@umbraco-cms/backoffice/external/lit';
import { createRef, ref, type Ref } from 'lit/directives/ref.js';
import { UmbChangeEvent } from '@umbraco-cms/backoffice/event';
import { UmbLitElement } from '@umbraco-cms/backoffice/lit-element';
import type { UmbPropertyEditorUiElement } from '@umbraco-cms/backoffice/property-editor';
import type { PrismEditor } from 'prism-code-editor';

import 'prism-code-editor/layout.css';

@customElement('contentment-property-editor-ui-code-editor')
export class ContentmentPropertyEditorUICodeEditorElement extends UmbLitElement implements UmbPropertyEditorUiElement {
	@state()
	private _mode: string = 'razor';

	@state()
	private _loading = true;

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
		this.#loadEditor();
	}

	async #loadEditor() {
		try {
			const { createEditor } = await import('prism-code-editor');
			this._loading = false;
			await this.updateComplete;
			if (!this.#containerRef.value) return;
			this.#editor = createEditor(this.#containerRef.value, {
				language: 'plain',
				value: this.value ?? '',
			});
			this.#editor.addListener('update', () => {
				if (!this.#editor) return;
				this.value = this.#editor.value;
				this.dispatchEvent(new UmbChangeEvent());
			});
		} catch (error) {
			console.error(error);
		}
	}

	override render() {
		if (this._loading) return html`<uui-loader></uui-loader>`;
		return html`<div id="code-editor" ${ref(this.#containerRef)}></div>`;
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
```

Notes for the implementer:

- The exact event name on the Prism editor instance (`'update'`) and the listener method (`addListener` vs `addEventListener`) should be confirmed against the `prism-code-editor` 5.1.0 type declarations. If TypeScript flags the call, switch to whichever API the type definitions expose — the contract we need is "fire when the user-edited text changes; we then read `editor.value`". Don't accept silent-fail here; surface the change.
- The `createEditor` import comes from the package root (`'prism-code-editor'`). Vite externalises only `@umbraco/*`, so this gets bundled.
- `language: 'plain'` is a placeholder that resolves to no syntax highlighting — Prism Code Editor falls back to plain text when a grammar isn't loaded. Task 4 replaces this with a real grammar.

- [ ] **Step 2:** Type-check.

From `src/Umbraco.Community.Contentment.Client/`:

```bash
npx tsc --noEmit
```

Expected: no errors.

- [ ] **Step 3:** Build.

```bash
npm run build
```

Expected: build succeeds; output in `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/code-editor.element.js` is regenerated.

- [ ] **Step 4:** Manual verification — editor still loads.

Run a host site (e.g. `dotnet run --project src/Umbraco.Cms.17.x` from the repo root). Open the backoffice, navigate to a Document Type that uses a Contentment Code Editor data type (or create one if needed). Open a document with that property.

Expected: `<uui-loader>` flashes briefly, then a plain-text editable area appears. Typing into it and saving the document should persist the value (round-trip via the property's value pipeline). No console errors.

- [ ] **Step 5:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
git commit -m "Client: rewrite code editor as prism-code-editor skeleton"
```

---

### Task 4: Wire grammar lazy-loading

Replace `language: 'plain'` with the real grammar that matches the configured `_mode`. Add a small mode→Prism-language-id map and an async grammar import.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Add the language map as a module-level constant near the top of the file (just below the imports, above the `@customElement` decorator):

```ts
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
```

- [ ] **Step 2:** Replace the body of `#loadEditor` with this version that resolves and lazy-loads the grammar:

```ts
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
		this.#editor.addListener('update', () => {
			if (!this.#editor) return;
			this.value = this.#editor.value;
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
```

Notes for the implementer:

- `/* @vite-ignore */` keeps Vite from trying to statically pre-resolve the dynamic specifier at build time. Vite *will* emit code-split chunks for the resolved paths at runtime; if you find chunks aren't being emitted with predictable filenames, switch to an explicit `switch` over the known `language` values that uses static `import('prism-code-editor/prism/languages/cshtml')`-style specifiers. The spec calls for one chunk per grammar; either approach satisfies that.
- The Prism language id `markup` covers HTML, XML, and SVG — that's why both `html` and `xml` modes map to the same grammar. `xml` keeps a separate dropdown entry for clarity.

- [ ] **Step 3:** Type-check.

```bash
npx tsc --noEmit
```

Expected: no errors.

- [ ] **Step 4:** Build.

```bash
npm run build
```

Expected: build succeeds. Inspect `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/`. You should see chunked output for the editor core and (eventually, on first use of each language at runtime) per-language grammar chunks. The build itself only emits chunks for statically reachable specifiers, so dynamic-imported grammars are pulled lazily by the host's module loader at runtime — that's expected.

- [ ] **Step 5:** Manual verification — syntax highlighting works.

Restart (or hot-reload) the host site. Open a Code Editor data type with the default `razor` mode. Type a sample like `@if (Model != null) { <p>@Model.Name</p> }`.

Expected: Razor `@`-blocks highlight differently from the surrounding HTML. No console errors. Network panel shows a chunk fetched for the `cshtml` grammar on first edit.

Repeat for one or two other modes (e.g. JSON: `{ "a": 1 }`; JavaScript: `const x = 42;`) by editing the data type's mode setting.

- [ ] **Step 6:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
git commit -m "Client: lazy-load prism grammars per configured mode"
```

---

### Task 5: Wire core editor extensions

Add the spec's default extension set: search (Ctrl+F), indent guides, match brackets + bracket pair highlight, match tags. Line numbers, autoclose pairs, and active-line highlight are part of Prism Code Editor's built-in editor features and are toggled via editor options where applicable.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Add the extension and CSS imports near the top of the file, alongside the existing `import 'prism-code-editor/layout.css';` line:

```ts
import 'prism-code-editor/layout.css';
import 'prism-code-editor/search.css';
import 'prism-code-editor/guides.css';
```

- [ ] **Step 2:** Inside `#loadEditor`, after `createEditor(...)` returns the editor and before `addListener('update', ...)`, dynamic-import and apply the extensions:

```ts
const [
	{ indentGuides },
	{ matchBrackets },
	{ highlightBracketPairs },
	{ matchTags },
	{ searchWidget },
] = await Promise.all([
	import('prism-code-editor/guides'),
	import('prism-code-editor/match-brackets'),
	import('prism-code-editor/highlight-brackets'),
	import('prism-code-editor/match-tags'),
	import('prism-code-editor/search'),
]);

this.#editor.addExtensions(
	indentGuides(),
	matchBrackets(),
	highlightBracketPairs(),
	matchTags(),
	searchWidget(),
);
```

Notes for the implementer:

- Some Prism Code Editor versions expose `addExtension` (singular) instead of `addExtensions` (plural). If TypeScript flags this, switch to repeated `addExtension(...)` calls or the variadic spread that the type definitions accept.
- Line numbers, autoclose, and active-line highlight: confirm against the Prism Code Editor docs whether these are options on `createEditor({ … })` (e.g. `lineNumbers: true`) or are wired up via additional extensions / setups. If they are options, add them to the `createEditor` call. If they are part of `prism-code-editor/setups` (preconfigured factories), prefer using `import { basicEditor } from 'prism-code-editor/setups'` in place of `createEditor` once you've confirmed the setup includes the spec's required behaviours and *only* those.
- Autoclose pair behaviour (typing `(` inserts `)` and positions caret between) is provided by `prism-code-editor/commands` — likely needs to be enabled either via setup defaults or by adding `defaultCommands(editor)` from that module. Verify and wire whichever path is needed; do not skip this — it's in the spec's default-extensions list.

- [ ] **Step 3:** Type-check.

```bash
npx tsc --noEmit
```

Expected: no errors.

- [ ] **Step 4:** Build.

```bash
npm run build
```

Expected: build succeeds.

- [ ] **Step 5:** Manual verification — extensions work.

Open a Code Editor property in the backoffice. Verify each:

- **Line numbers** appear on the left gutter.
- **Match brackets:** type `{ }` and place the cursor on either; the matching brace highlights.
- **Highlight bracket pairs:** different bracket pairs at different nesting levels render in distinct colours.
- **Match tags:** in HTML/markup mode, type `<div></div>`, place caret on the opening tag; closing tag highlights.
- **Indent guides:** vertical lines appear at indent levels.
- **Search:** Ctrl+F opens the search widget; typing into it highlights matches.
- **Autoclose pairs:** typing `(` inserts `)` and positions caret between.
- **Active-line highlight:** the line the caret is on has a subtle background highlight.

If any of the seven don't work, fix the specific extension (consult the Prism Code Editor API) before moving on.

- [ ] **Step 6:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
git commit -m "Client: enable prism editor extensions (search, guides, brackets, tags)"
```

---

### Task 6: Subscribe `UMB_THEME_CONTEXT` and toggle `data-theme`

Observe Umbraco's backoffice theme alias and reflect it as a `data-theme="dark" | "light"` attribute on the editor wrapper. Theme CSS comes in Task 7; this task only wires up the JS-side observation and the attribute.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Add the import for the theme context token, near the existing Umbraco imports:

```ts
import { UMB_THEME_CONTEXT } from '@umbraco-cms/backoffice/themes';
```

- [ ] **Step 2:** Add the `_isDark` state declaration alongside the other `@state` fields:

```ts
@state()
private _isDark = false;
```

- [ ] **Step 3:** Subscribe in the constructor. Find the existing constructor:

```ts
constructor() {
	super();
	this.#loadEditor();
}
```

Replace with:

```ts
constructor() {
	super();

	this.consumeContext(UMB_THEME_CONTEXT, (context) => {
		this.observe(context?.theme, (themeAlias) => {
			this._isDark = themeAlias === 'umb-dark-theme';
		});
	});

	this.#loadEditor();
}
```

- [ ] **Step 4:** Reflect `_isDark` on the wrapper. Update `render`:

```ts
override render() {
	if (this._loading) return html`<uui-loader></uui-loader>`;
	return html`<div id="code-editor" data-theme=${this._isDark ? 'dark' : 'light'} ${ref(this.#containerRef)}></div>`;
}
```

- [ ] **Step 5:** Type-check.

```bash
npx tsc --noEmit
```

Expected: no errors.

- [ ] **Step 6:** Build.

```bash
npm run build
```

Expected: build succeeds.

- [ ] **Step 7:** Manual verification — attribute toggles.

Open a Code Editor property. In DevTools, inspect the rendered `<contentment-property-editor-ui-code-editor>` shadow root → `<div id="code-editor">` element. Confirm `data-theme="light"`.

Then switch the backoffice theme to dark via the current-user theme picker (top-right user menu → preferences → theme). Without reloading, the same `<div>` should now have `data-theme="dark"`. Switch back to light → `data-theme="light"`. Switch to high contrast → still `data-theme="light"` (per spec, only `umb-dark-theme` resolves to dark).

(Visually, the editor still renders only one stylistic state at this point — Task 7 wires the actual theme CSS.)

- [ ] **Step 8:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
git commit -m "Client: observe UMB_THEME_CONTEXT, expose data-theme on code editor host"
```

---

### Task 7: Import scoped Prism theme CSS

Import both the light and dark Prism stylesheets and scope each behind a `[data-theme="…"]` attribute selector so they coexist without conflicting.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Identify which stock themes ship with `prism-code-editor`. Run from the repo root:

```bash
ls src/Umbraco.Community.Contentment.Client/node_modules/prism-code-editor/dist/themes/
```

Expected: a list of `.css` files. The package's `exports` map allows imports of the form `prism-code-editor/themes/<name>.css` for any of them. Pick one light theme and one dark theme — the spec calls out `github-light` + `github-dark` as the default; if those filenames are not present, substitute the closest equivalents available in the package (e.g. `github-light.css` / `github-dark.css`, or `vs-code-light.css` / `vs-code-dark.css`).

- [ ] **Step 2:** Add the side-effect imports to the top of the file, alongside the existing `'prism-code-editor/layout.css'` import. Do **not** import them at the global module level if they conflict — instead, scope them via `:host` styles. The simplest approach Vite supports is plain side-effect CSS imports (which apply globally) plus selector scoping in our static styles; that's what we'll do, paired with `:host` scoping in our element styles to keep them inside the shadow DOM.

For Lit elements, side-effect CSS imports applied to the document head do **not** reach inside shadow DOM. The clean approach is to inline the theme CSS variables into the element's static styles using Lit's `unsafeCSS` helper. Use this pattern:

```ts
import { css, customElement, html, property, state, unsafeCSS } from '@umbraco-cms/backoffice/external/lit';
import githubLight from 'prism-code-editor/themes/github-light.css?inline';
import githubDark from 'prism-code-editor/themes/github-dark.css?inline';
```

(`?inline` is a Vite query that returns the CSS as a string instead of injecting it.)

- [ ] **Step 3:** Update `static styles` to apply each theme's rules under the matching `[data-theme="…"]` selector:

```ts
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

		#code-editor[data-theme='light'] .prism-code-editor {
			${unsafeCSS(githubLight)}
		}

		#code-editor[data-theme='dark'] .prism-code-editor {
			${unsafeCSS(githubDark)}
		}
	`,
];
```

Notes for the implementer:

- The theme stylesheets target `.prism-code-editor` directly. Nesting their rules under our scoped selector via Lit's `css` template literal nesting (or by inlining as raw CSS strings) keeps both stylesheets present without conflict.
- If the theme CSS contains `:root`-level custom property declarations that don't survive the nesting transform (e.g. `:root { --x: y }`), prefer hosting the variables on `.prism-code-editor[data-theme='light'|'dark']` directly. Test in the browser; if the wrapped form fails to apply colours, fall back to splitting into two classes added via JS (`classList.toggle('theme-dark', this._isDark)`) and importing each stylesheet only when its theme is active.
- This step has the highest likelihood of needing a small adjustment from the planned shape — the goal is "both themes coexist; scope picks the active one". If the inlined-CSS approach hits a CSS parsing edge case in Lit, switch to dynamic-import of the matching CSS via `@vite-ignore` plus a `<link>` element appended to shadow DOM. Either approach satisfies the spec.

- [ ] **Step 4:** Type-check.

```bash
npx tsc --noEmit
```

Expected: no errors. (You may need a `*.css?inline` ambient declaration if TypeScript doesn't already know about Vite's `?inline` query parameter. If it complains, add the following to a new file or an existing `.d.ts` such as `src/Umbraco.Community.Contentment.Client/src/types.d.ts`:

```ts
declare module '*.css?inline' {
	const content: string;
	export default content;
}
```

If you create a new `.d.ts`, include it in the commit.)

- [ ] **Step 5:** Build.

```bash
npm run build
```

Expected: build succeeds. The two theme CSS files are embedded as strings into the element bundle — there's no separate `.css` chunk for them.

- [ ] **Step 6:** Manual verification — theme switches visually.

Open the Code Editor property. Type a multi-language sample (e.g. some HTML with attributes). Verify colours match a "GitHub Light"-like palette. Switch the backoffice to dark via the user menu — without a page reload, the editor's colours should switch to a "GitHub Dark"-like palette. Switch back. Switch to high contrast — expect the light palette to remain (per spec).

- [ ] **Step 7:** Commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
# Include the .d.ts file if you added one in Step 4:
git add src/Umbraco.Community.Contentment.Client/src/types.d.ts 2>/dev/null || true
git commit -m "Client: scope prism theme CSS by data-theme attribute"
```

---

### Task 8: Final cleanup

Drop now-unused imports and styles. The element previously subscribed to `UMB_PROPERTY_CONTEXT` to track `appearance.labelOnTop` and applied a `-30px` margin hack. Per spec, both go away.

**Files:**
- Modify: `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`

- [ ] **Step 1:** Verify all of the following are absent from the file (they should already be, given the rewrite in Task 3):

- The `classMap` import from `@umbraco-cms/backoffice/external/lit`.
- The `UmbInputEvent` import.
- The `UmbCodeEditorElement` type import (`@umbraco-cms/backoffice/code-editor`).
- The `UMB_PROPERTY_CONTEXT` import / `consumeContext(UMB_PROPERTY_CONTEXT, …)` block.
- The `_hideMargin` `@state` field.
- Any `&.margin { margin-left: -30px }` CSS rule.

If any are still present, remove them.

- [ ] **Step 2:** Sweep imports — every import in the file should be referenced in code. Run:

```bash
npx tsc --noEmit
```

Expected: no errors. If TypeScript reports unused imports (with `noUnusedLocals` enabled), remove them.

- [ ] **Step 3:** Build.

```bash
npm run build
```

Expected: build succeeds.

- [ ] **Step 4:** Manual verification — alignment.

Open a Code Editor property in two configurations:

- A data type with the default appearance (`labelOnTop: false`). The editor should sit flush in the property column with no leftward overhang.
- A data type with `labelOnTop: true`. Same — flush.

Compare horizontal alignment to a sibling property (e.g. a text input on the same form). Edges should line up.

- [ ] **Step 5:** If any cleanup edits were made, commit.

```bash
git add src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts
git commit -m "Client: drop margin hack and property-context observer from code editor"
```

If no edits were needed (everything was already clean from Task 3), skip the commit and note this in your report.

---

### Task 9: Walk the spec's manual verification checklist

This is the formal sign-off task — every numbered item in the spec's "Verification" section must pass.

**Files:** None modified.

- [ ] **Step 1:** Start a host site if one isn't already running.

```bash
dotnet run --project src/Umbraco.Cms.17.x
```

Wait for `Now listening on: http://localhost:21187` (or whatever port it picks). Log in to the backoffice.

- [ ] **Step 2:** Walk each verification step from the design document. Do not paraphrase from memory — open the spec file and tick each off.

Reference: [`.claude/plans/2026-05-05-prism-code-editor-design.md`](./2026-05-05-prism-code-editor-design.md), section "Verification (manual — no test suite)".

For each numbered step (1–9), record pass/fail and any observation. The nine steps cover:

1. Editor loads, no JS errors, single `<uui-loader>` flash.
2. Cycle through all 12 dropdown entries.
3. Default new data type → Razor → `cshtml` grammar applied.
4. Backoffice theme toggle (light → dark → high-contrast).
5. Search, brackets, autoclose, line numbers, active-line, indent guides.
6. Manually configured `mode: ruby` → plain text fallback.
7. SQL data source / Templated List config still work (they embed the same alias).
8. Layout: `labelOnTop: true` and `false` both look right.
9. Bundle size: gzip-compare `wwwroot/App_Plugins/Contentment/` before and after the swap.

For step 9 specifically:

```bash
# Before (run on dev/v6.x as a baseline; you may need to rebuild that branch separately to capture the "before" size)
# Compare total gzipped size of the wwwroot bundle.
```

Capture the before/after numbers in your final report. The expectation is "smaller"; no specific budget.

- [ ] **Step 3:** Compile a brief verification report.

Write a short (<200 word) summary covering: which steps passed, which steps required adjustment (if any), the bundle size delta, and any notes on rough edges that should be addressed before merging upstream. This report is not committed — it's the message you return with on completion.

- [ ] **Step 4 (optional):** If verification surfaced any issues, fix them in additional commits on the same branch using the `Client: …` prefix. Re-run any affected steps from the checklist.

---

## Out of scope (do not do)

These items appear in the codebase but are explicitly out of scope per the spec:

- `src/Umbraco.Community.Contentment.Client/src/property-action/edit-json/edit-json.controller.ts` — keeps using `UMB_CODE_EDITOR_MODAL` from `@umbraco-cms/backoffice/code-editor` (Umbraco's bundled Monaco modal). Do not change it.
- C# code (`src/Umbraco.Community.Contentment/DataEditors/CodeEditor/CodeEditorDataEditor.cs`, ValueConverters, migrations). Do not change them.
- `src/Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/*` — generated build output, regenerated by `npm run build`. Don't hand-edit.
- The `.csproj` exclusion lists, `src/packages/`, and any other vendored or generated paths.
- New tests, test infrastructure, or CI workflows. The repo has no automated test suite and the spec does not introduce one.

---

## Done when

- All eight implementation tasks are committed on `dev/wip/prism-code-editor`.
- Task 9's verification report shows every numbered spec step passing (or each failure noted with a follow-up commit and re-verification).
- `git status` is clean.
- The branch has not been pushed (the spec doesn't ask for it; await user instruction).
