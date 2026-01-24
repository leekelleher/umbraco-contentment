# Umbraco.Community.Contentment.Client

TypeScript/Lit-based Umbraco Backoffice extension package for Contentment.

- **License**: MIT
- **Repository**: [GitHub](https://github.com/leekelleher/umbraco-contentment)
- **Peer Dependency**: `@umbraco-cms/backoffice ^16.0.0`

> For Umbraco Backoffice development patterns and architecture, see: [Umbraco.Web.UI.Client/CLAUDE.md](https://github.com/umbraco/Umbraco-CMS/blob/main/src/Umbraco.Web.UI.Client/CLAUDE.md)

---

## Quick Start

### Prerequisites

- Node.js >= 22 (see `.nvmrc`)
- npm >= 10

### Setup

```bash
npm install
```

### Common Commands

| Command | Description |
|---------|-------------|
| `npm run dev` | TypeScript compilation + Vite watch mode |
| `npm run build` | Full production build |
| `npm run build:api` | Build API types only |
| `npm run generate:server-api` | Regenerate OpenAPI client from Swagger spec |

### Build Output

Output directory: `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment`

---

## Quick Reference

### Directory Structure

| Directory | Purpose |
|-----------|---------|
| `src/api/` | Auto-generated OpenAPI client (types, SDK, client) |
| `src/components/` | Shared Lit web components |
| `src/condition/` | Extension condition implementations |
| `src/extensions/` | Extension implementations (block editor views, data sources, etc.) |
| `src/external/` | External library wrappers (liquidjs, sortablejs) |
| `src/global-context/` | Global context providers |
| `src/icons/` | Custom icon definitions |
| `src/localization/` | Language translations |
| `src/property-action/` | Property editor actions |
| `src/property-editor-ui/` | Property editor UI implementations (34+ editors) |
| `src/utils/` | Utility functions |
| `src/workspace/` | Workspace editor implementations |

### Entry Points

| File | Purpose |
|------|---------|
| `src/index.ts` | Main API exports (components, utilities) |
| `src/manifests.ts` | All extension manifests aggregated |

### Naming Conventions

| Type | Pattern | Example |
|------|---------|---------|
| Elements | `{name}.element.ts` | `text-input.element.ts` |
| Manifests | `manifest.ts` / `manifests.ts` | `manifests.ts` |
| Functions | `{name}.function.ts` | `parse-boolean.function.ts` |
| Context tokens | `{name}.context-token.ts` | `liquid.context-token.ts` |
| Custom elements | `contentment-{feature}` | `contentment-property-editor-ui` |
| Extension aliases | `Umb.Contentment.{Type}.{Name}` | `Umb.Contentment.PropertyEditorUi.TextInput` |

### Extension Types Implemented

- `propertyEditorUi` - Custom UI components for property editing
- `propertyEditorSchema` - Metadata/configuration for property editors
- `propertyAction` - Actions available on property editors
- `blockEditorCustomView` - Custom views for block editors (Liquid-based)
- `dataSources` - Data providers for UI components
- `dataListItemUi` - Custom renderers for data list items
- `displayMode` - Display mode configurations
- `globalContext` - Global context providers
- `conditions` - Conditional logic for extensions
- `workspace` - Workspace implementations
- `localization` - Language translations
- `icons` - Custom icon definitions

---

## Key Dependencies

| Package | Purpose |
|---------|---------|
| `liquidjs` | Liquid template engine for dynamic rendering |
| `sortablejs` | Drag-and-drop sortable list functionality |
| `@hey-api/openapi-ts` | Auto-generates TypeScript from OpenAPI specs |

---

## Configuration Files

| File | Purpose |
|------|---------|
| `vite.config.ts` | Build configuration (ES module library, two entry points) |
| `tsconfig.json` | TypeScript config (strict mode, ESNext target, decorators) |
| `openapi-ts.config.js` | OpenAPI client generation from Swagger spec |
| `.prettierrc.json` | Code formatting (120 char width, single quotes, tabs) |

---

## Architecture Notes

- **Umbraco Lit Web Components**: All UI elements extend Umbraco's Lit base class `UmbLitElement`
- **Manifest-driven**: Each extension declares metadata in `manifest.ts` files
- **Context API**: Uses Umbraco's context system for dependency injection
- **Lazy Loading**: External libraries (Liquid) loaded on demand
- **Type Safety**: Strict TypeScript throughout with generated API types

### Adding New Property Editor UIs

1. Create directory under `src/property-editor-ui/{name}/`
2. Add `{name}.element.ts` with Lit component
3. Add `manifest.ts` declaring the extension
4. Export from `src/property-editor-ui/manifests.ts`
5. Use VS Code snippet `new umb pe` for scaffolding

---

## Getting Help

- [Umbraco Backoffice Documentation](https://docs.umbraco.com/umbraco-cms/extending/backoffice)
- [Contentment Documentation](https://github.com/leekelleher/umbraco-contentment)
- [Umbraco Community Forum](https://forum.umbraco.com/)
