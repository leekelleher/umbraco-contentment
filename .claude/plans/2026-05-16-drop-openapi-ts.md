# Drop @hey-api/openapi-ts (client) and the server-side OpenAPI surface

## Context

Two related cleanups under the same arc: right-size the OpenAPI/Swagger toolchain for a solo-maintained extension package.

**Client-side (decided 2026-05-15).** The Contentment client has been carrying `@hey-api/openapi-ts` to generate a typed OpenAPI client from the server's Swagger spec. After a recurring pattern of breaking changes (most recently 0.66 → 0.97, plus a `bundle: false` → `bundle: true` regression that produced cryptic duplicate-`Client` type errors), the maintenance tax no longer matches the value.

**Server-side (decided 2026-05-16).** Once the client codegen is gone, the C# Swagger infrastructure has no real consumer. The API is not a documented public contract; it exists only to serve the property editors that ship inside the same NuGet. Keeping a visible Swagger surface broadcasts a contract we haven't promised to keep — silent-drift waiting to happen. We reserve the right to change direction.

**Versioning (decided 2026-05-16).** Non-breaking: target **v6.2** (minor). The published npm package's public surface (`@leekelleher/umbraco-contentment` on GitHub Packages re-exports everything from `src/api/index.ts` via `src/index.ts`) is preserved by hand-rolling thin Service-class wrappers around `umbHttpClient`. Internal call sites stay unchanged; external consumers of `DataListService` etc. see no break. HTTP API URLs, request/response shapes, and auth are all preserved.

The actual surface area:

- **9 stable endpoints** across 5 controller groups (ContentBlocks, DataList, DataPicker, Data, Meta).
- **625 LOC of generated client code** vs. ~100 LOC if hand-rolled (~60 LOC of types + ~80 LOC of Service-class wrappers).
- **7 client call sites**, all using the identical `ServiceClass.method({ client: umbHttpClient, body, query })` shape — preserved by the wrapper approach.
- **Flat DTOs** — no generics, no discriminated unions, no recursive shapes.
- **~50 LOC of server-side Swagger config** (3 files + 2 Composer lines) plus dead decoration across 6 controllers.
- **Same maintainer owns both sides.** The OpenAPI contract is internal, not a cross-team boundary.
- Endpoint count expected to stay roughly steady (~10) over the next 12–18 months.

Outcome: replace the generated client with hand-rolled types and hand-rolled Service-class wrappers (preserving the existing API surface for back-compat); hide all Contentment controllers from Swagger entirely; remove dead OpenAPI decoration. Stop paying the 0.x version tax forever, drop the regenerate-with-running-host friction, eliminate the dep-alignment trap with `@umbraco-cms/backoffice`, and stop advertising an undocumented internal API as a contract — all without a breaking-change bump.

## Approach

### Client-side

1. **Hand-roll types.** Create `src/api/types.ts` with the small set of request/response models currently exported from `src/api/index.ts`. Source of truth is the existing `src/api/types.gen.ts` — copy the relevant flat-shape interfaces (`DataListItem`, `DataListEditorResponseModel`, `DataListConfigurationRequestModel`, `DataPickerEditorRequestModel`, `DataPickerEditorResponseModel`, `PagedModelDataListItemModel`, `ContentmentSettings`, `MetaConfigurationResponseModel`, `ConfigurationEditorItemRequestModel`, `DataTypePropertyPresentationModel`, `NotificationHeaderModel`, `ProblemDetails`, `EventMessageTypeModel`) verbatim into a single hand-maintained file, stripping the `@hey-api`-specific operation type unions. Also hand-roll a minimal `Options<T>` type — structurally compatible with how the generator's `Options` type was used externally (e.g. `type Options<T = unknown> = T & { client?: unknown; throwOnError?: boolean }`).

2. **Hand-roll Service classes as thin wrappers, preserving the existing API surface.** Create `src/api/endpoints.ts` containing the same five Service classes (`ContentBlocksService`, `DataListService`, `DataPickerService`, `DataService`, `MetaService`) with the same static-method names and signatures as today. Each method is a thin wrapper that uses `umbHttpClient` directly. The `client` argument is accepted on the signature for back-compat and ignored at runtime (one-line comment explains). Example:

   ```ts
   export class DataListService {
       /** `client` argument retained for back-compat with the previous generated SDK; ignored — `umbHttpClient` is always used. */
       static postDataListEditor(args: { client?: unknown; body: DataListConfigurationRequestModel }) {
           return umbHttpClient.post<DataListEditorResponseModel>({
               url: '/umbraco/management/api/v1/contentment/data-list/editor',
               body: args.body,
           });
       }
   }
   ```

   Because `umbHttpClient` IS a `@hey-api/client-fetch` instance, the return shape (`RequestResult<T>`) matches the generator's output exactly — `const { data } = await tryExecute(this, ...)` keeps working everywhere with no consumer changes.

