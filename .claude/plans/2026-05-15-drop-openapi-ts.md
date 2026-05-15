# Drop @hey-api/openapi-ts from the Contentment client

## Context

The Contentment client project has been carrying `@hey-api/openapi-ts` to generate a typed OpenAPI client from the server's Swagger spec. After a recurring pattern of breaking changes (most recently 0.66 → 0.97, plus a `bundle: false` → `bundle: true` regression that produced cryptic duplicate-`Client` type errors), the maintenance tax no longer matches the value.

The actual surface area:

- **9 stable endpoints** across 5 controller groups (ContentBlocks, DataList, DataPicker, Data, Meta).
- **625 LOC of generated code** vs. ~60 LOC if hand-typed (10× overhead).
- **7 consumer call sites**, all using the identical `ServiceClass.method({ client: umbHttpClient, body, query })` shape.
- **Flat DTOs** — no generics, no discriminated unions in the domain layer, no recursive shapes.
- **Same maintainer owns both sides.** The OpenAPI contract is internal, not a cross-team boundary.
- Endpoint count is expected to stay roughly steady (~10) over the next 12–18 months.

Outcome: replace the generated client with hand-rolled types and a thin fetch wrapper that uses `umbHttpClient` directly. Stop paying the 0.x version tax forever, drop the regenerate-with-running-host friction, eliminate the dep-alignment trap with `@umbraco-cms/backoffice`.

## Approach

1. **Hand-roll types.** Create `src/api/types.ts` with the small set of request/response models currently exported from `src/api/index.ts`. Source of truth is the existing `src/api/types.gen.ts` — copy the relevant flat-shape interfaces (`DataListItem`, `DataListEditorResponseModel`, `DataListConfigurationRequestModel`, `DataPickerEditorRequestModel`, `DataPickerEditorResponseModel`, `PagedModelDataListItemModel`, `ContentmentSettings`, `MetaConfigurationResponseModel`, `ConfigurationEditorItemRequestModel`, `DataTypePropertyPresentationModel`, `NotificationHeaderModel`, `ProblemDetails`, `EventMessageTypeModel`) verbatim into a single hand-maintained file, stripping the `@hey-api`-specific operation type unions.

2. **Replace SDK calls with direct `umbHttpClient` usage.** `umbHttpClient` is exported from `@umbraco-cms/backoffice/http-client` and is an `@hey-api/client-fetch` instance with `.get()`, `.post()` etc. Each call site changes from:

   ```ts
   const { data } = await tryExecute(this, DataListService.postDataListEditor({ client: umbHttpClient, body }));
   ```

   to:

   ```ts
   const { data } = await tryExecute(this, umbHttpClient.post<DataListEditorResponseModel>({
       url: '/umbraco/management/api/v1/contentment/data-list/editor',
       body,
   }));
   ```

   Consider a thin wrapper file `src/api/endpoints.ts` with one function per endpoint to centralise the URLs — keeps the URL strings out of property-editor code and mimics the existing service-method shape so the consumer diff stays small. ~50 LOC total.

3. **Delete generated artifacts and the generator.** Remove:

   - `src/api/client/` directory
   - `src/api/core/` directory
   - `src/api/client.gen.ts`
   - `src/api/sdk.gen.ts`
   - `src/api/types.gen.ts`
   - `src/api/index.ts` (replaced by new `src/api/types.ts` + `src/api/endpoints.ts`)
   - `openapi-ts.config.js`
   - From `package.json`: the `@hey-api/openapi-ts` devDependency and the `generate:server-api` script.

4. **Keep server-side OpenAPI infrastructure untouched.** The C# side (`ContentmentOperationIdHandler`, `ConfigureContentmentSwaggerGenOptions`, the Swagger group) stays — Swagger is still useful for manual API exploration and as documentation. We're only removing the client-side codegen step.

## Files to modify

### New
- `src/Umbraco.Community.Contentment.Client/src/api/types.ts` — hand-rolled DTOs (~60 LOC).
- `src/Umbraco.Community.Contentment.Client/src/api/endpoints.ts` — one async function per endpoint, ~50 LOC.

### Update (call sites — replace `*Service.method(...)` with new endpoint function)
- `src/property-editor-ui/content-blocks/content-block-type-configuration.element.ts` (line 16) — `ContentBlocksService.getElementTypes`
- `src/property-editor-ui/data-list/data-list.repository.ts` (line 93) — `DataListService.postDataListEditor`
- `src/property-editor-ui/data-picker/data-picker-modal.element.ts` (line 181) — `DataPickerService.postDataPickerSearch`
- `src/property-editor-ui/data-picker/data-picker.element.ts` (line 145) — `DataPickerService.postDataPickerEditor`
- `src/property-editor-ui/text-input/text-input.element.ts` (line 81) — `DataListService.postDataListEditor`
- `src/property-editor-ui/textbox-list/textbox-list.element.ts` (line 55) — `DataListService.postDataListEditor`
- `src/workspace/workspace.element.ts` (line 62) — `MetaService.getConfiguration`

Imports change from `'../api'` / `'@umbraco-cms/backoffice/http-client'` to a single `'../api/endpoints'` import per file.

### Update (config)
- `src/Umbraco.Community.Contentment.Client/package.json` — remove `@hey-api/openapi-ts` from `devDependencies`; remove `generate:server-api` from `scripts`.

### Delete
- `src/Umbraco.Community.Contentment.Client/openapi-ts.config.js`
- `src/Umbraco.Community.Contentment.Client/src/api/client/` (entire dir)
- `src/Umbraco.Community.Contentment.Client/src/api/core/` (entire dir)
- `src/Umbraco.Community.Contentment.Client/src/api/client.gen.ts`
- `src/Umbraco.Community.Contentment.Client/src/api/sdk.gen.ts`
- `src/Umbraco.Community.Contentment.Client/src/api/types.gen.ts`
- `src/Umbraco.Community.Contentment.Client/src/api/index.ts`

## Reused utilities (no changes)

- `tryExecute` from `@umbraco-cms/backoffice/resources` — wraps the promise the same way; the new endpoint functions are drop-in compatible with it.
- `umbHttpClient` from `@umbraco-cms/backoffice/http-client` — used by the new `endpoints.ts` helpers.

## Verification

1. `npx tsc --noEmit` from `src/Umbraco.Community.Contentment.Client/` — passes with zero errors. Today there are 9; target is 0.
2. `npm run build` from the client — successful build, output emitted into `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/`.
3. `npm run lint` (if present, otherwise skip) — passes.
4. `dotnet run --project src/Umbraco.Cms.17.x` — run the host. Exercise each affected editor manually in the backoffice:
   - Content Blocks editor → confirm element-type list loads (calls `getElementTypes`).
   - Data List editor (and text-input, textbox-list display modes) → confirm picker UI loads (calls `postDataListEditor`).
   - Data Picker editor → confirm both the editor config (`postDataPickerEditor`) and the search modal (`postDataPickerSearch`) work.
   - Workspace → confirm settings/version display loads (`getConfiguration`).
5. Confirm `package-lock.json` shrinks meaningfully (no more `@hey-api/*` transitive tree).

## Notes

- Branch name (per convention): `dev/wip/drop-openapi-ts`, based on the current release branch (`dev/v6.x`).
- Commit-trailer convention (no `Co-Authored-By: Claude`) applies when implementation lands.
