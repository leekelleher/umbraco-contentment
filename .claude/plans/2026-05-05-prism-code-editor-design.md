# Prism Code Editor — Design

**Status:** Proposed
**Date:** 2026-05-05
**Branch:** `dev/wip/prism-code-editor` (off `dev/v6.x`)
**Scope:** Client-only (TypeScript/Lit). No C# changes.

## Goal

Replace the internals of the Contentment Code Editor property editor UI
(`Umb.Contentment.PropertyEditorUi.CodeEditor`) — currently a thin wrapper
around Umbraco's bundled `umb-code-editor` (Monaco) — with
[`prism-code-editor`](https://prism-code-editor.netlify.app/). The public alias,
manifest schema, and value shape stay unchanged so existing data types upgrade
transparently.

## Motivation

The five concrete pain points driving the swap:

- **Bundle size** — Monaco is heavy; Prism Code Editor is significantly lighter.
- **Theming** — Monaco's theme doesn't track Umbraco's light/dark backoffice
  theme cleanly; we want the editor to follow `prefers-color-scheme`.
- **Language coverage / customisation** — Prism's grammar surface is more
  flexible and we can curate what we ship.
- **Editor behaviour / API** — search, autoclose, indent guides, read-only
  capability, etc. are first-class on Prism Code Editor and easy to wire up.
- **Embedding / sizing** — the current `-30px` margin hack is a symptom of
  `umb-code-editor` not laying out cleanly in property columns.

## In scope

- `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/code-editor.element.ts`
- `src/Umbraco.Community.Contentment.Client/src/property-editor-ui/code-editor/manifest.ts`
- `src/Umbraco.Community.Contentment.Client/package.json` (new dependency)

## Out of scope

- The `edit-json` property action (`src/property-action/edit-json/edit-json.controller.ts`)
  continues to use Umbraco's `UMB_CODE_EDITOR_MODAL`. Replacing that modal would
  mean writing a custom Contentment modal that mirrors the same UX — bigger
  undertaking, separate piece of work.
- Any C# code (`CodeEditorDataEditor.cs`, ValueConverters, migrations).
- The build output under `wwwroot/App_Plugins/Contentment/` — Vite regenerates
  that automatically.
- Data migrations — value is still a string.

## Data contract

- **Property value:** `string`. Unchanged.
- **Manifest config:** `mode: string`. Unchanged alias. Item list grows from
  7 → 12 entries; the existing `razor` value remains the default so existing
  data types pick up the same default behaviour.

## Language coverage

| Label          | Manifest `mode` | Prism language id |
| -------------- | --------------- | ----------------- |
| C#             | `csharp`        | `csharp`          |
| CSS            | `css`           | `css`             |
| HTML           | `html`          | `markup`          |
| JavaScript     | `javascript`    | `javascript`      |
| JSON           | `json`          | `json`            |
| Liquid         | `liquid`        | `liquid`          |
| Markdown       | `markdown`      | `markdown`        |
| Razor (CSHTML) | `razor`         | `cshtml`          |
| SQL            | `sql`           | `sql`             |
| TypeScript     | `typescript`    | `typescript`      |
| XML            | `xml`           | `xml`             |
| YAML           | `yaml`          | `yaml`            |

Original 7 entries (CSS, HTML, JavaScript, JSON, Markdown, Razor, TypeScript)
are preserved. Five additions: C#, Liquid (used heavily by Contentment's
`blockEditorCustomView` Liquid rendering), SQL, XML, YAML.

A small lookup map inside the element file translates manifest `mode` values
to Prism language ids (most are 1:1; HTML→`markup`, Razor→`cshtml`).

## Loading strategy

- **Editor core** — dynamic-imported on first render. Matches today's behaviour
  (the `<uui-loader>` placeholder stays). Vite emits a chunk for the core under
  `wwwroot/App_Plugins/Contentment/`.
- **Grammar** — dynamic-imported once `_language` is resolved. Each grammar
  becomes its own Vite chunk (e.g. `cshtml.js`). Only grammars users actually
  configure ship over the wire on first use.
- **Extensions** (line numbers, match brackets, autoclose pairs, active-line
  highlight, indent guides, search) — bundled with the core import. Small,
  used universally, not worth a separate chunk.
- **Theme CSS** — both stock themes (`github-light` + `github-dark`) imported
  as side-effect CSS on the editor host, scoped behind a `data-theme="dark"`
  attribute on the editor wrapper so they coexist without conflict. The
  element subscribes to `UMB_THEME_CONTEXT` (from `@umbraco-cms/backoffice/themes`)
  and toggles `data-theme` based on the observed theme alias. Default light
  if the context isn't available.
- **Unknown `mode`** — if grammar dynamic import rejects (e.g. data type
  configured manually with `mode: ruby`), the editor still mounts as plain
  text. No syntax colouring, no error toast, no warning UI.

## Element architecture

