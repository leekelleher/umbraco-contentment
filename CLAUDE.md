# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Overview

**Contentment for Umbraco** — a community package distributing property editors, data sources, and related components for Umbraco CMS. Ships as the `Umbraco.Community.Contentment` NuGet package.

The repository is a **two-project package**:

- `src/Umbraco.Community.Contentment/` — C# Razor class library (`Microsoft.NET.Sdk.Razor`), multi-targeting `net9.0` (Umbraco v16) and `net10.0` (Umbraco v17). Produces the NuGet package.
- `src/Umbraco.Community.Contentment.Client/` — TypeScript/Lit Umbraco backoffice extension (ES module library). Built output is emitted into the C# project's `wwwroot/App_Plugins/Contentment/` so it ships inside the NuGet package as a static web asset. Client-specific conventions live in `.claude/rules/client.md`.

The `src/Umbraco.Cms.16.x/` and `src/Umbraco.Cms.17.x/` folders are **demo/runtime host sites** used for local development and manual testing (configured for unattended install with SQLite and uSync). They are not part of the shipped package.

## Branching

The active development branch is **`contrib`** (v6.x, current). Each major Contentment version has its own long-lived `dev/vX.x` branch that targets specific Umbraco versions — check `.github/CONTRIBUTING.md` for the full mapping before backporting.

## Common Commands

Run from the repo root unless noted.

### Client (TypeScript/Lit)

```bash
cd src/Umbraco.Community.Contentment.Client
npm install
npm run dev                  # tsc + vite build --watch (active development)
npm run build                # tsc + vite build + tsc types (production package + .d.ts)
```

Node >= 22 (pinned in `.nvmrc`). The build writes directly into `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/`.

`npm run generate:server-api` regenerates the OpenAPI client under `src/api/` from `http://localhost:21187/umbraco/swagger/contentment/swagger.json` — this requires a running Umbraco host (e.g. `Umbraco.Cms.17.x`) on that port. Don't run it without the host up.

### Server (C# package)

```bash
# Pack the NuGet (matches what CI does, minus versioning)
build/build-pkgs.cmd
# …which runs:
dotnet pack src/Umbraco.Community.Contentment/Umbraco.Community.Contentment.csproj -c Release -o artifacts
```

The release workflow (`.github/workflows/release.yml`, triggered by a SemVer tag push) builds the client first, then `dotnet pack` with `/p:Version=${tag}`, then pushes to GitHub Packages + NuGet.org.

### Local run / manual testing

Run one of the host projects (e.g. `dotnet run --project src/Umbraco.Cms.17.x`). The host references `Umbraco.Community.Contentment.csproj` via `ProjectReference`, so changes rebuild into the host on run. The client must be rebuilt separately (use `npm run dev` in parallel).

**There is no automated test suite** — verification is done by running a host site and exercising the editors in the backoffice.

## Architecture

### C# project layout (`src/Umbraco.Community.Contentment/`)

- `DataEditors/{EditorName}/` — one folder per property editor (e.g. `TextInput/`, `ContentBlocks/`, `DataList/`). Each typically contains a `{Name}DataEditor.cs` (the `[DataEditor]`-attributed class), a `ValueConverter`, and sometimes configuration field classes.
- `DataEditors/_/` — shared interfaces (`IContentmentDataSource`, `IContentmentListItem`, `IContentmentDisplayMode`, etc.) plus shared `ConfigurationFields/`, `DataSources/`, and `DataValueEditors/`.
- `Composing/ContentmentComposer.cs` — single `IComposer` wiring up services, custom collection builders (`ContentmentListItemCollectionBuilder`, `ContentmentDataListItemPropertyValueConverterCollectionBuilder`), config binding, and notification handlers.
- `Api/Management/` — backoffice Management API controllers (Umbraco 14+ style). Exposed under the `contentment` Swagger group (see `ContentmentOperationIdHandler`, `ConfigureContentmentSwaggerGenOptions`).
- `ContentmentPackageManifestReader.cs` — registers the client bundle by emitting a package manifest that points at `App_Plugins/Contentment/*`.
- `Constants.cs` / `ContentmentConstants.cs` — `Constants.Internals` holds all alias/path/namespace prefixes; `ContentmentConstants` is the public surface (editor aliases, config keys). New editors should follow the existing naming pattern (`DataEditorAliasPrefix + "Name"`, UI alias `Umb.Contentment.PropertyEditorUi.Name`).
- `Migrations/` — `ContentmentPlan` chains install/upgrade migrations.
- `Notifications/` — `INotificationHandler` implementations (telemetry, data type save/delete, content blocks copy).
- `Services/ContentmentContentContext.cs` — abstraction for the current content context used by editors.

### Client

Client conventions, layout, naming rules, and the list of extension types implemented are in `.claude/rules/client.md` — path-scoped to the client project so it auto-loads only when Claude reads files under `src/Umbraco.Community.Contentment.Client/`.

High level: Lit web components registered via manifest-driven extensions, one folder per property editor UI under `src/property-editor-ui/`, aggregated in `src/manifests.ts`. Vite builds two entry points (`index.ts`, `manifests.ts`) as ESM, externalising `@umbraco/*`.

### How a property editor is put together

A Contentment property editor typically spans both projects:

1. **Schema (C#)**: `DataEditors/{Name}/{Name}DataEditor.cs` — `[DataEditor]`-attributed class defining alias, value type, and optional configuration editor. Often paired with a `ValueConverter` for published content rendering.
2. **UI (TS)**: `src/property-editor-ui/{name}/{name}.element.ts` + `manifest.ts` registering a `propertyEditorUi` extension keyed to the C# alias. Exported from `src/property-editor-ui/manifests.ts`.
3. **Wiring**: the alias on the C# side must match what the TS manifest targets. Prefixes are centralised in `Constants.Internals`.

## Conventions / Constraints

From `.github/CONTRIBUTING.md` (upstream maintainer rules):

- **Do not upgrade NuGet dependencies.** Version ranges in the `.csproj` are intentional.
- **Do not modify licensing headers or bump the copyright year.**
- Start with an issue or discussion before non-trivial PRs.

Other repository conventions:

- All C# files carry an MPL-2.0 header comment (older files) or are MIT (newer). The NuGet package itself is licensed MIT (`PackageLicenseExpression`). Do not change headers when editing existing files.
- `.editorconfig` in `src/` governs C# style — let it drive formatting, don't override.
- The `src/packages/` folder contains vendored legacy `packages.config`-style dependencies; leave it alone unless explicitly working on it.
- `Umbraco.Community.Contentment/DataEditors/InputList/**` and `wwwroot/**` are excluded from compilation (`<Compile Remove>` in the `.csproj`) — they are work-in-progress or generated output.
- **`<lee-was-here></lee-was-here>` is an easter egg, not debug code.** It's the intentional placeholder rendered by client components when an expected child component is missing (e.g. when a `propertyEditorUiAlias` is unset, or a custom component manifest can't be resolved). Do not "fix", remove, or replace it with `nothing` — it's a deliberate signature.
- **For inline status / error / warning UI in the client, use `<contentment-info-box>`.** Default shape: `<contentment-info-box type="warning" icon="icon-alert" headline=… message=…>` (also supports a `compact` boolean for the empty-state pattern). Don't introduce bespoke styled `<div>` containers for these states — match the existing component for visual consistency.
