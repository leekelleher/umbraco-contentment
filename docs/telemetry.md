<img src="assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Telemetry

Since version 1.2.0, by default, the package has been collecting telemetry data. This provides me with insights to which of the editors are being used, so that I can make informed decisions on how to focus my future development efforts.

When a Data Type is saved in the CMS backoffice, minimal data about the property-editor is collected and a request is sent to my web-server. The data is sent anonymously, no personal or sensitive data is collected.

Here is an example of the JSON data that is sent.

```json
{
    "dataType": "4E7D6B3A-F959-42E4-921E-081BC0E9E7EE",
    "editorAlias": "DataList",
    "umbracoId": "0403E47E-EFE7-4CF2-8E97-148681DAFC10",
    "umbracoVersion": "8.6.8",
    "contentmentVersion": "1.3.0",
    "dataTypeConfig": {
        "dataSource": "EnumDataListSource",
        "listEditor": "CheckboxListDataListEditor",
    }
}
```

The JSON is then encoded as Base64, sent to my web-server, where it is then decoded and stored in a database.

**A note about the `umbracoId` value.** Currently, (for v1.x), the value is a GUID, derived from an MD5 hash of the `Server.MachineName`. _(Note, since I encrypt the machine name, it is never known to me)._ Once I have increased the minimum Umbraco dependency (for Contentment v2.x) to beyond v8.10, I will use the same GUID as Umbraco does for their own telemetry purposes.

For information about the data and analysis, please go to: <https://leekelleher.com/umbraco/contentment/telemetry/>


#### Disable telemetry feature

If you would prefer to disable the telemetry feature completely, you can use this code snippet to disable it.

##### For Umbraco v9+

Configuration to disable Contentment telemetry.

In your `appsettings.json` file, add this option inside the `"Umbraco"` section, add the following.

```json
{
    "Umbraco": {
        "Contentment": {
            "DisableTelemetry": true
        }
    }
}
```

If you prefer to use a strongly-typed configuration in C# code, you can do this with the `.AddContentment(opts => { opts.DisableTelemetry = true; })` extension method in your `Program.cs` (or `Startup.cs`) file `ConfigureServices()` method.

##### For Umbraco v8

Code snippet to disable Contentment telemetry.

Copy the C# class below. You can either save this to your `~/App_Code/` folder, or add it to your own code library.

```csharp
using Umbraco.Core.Composing;

namespace Our.Umbraco.Web
{
    public class DisableContentmentTelemetryComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.DisableContentmentTelemetry();
        }
    }
}
```

If you already have your own composer class, you can add the `composition.DisableContentmentTelemetry();` line to it.

