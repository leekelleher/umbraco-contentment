---
paths:
  - "src/Umbraco.Community.Contentment.Client/**"
---

# Client project conventions

Scoped to `src/Umbraco.Community.Contentment.Client/` — the TypeScript/Lit Umbraco backoffice extension that builds into the C# project's `wwwroot/App_Plugins/Contentment/`.

## Peer dependency

`@umbraco-cms/backoffice ^16.0.0` — only import from packages already available in the Umbraco backoffice. Vite externalises everything matching `/^@umbraco/`, so adding a new `@umbraco-cms/*` import means relying on the host providing it.

## `src/` layout

| Folder | Purpose |
|---|---|
| `api/` | Hand-rolled Contentment Management API client — `types.ts` (DTOs + operation types), `endpoints.ts` (Service classes wrapping `umbHttpClient`), `index.ts` (re-export hub) |
| `components/` | Shared Lit web components |
| `condition/` | Extension `condition` implementations |
| `extensions/` | Extension implementations (block editor views, data sources, etc.) |
| `external/` | Wrappers around external libraries (`liquidjs`, `sortablejs`) |
| `global-context/` | Global context providers |
| `icons/` | Custom icon definitions |
| `localization/` | Language translations |
| `property-action/` | Property editor actions |
| `property-editor-ui/` | Property editor UI implementations (one folder per editor) |
| `utils/` | Utility functions |
| `workspace/` | Workspace editor implementations |

## Entry points and build

- `src/index.ts` — public API re-exports (components, utilities, aggregated types).
- `src/manifests.ts` — aggregated extension manifests from every subtree.
- Vite builds both as ESM with `formats: ['es']`, externalising `@umbraco/*`. Output goes to `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/` so it ships inside the NuGet package as a static web asset.
- `npm run generate:types` — emits TypeScript declarations (`.d.ts` + `.d.ts.map`) into `dist/` via `tsconfig.types.json`. Also chained at the end of `npm run build`. The `dist/` output is published to GitHub Packages as `@leekelleher/umbraco-contentment` on release.

## Extension types in use

- `propertyEditorUi` — custom UI components for property editing
- `propertyEditorSchema` — metadata/configuration for property editors
- `propertyAction` — actions available on property editors
- `blockEditorCustomView` — custom views for block editors (Liquid-based)
- `dataSources` — data providers for UI components
- `dataListItemUi` — custom renderers for data list items
- `displayMode` — display mode configurations
- `globalContext` — global context providers
- `conditions` — conditional logic for extensions
- `workspace` — workspace implementations
- `localization` — language translations
- `icons` — custom icon definitions

## Naming conventions

| Thing | Pattern | Example |
|---|---|---|
| Element files | `{name}.element.ts` | `text-input.element.ts` |
| Function files | `{name}.function.ts` | `parse-boolean.function.ts` |
| Context tokens | `{name}.context-token.ts` | `liquid.context-token.ts` |
| Per-feature manifest file | `manifests.ts` (plural) | `property-editor-ui/text-input/manifests.ts` |
| Aggregated manifest file | `manifests.ts` | `src/manifests.ts` |
| Custom element tag | `contentment-{feature}` | `contentment-property-editor-ui-text-input` |
| Manifest extension alias | `Umb.Contentment.{Type}.{Name}` | `Umb.Contentment.PropertyEditorUi.TextInput` |

The manifest alias mirrors the C# side's `Constants.Internals.DataEditorAliasPrefix + "Name"` pattern — a new editor's C# alias and TS manifest alias must line up.

## Architecture notes

- All UI elements extend `UmbLitElement` (Umbraco's Lit base class).
- Extension registration is manifest-driven; nothing is imperatively registered.
- External libs like Liquid are **lazy-loaded** on demand, not imported eagerly.
- Strict TypeScript throughout (`strict: true`); the hand-rolled `api/` client provides typed server access via `umbHttpClient`.

## Key runtime / tooling dependencies

- `liquidjs` — Liquid template engine, used by `blockEditorCustomView` extensions.
- `sortablejs` — drag-drop for sortable lists.

The `src/api/` client is hand-maintained. When the C# request/response models change, update `src/api/types.ts` to match.

## Formatting

`.prettierrc.json` governs TS/JS formatting: 120-char width, single quotes, tabs. Let it drive formatting — don't override in-file.

## Adding a new property editor UI

1. Create `src/property-editor-ui/{name}/` with `{name}.element.ts` + `manifests.ts`.
2. The `manifests.ts` must declare an alias matching the C# `DataEditor`'s UI alias (`Umb.Contentment.PropertyEditorUi.{Name}`).
3. Export the feature's manifests from `src/property-editor-ui/manifests.ts` so they're bundled into the root `src/manifests.ts` entry point.