3. **Replace the generated `src/api/index.ts` with a hand-rolled re-export hub** that exports from `types.ts` and `endpoints.ts`, keeping every name currently exported. The 7 internal client call sites do **not** change. The published npm package's public surface (`@leekelleher/umbraco-contentment`) does **not** change.

4. **Delete generated client artifacts and the generator.** Remove:
   - `src/api/client/` directory
   - `src/api/core/` directory
   - `src/api/client.gen.ts`
   - `src/api/sdk.gen.ts`
   - `src/api/types.gen.ts`
   - `openapi-ts.config.js`
   - From `package.json`: the `@hey-api/openapi-ts` devDependency and the `generate:server-api` script.

### Server-side

5. **Hide all Contentment controllers from Swagger entirely.**
   - On `ContentmentControllerBase`: replace `[ApiExplorerSettings(GroupName = Constants.Internals.ProjectName)]` with `[ApiExplorerSettings(IgnoreApi = true)]`. Also remove `[MapToApi(Constants.Internals.ProjectAlias)]` and the now-unused `using Umbraco.Cms.Api.Common.Attributes;` import.
   - Remove the shadowing `[ApiExplorerSettings(GroupName = "...")]` attribute from each of the 5 derived controllers — they currently override the base and would defeat `IgnoreApi = true`.

6. **Remove dead `[ProducesResponseType]` decoration** (10 attributes across 5 controllers). With Swagger off, they have zero functional effect at runtime and misleadingly signal a documented API contract. Tidy up any `using Microsoft.AspNetCore.Http;` and `using Microsoft.AspNetCore.Mvc;` imports that become unused as a result.

7. **Delete server-side Swagger infrastructure.**
   - Delete `Api/Management/Configuration/ConfigureContentmentSwaggerGenOptions.cs` (also contains `ContentmentApiOperationSecurityFilter`, only used for the Swagger doc — safe to delete; auth is enforced by `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]` on the controller base, not by this filter).
   - Delete `Api/Management/Configuration/ContentmentOperationIdHandler.cs`.
   - Remove the two lines from `Composing/ContentmentComposer.cs`:
     - `.AddSingleton<IOperationIdHandler, ContentmentOperationIdHandler>()`
     - `.ConfigureOptions<ConfigureContentmentSwaggerGenOptions>()`
   - Drop the now-unused `using Umbraco.Community.Contentment.Api.Management;` from the Composer if no other reference remains.

### Things deliberately left alone

- **Client call sites.** All 7 consumer files (`text-input.element.ts` etc.) stay literally unchanged thanks to the wrapper-preserves-signature approach.
- **`src/api/index.ts` exports.** Same names re-exported — npm public surface preserved.
- `VersionedApiContentmentRouteAttribute` — structural URL routing (prefixes `/contentment/...`), not Swagger. Stays.
- `Constants.Internals.ProjectAlias` — still used by the route attribute and `ContentmentPackageManifestReader`. Stays.
- `[MapToApiVersion("1.0")]` on every action — API *versioning* (Asp.Versioning), load-bearing for routing. Don't confuse with `[MapToApi]`. Stays.
- `[Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]` on the base — real auth enforcement, independent of Swagger. Stays.
- Direct NuGet references — there are none for Swashbuckle/Microsoft.OpenApi; they come transitively via Umbraco. Nothing to remove from the csproj.

## Files to modify

### New
- `src/Umbraco.Community.Contentment.Client/src/api/types.ts` — hand-rolled DTOs + `Options<T>` (~60 LOC).
- `src/Umbraco.Community.Contentment.Client/src/api/endpoints.ts` — hand-rolled Service classes wrapping `umbHttpClient` (~80 LOC).

### Replace (client — generated index becomes hand-rolled)
- `src/Umbraco.Community.Contentment.Client/src/api/index.ts` — replace generated content with a hand-rolled re-export hub: `export * from './types.js';` and `export * from './endpoints.js';`. All currently exported names preserved.

### Update (client — config)
- `src/Umbraco.Community.Contentment.Client/package.json` — remove `@hey-api/openapi-ts` from `devDependencies`; remove `generate:server-api` from `scripts`.