```
ContentmentPropertyEditorUICodeEditorElement (UmbLitElement)
├─ @state _language?: string                     // resolved from manifest config
├─ @state _loading = true                        // true until core import resolves
├─ @state _isDark = false                        // observed from UMB_THEME_CONTEXT
├─ @property value?: string                      // bound to property context
├─ constructor()                                 // consumeContext(UMB_THEME_CONTEXT, …)
├─ #loadEditor()                                 // dynamic-import core, grammar, extensions; mount editor
├─ #onChange(value)                              // updates this.value, dispatches UmbChangeEvent
├─ render()                                      // <uui-loader> while loading; host <div data-theme=…> after
└─ static styles                                 // host sizing + scoped theme CSS rules
```

The Prism editor mounts into a host `<div>` — referenced via Lit's `ref()`
directive — once the core module resolves. The element no longer:

- subscribes to `UMB_PROPERTY_CONTEXT` for the `appearance` observable,
- carries the `_hideMargin` state,
- ships the `&.margin { margin-left: -30px }` CSS branch.

The element gains a new `consumeContext(UMB_THEME_CONTEXT, …)` subscription
that observes the context's `theme` observable and sets `_isDark = true`
when the alias is `umb-dark-theme`. All other aliases (including
`umb-light-theme`, `umb-high-contrast-theme`, anything unrecognised, or the
context being absent) fall back to light.

The public element export contract (`export { ... as element }` at the bottom
of the file, plus the `HTMLElementTagNameMap` declaration) is preserved — it's
part of the manifest contract.

## Theme

Both stock themes are imported as side-effect CSS at editor-load time, with
each theme's rules scoped behind a `[data-theme="light"]` / `[data-theme="dark"]`
attribute selector on the editor wrapper so they don't collide. The element
observes `UMB_THEME_CONTEXT.theme` (the active backoffice theme alias) and
sets the wrapper's `data-theme` attribute to `dark` only when the alias is
`umb-dark-theme`. Everything else — `umb-light-theme`, `umb-high-contrast-theme`,
unknown aliases, no context — resolves to `light`.

This is a deliberate departure from `prefers-color-scheme`: Umbraco's
backoffice theme is independent of the user's OS dark-mode preference.

## Events

- Prism Code Editor exposes an `addListener('update', …)` API on its editor
  instance. The wrapper translates `update` callbacks into:
  - `this.value = editor.value`
  - `this.dispatchEvent(new UmbChangeEvent())`
- No `UmbInputEvent` translation is needed — we read the editor's `value`
  property directly. The external surface (a single `UmbChangeEvent`) is
  unchanged from the current implementation's perspective.

## Build / packaging

- Add `prism-code-editor` to `dependencies` in
  `src/Umbraco.Community.Contentment.Client/package.json`. Use whatever
  version `npm install prism-code-editor` resolves at implementation time
  (recorded as a `^x.y.z` caret range in `package.json`, exact in the
  lockfile). Not a `peerDependency`.
- Vite's existing `external: [/^@umbraco/]` is unchanged — `prism-code-editor`
  and its grammars are bundled into the output as expected.
- `chunkFileNames: '[name].js'` keeps lazy-loaded grammars at predictable
  filenames inside `wwwroot/App_Plugins/Contentment/`.

## Verification (manual — no test suite)

Run a host site (e.g. `Umbraco.Cms.17.x` on `http://localhost:21187`).
Success criterion in parentheses for each step.

1. Property editor loads on a Contentment Code Editor data type
   (no JS errors; `<uui-loader>` flashes once on first open).
2. Cycle through every entry in the dropdown, type a small snippet for each
   (syntax colours match the language).
3. Default new data type (defaults to Razor; `cshtml` grammar applied).
4. Toggle the Umbraco backoffice theme via the current-user theme picker
   (light → dark → high contrast)
   (editor renders dark when the backoffice is in `umb-dark-theme`, light
   for `umb-light-theme` and `umb-high-contrast-theme`).
5. Search (Ctrl+F), bracket matching, autoclose pairs, line numbers,
   active-line highlight, indent guides
   (all work as expected).
6. Manually configure a data type with `mode: ruby`
   (editor mounts as plain text, no error toast, no warning).
7. SQL data source / Templated List configuration editors that internally
   embed the Code Editor still work
   (they use the alias `Umb.Contentment.PropertyEditorUi.CodeEditor`, so
   pick up the new editor unchanged).
8. Compare layout for `labelOnTop: true` vs `labelOnTop: false`
   (editor sits flush in the property column for both — no horizontal
   misalignment).
9. Bundle size sanity check
   (compare gzipped output of `wwwroot/App_Plugins/Contentment/` before
   and after the swap; total package size goes down).

## Known risks / non-decisions

- **Bundle accounting is unmeasured** in this design — the assumption is
  Prism Code Editor is materially smaller than Monaco. We verify this in
  step 9 of the manual checklist; we do not commit to a numeric budget.
- **Razor grammar fidelity** — Prism's `cshtml` grammar may render
  edge-case Razor differently from Monaco. Acceptable per the brainstorm.
- **High-contrast theme** — the design renders Prism's light theme when
  the backoffice is in `umb-high-contrast-theme`. A dedicated high-contrast
  Prism theme can be added later as a follow-up if maintainers ask for it.
