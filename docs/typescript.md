<img src="assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### TypeScript

Contentment's types are bundled in the NuGet package's contentFiles.  
Specifically `contentFiles/any/any/vendor/Contentment/index.d.ts`.

It will be copied automatically to `/vendor/Contentment` in your project.

To opt out, set the msbuild property `Contentment_CopyTypes` to false.

```xml
<PropertyGroup>
  <Contentment_CopyTypes>false</Contentment_CopyTypes>
</PropertyGroup>
```

It is recommended to add it as an aliased path in your tsconfig.json:

```json
{
  "compilerOptions": {
    // ...
    "paths": {
      "@umbraco-community/contentment": ["./../vendor/Contentment/index.d.ts"]
      // ...
    }
  }
}
```

### Tests

At the time of writing, the compiled JavaScript is not copied to any projects,
except on publish with the right options to include Razor Class Library static files.

They may be copied to a test project within an msbuild project using the following task and making sure you have the `GeneratePathProperty` attribute on the package reference:

```xml
  <PackageReference Include="Umbraco.Community.Contentment" Version="6.*" GeneratePathProperty="true" />

  <Target Name="CopyContentmentBundleForTests">
    <PropertyGroup>
      <ContentmentBundleSourcePath>$(PkgUmbraco_Community_Contentment)/staticwebassets/App_Plugins/Contentment</ContentmentBundleSourcePath>
      <ContentmentBundleDestinationPath>$(MSBuildProjectDirectory)/Client/vendor/contentment</ContentmentBundleDestinationPath>
    </PropertyGroup>
    <ItemGroup>
      <ContentmentBundleFiles Include="$(ContentmentBundleSourcePath)/**/*.*" />
    </ItemGroup>
    <Message Importance="High" Text="Copying @(ContentmentBundleFiles-&gt;Count()) Contentment bundle files from '$(ContentmentBundleSourcePath)' to '$(ContentmentBundleDestinationPath)'" />
    <MakeDir Directories="$(ContentmentBundleDestinationPath)" Condition="!Exists('$(ContentmentBundleDestinationPath)')" />
    <Copy SourceFiles="@(ContentmentBundleFiles)" DestinationFiles="@(ContentmentBundleFiles->'$(ContentmentBundleDestinationPath)\%(RecursiveDir)%(Filename)%(Extension)')" SkipUnchangedFiles="true" />
  </Target>
```

Additionally the bundle should be imported in your test setup.  
For example in `setup.ts` when using Vitest.

```typescript
import fs from 'fs';
import path from 'path';
import { pathToFileURL } from 'url';

const bundle = path.resolve(__dirname, './vendor/contentment/index.js');
if (fs.existsSync(bundle)) {
  const bundleHref = pathToFileURL(bundle).href;
  await import(bundleHref).catch((e) => { console.error('Failed to import Contentment bundle.', e)});
}
```
