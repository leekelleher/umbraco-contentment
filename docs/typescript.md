<img src="assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### TypeScript

Contentment publishes its TypeScript definitions to the [GitHub Packages](https://github.com/leekelleher?tab=packages) npm registry as `@leekelleher/umbraco-contentment`.

> **This package currently ships TypeScript types only.**
> The runtime (JavaScript) continues to ship inside the `Umbraco.Community.Contentment` NuGet package as a static web asset (`App_Plugins/Contentment/*`) — Umbraco loads it at runtime.
> The npm package exists purely so consumer projects can get autocomplete and compile-time checking when extending Contentment's client side (e.g. implementing a custom `ContentmentDataSourceApi`).

#### Installation

The package is hosted on GitHub's npm registry, so your project needs an `.npmrc` file to tell npm where to resolve the `@leekelleher` scope, and a GitHub Personal Access Token with the `read:packages` scope.

Create (or append to) `.npmrc` in your project root:

```ini
@leekelleher:registry=https://npm.pkg.github.com
//npm.pkg.github.com/:_authToken=${GITHUB_TOKEN}
```

Set the `GITHUB_TOKEN` environment variable to a [Personal Access Token (classic)](https://github.com/settings/tokens) with the `read:packages` scope.

Then install the package as a dev dependency:

```bash
npm install --save-dev @leekelleher/umbraco-contentment
```

#### Usage

Import the types you need in your own extension code:

```ts
import type { ContentmentDataSourceApi, ContentmentDataSourceItem } from '@leekelleher/umbraco-contentment';

export class MyCustomDataSource implements ContentmentDataSourceApi {
  async getItems(): Promise<Array<ContentmentDataSourceItem>> {
    // ...
  }
}
```

Because declaration maps are shipped alongside the types, "Go to Definition" in VS Code will jump to Contentment's source TypeScript — useful when exploring the available APIs.

#### Versioning

The npm package version tracks the NuGet package version one-to-one. Pick the npm version that matches the `Umbraco.Community.Contentment` NuGet package installed in your Umbraco site, so the types reflect the runtime you actually have.

#### Further reading

- [GitHub Packages — Working with the npm registry](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-npm-registry#authenticating-to-github-packages)