### Update (server — controller base)
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/ContentmentControllerBase.cs` — replace `[ApiExplorerSettings(GroupName = …)]` with `[ApiExplorerSettings(IgnoreApi = true)]`; remove `[MapToApi(…)]` and the `using Umbraco.Cms.Api.Common.Attributes;` import.

### Update (server — derived controllers; remove shadowing `[ApiExplorerSettings]` and all `[ProducesResponseType]` attributes)
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/ContentBlocks/ContentBlocksController.cs`
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/Data/AssemblyEnumController.cs`
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/DataList/DataListController.cs`
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/DataPicker/DataPickerController.cs`
- `src/Umbraco.Community.Contentment/Api/Management/Controllers/Meta/MetaConfigurationController.cs`

### Update (server — composer)
- `src/Umbraco.Community.Contentment/Composing/ContentmentComposer.cs` — remove the two service registrations and any now-unused `using`s.

### Delete (client)
- `src/Umbraco.Community.Contentment.Client/openapi-ts.config.js`
- `src/Umbraco.Community.Contentment.Client/src/api/client/` (entire dir)
- `src/Umbraco.Community.Contentment.Client/src/api/core/` (entire dir)
- `src/Umbraco.Community.Contentment.Client/src/api/client.gen.ts`
- `src/Umbraco.Community.Contentment.Client/src/api/sdk.gen.ts`
- `src/Umbraco.Community.Contentment.Client/src/api/types.gen.ts`

### Delete (server)
- `src/Umbraco.Community.Contentment/Api/Management/Configuration/ConfigureContentmentSwaggerGenOptions.cs`
- `src/Umbraco.Community.Contentment/Api/Management/Configuration/ContentmentOperationIdHandler.cs`

## Reused utilities (no changes)

- `tryExecute` from `@umbraco-cms/backoffice/resources` — wraps the promise the same way; the new Service-class methods return the same `RequestResult<T>` shape as the generator's output, so destructuring continues to work.
- `umbHttpClient` from `@umbraco-cms/backoffice/http-client` — used internally by every method in `endpoints.ts`.

## Verification

1. **Client compiles.** `npx tsc --noEmit` from `src/Umbraco.Community.Contentment.Client/` — passes with zero errors (today's baseline: 9 errors).
2. **Client builds.** `npm run build` from the client — successful build, output emitted into `../Umbraco.Community.Contentment/wwwroot/App_Plugins/Contentment/`.
3. **Server compiles.** `dotnet build src/Umbraco.Community.Contentment` — succeeds.
4. **`package-lock.json` shrinks.** No more `@hey-api/*` transitive tree.
5. **npm export surface preserved.** Spot-check `dist/api/index.d.ts` after `npm run build` — every name currently exported from `src/api/index.ts` (the 5 Service classes, the DTOs, `Options`, `EventMessageTypeModel`) is still exported.
6. **Run the host:** `dotnet run --project src/Umbraco.Cms.17.x`.
   - **Swagger UI invisibility:**
     - Visit `/umbraco/swagger/contentment/swagger.json` — should return **404** (the doc no longer exists).
     - Visit `/umbraco/swagger/` and inspect the main Management API document — Contentment endpoints should **not appear** anywhere.
   - **Endpoints still function:** exercise each affected editor manually in the backoffice:
     - Content Blocks editor → element-type list loads (`getElementTypes`).
     - Data List editor (and text-input, textbox-list display modes) → picker UI loads (`postDataListEditor`).
     - Data Picker editor → editor config (`postDataPickerEditor`) and search modal (`postDataPickerSearch`) work.
     - Workspace → settings/version display loads (`getConfiguration`).
   - **Auth still enforced:** make an unauthenticated request (e.g. via `curl` without backoffice cookies) to `/umbraco/management/api/v1/contentment/meta/configuration` — should return **401/403**, confirming auth survived the cleanup of the Swagger security filter.

## Notes

- **Versioning:** v6.2 (minor), non-breaking. See the versioning paragraph in Context.
- **Branch** (per convention): `dev/wip/drop-openapi-ts`, based on the current release branch (`dev/v6.x`). Already checked out.
- **Commit-trailer convention** (no `Co-Authored-By: Claude`) applies when implementation lands.
- **Why `endpoints.ts` uses `class { static }`:** the Service-class shape is preserved purely for npm-export back-compat with the previous generated SDK. A hand-rolled-from-scratch design would use free functions; the wrapper exists to avoid breaking external consumers of `@leekelleher/umbraco-contentment`. A short comment in the file should explain this.
- **Blog-post draft** already exists in conversation history (response.md, 7235 chars) — when polishing, update the closing section to reflect the deeper "we also dropped server-side OpenAPI" arc, and remove any "this is a breaking change" framing since the wrapper approach kept it minor.
