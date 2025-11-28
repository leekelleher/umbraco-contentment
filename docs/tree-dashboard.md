<img src="assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Tree Dashboard

By default, the package will display a Contentment tree item in the Settings section. Currently, this is used for promotional purposes, to give information about the package itself, newsletter sign-up, etc.


#### Disable tree dashboard

If you would prefer to disable the tree dashboard completely, you can use this code snippet to disable it.

##### For Umbraco v9+

Configuration to disable Contentment tree dashboard.

In your `appsettings.json` file, add this option inside the `"Umbraco"` section, add the following.

```json
{
    "Umbraco": {
        "Contentment": {
            "DisableTree": true
        }
    }
}
```

If you prefer to use a strongly-typed configuration in C# code, you can do this with the `.AddContentment(opts => { opts.DisableTree = true; })` extension method in your `Program.cs` (or `Startup.cs`) file `ConfigureServices()` method.

##### For Umbraco v8

Code snippet to disable Contentment tree dashboard.

Copy the C# class below. You can either save this to your `~/App_Code/` folder, or add it to your own code library.

```csharp
using Umbraco.Core.Composing;

namespace Our.Umbraco.Web
{
    public class DisableContentmentTreeComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableContentmentTree();
        }
    }
}
```

If you already have your own composer class, you can add the `composition.DisableContentmentTree();` line to it.

