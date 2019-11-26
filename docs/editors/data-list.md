<img src="../assets/img/logo.png" alt="Umbraco Contentment Logo" title="A shoebox of Umbraco happiness." height="130" align="right">

## Umbraco Contentment

### Data List

Data List is a property-editor that takes a data source and makes the values selectable in a list editor control.

_If that sounds too generic, think of it like this... take a data source, say a SQL query, and display the results in an editor, say a dropdown-list, or checkbox-list, or whatever!_

> This property-editor has taken some inspiration from the community package, [nuPickers](https://our.umbraco.com/packages/backoffice-extensions/nupickers/) by Hendy Racher, _(itself inspired by [a uComponents idea](https://gist.github.com/leekelleher/6183524))._


### How to configure the editor?

In your new Data Type, selected the "[Contentment] Data List" option. You will see the following configuration fields.

The two main fields are "**Data source**" and "**List editor**".

![Configuration Editor for Data List - empty state](data-list--configuration-editor-01.png)

Selecting the **Data source**, you will be presented with a selection of data sources, including .NET enumeration, file system, SQL, Umbraco entities, XML data.

![Configuration Editor for Data List - available data sources](data-list--configuration-editor-02.png)

For our example, let's choose **Umbraco Entity**. You will then be presented with configuration options for this data source.

![Configuration Editor for Data List - data source configuration (for Umbraco Entity)](data-list--configuration-editor-03.png)

Once you have configured the data source, press the **Done** button at the bottom of the overlay.

Next is to select and configure the **List editor**. You will be presented with a selection of configuration options.

![Configuration Editor for Data List - available list editors](data-list--configuration-editor-04.png)

For our example, let's choose **Checkbox List**. You will then be able to configure the editor options.

![Configuration Editor for Data List - list editor configuration (for Checkbox List)](data-list--configuration-editor-05.png)

Once you have configured both the **Data source** and **List editor** you can **Save** the Data Type and add it to your Document Type.


### How to use the editor?

Once you have added the configured Data Type on your Document Type, the Data List will be displayed.

![Data List property-editor - displaying the data source with a Checkbox List](data-list--property-editor-01.png)

The beauty of the **Data List** property-editor is that all of the list editors are hot-swappable. Meaning that if you wanted to use a **Radiobutton List** instead, no problem.

![Data List property-editor - displaying the data source with a Radiobutton List](data-list--property-editor-02.png)

or a **Dropdown List**?

![Data List property-editor - displaying the data source with a Dropdown List](data-list--property-editor-03.png)

or an **Item Picker**? _(This list editor is visually similar to Umbraco's Content Picker editor.)_

![Data List property-editor - displaying the data source with an Item Picker](data-list--property-editor-04.png)


### How to extend this with my own stuff?

You can extend Data List with your own custom data sources and list editors.


#### Extending with your own custom data source

For creating your own custom data source, you will need to create a new C# class that implements the `Umbraco.Community.Contentment.DataEditors.IDataListSource` interface.

This interface contains one method called `GetItems()`, which must return a `IEnumerable<DataListItem>` object type.

The `DataListItem` model is made up of four `string` properties: `Name`, `Value`, `Description` _(optional)_ and `Icon` _(optional)_.

Here's an example of a custom data source of time zones, _(leveraging .NET Framework's `System.TimeZoneInfo` API)_.

```csharp
public class TimeZoneDataSource : IDataListSource
{
    public string Name => "Time zones";

    public string Description => "Data source for all the time zones.";

    public string Icon => "icon-globe";

    public IEnumerable<DataListItem> GetItems()
    {
        var items = new List<DataListItem>();

        foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
        {
            items.Add(new DataListItem
            {
                Name = timezone.DisplayName,
                Value = timezone.BaseUtcOffset.ToString()
            });
        }

        return items;
    }
}
```

If you require extra configuration options on your custom data source, this can be done by adding extra properties on the class itself, and marking them with Umbraco's [`ConfigurationFieldAttribute`](https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.0/src/Umbraco.Core/PropertyEditors/ConfigurationFieldAttribute.cs).

```csharp
[ConfigurationField("alias", "Label (name)", "textstring", Description = "[Add a friendly description]")]
public string ConfigOption { get; set; }
```


#### Extending with your own custom list editor

For creating your own custom list editor, you will need to create a new C# class that implements the `Umbraco.Community.Contentment.DataEditors.IContentmentListItem` interface.

This interface contains two properties, `View` and `DefaultConfig` _(optional)_.

The `View` property should set the path of the AngularJS view file. This can be whatever you want it to be. The only prerequisite is that the AngularJS controller (for the view) will be passed the data source items, (an object array - a serialization of the `DataListItem` model), accessed by `$scope.model.config.items`.


### How to get the value?

The value for the Data List is an `IEnumerable<string>`, _(regardless of whether it's a single value or multiple values)._

To use this in your view templates, here are some examples.

For our example, we'll assume that your property's alias is `"dataList"`, then...

Using Umbraco's Models Builder...

```cshtml
<ul>
    @foreach (var item in Model.DataList)
    {
        <li>@item</li>
    }
</ul>
```

Without ModelsBuilder...

The weakly-typed API may give you some headaches, we suggest using strongly-typed, (or preferably Models Builder).

Here's an example of strongly-typed...

```cshtml
<ul>
    @foreach (var item in @(Model.Value<string>("dataList")))
    {
        <li>@item</li>
    }
</ul>
```


### Suggestions and ideas

Inspiration for how a Data List could be used...

#### Days of the week

Say you wanted to specify a business's opening days.

You could use the [`System.DayOfWeek`](https://docs.microsoft.com/en-us/dotnet/api/system.dayofweek) enum with a checkbox list to display the days of the week.

For the **Data source**, select **.NET Enumeration**. For the **Enumeration type**, select "mscorlib", then select "Day Of Week" option.

For the **List editor**, select the **Checkbox List** option.

![Days of the week example, using Data List property-editor, with DayOfWeek enum and Checkbox List editor](data-list--example-01-dayofweek.png)


#### Country picker

Say you wanted to select from a list of countries. You could use a third-party XML data source.

For the **Data source**, select **XML Data**.

- For the **URL**, enter "https://madskristensen.net/posts/files/countries.xml"
- For the **Items XPath**, enter "/countries/country"
- For the **Name XPath**, enter "text()"
- For the **Value XPath**, enter "@code"

For the **List editor**, select **Item Picker**, _maybe configure it with a nice Globe icon in blue?)_

![Country picker example, using Data List property-editor, with remote XML data source and Item Picker editor](data-list--example-02-countries.png)


