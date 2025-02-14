<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data List

Data List is a property-editor that takes a data source and makes the values selectable in a list editor control.

_If that sounds too generic, think of it like this... take a data source, say a SQL query, and display the results in an editor, say a dropdown-list, or checkbox-list, or whatever!_

> This property-editor has taken some inspiration from the community package, [nuPickers](https://our.umbraco.com/packages/backoffice-extensions/nupickers/) by Hendy Racher, _(itself inspired by [a uComponents idea](https://gist.github.com/leekelleher/6183524))._
> **Please note,** Data List is not a drop in replacement for nuPickers. Data List does not offer like-for-like features.


### How to configure the editor?

In your new Data Type, selected the "[Contentment] Data List" option. You will see the following configuration fields.

The two main fields are "**Data source**" and "**List editor**".

![Configuration Editor for Data List - empty state](data-list--configuration-editor-01.png)

Selecting the **Data source**, you will be presented with a selection of data sources, including .NET enumeration, file system, Umbraco content, SQL, CSV, JSON and XML data.

> [An extensive list of all the **built-in data-sources** is available](../data-sources/README.md).

![Configuration Editor for Data List - available data sources](data-list--configuration-editor-02.png)

For our example, let's choose **File System**. You will then be presented with configuration options for this data source.

![Configuration Editor for Data List - data source configuration (for File System)](data-list--configuration-editor-03.png)

Once you have configured the data source, press the **Done** button at the bottom of the overlay.

Next is to select and configure the **List editor**. You will be presented with a selection of configuration options.

![Configuration Editor for Data List - available list editors](data-list--configuration-editor-04.png)

For our example, let's choose **Checkbox List**. You will then be able to configure the editor options.

![Configuration Editor for Data List - list editor configuration (for Checkbox List)](data-list--configuration-editor-05.png)

Once you have configured both the **Data source** and **List editor** you can **Save** the Data Type and add it to your Document Type.


### How to use the editor?

Once you have added the configured Data Type on your Document Type, the Data List will be displayed on the content page's property panel.

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

For creating your own custom data source, you will need to create a new C# class that implements the [`Umbraco.Community.Contentment.DataEditors.IContentmentDataSource`](https://github.com/leekelleher/umbraco-contentment/blob/dev/v6.x/src/Umbraco.Community.Contentment/DataEditors/_/IContentmentDataSource.cs) interface.

This interface contains one method called `GetItems(config)`, which must return a `IEnumerable<DataListItem>` object type.

The `DataListItem` model is made up of four `string` properties: `Name`, `Value`, `Description` _(optional)_ and `Icon` _(optional)_.

Here's an example of a custom data source of time zones, _(leveraging .NET's `System.TimeZoneInfo` API)_.

```csharp
public class TimeZoneDataSource : IContentmentDataSource
{
    public string Name => "Time zones";

    public string Description => "Data source for all the time zones.";

    public string Icon => "icon-globe";

    public string Group => "Custom";

    public OverlaySize OverlaySize => OverlaySize.Small;

    public Dictionary<string, object> DefaultValues => default;

    public IEnumerable<ConfigurationField> Fields => default;

    public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
    {
        var items = new List<DataListItem>();

        foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
        {
            items.Add(new DataListItem
            {
                Name = timezone.DisplayName,
                Value = timezone.Id
            });
        }

        return items;
    }
}
```

If you require extra configuration options on your custom data source, this can be done by adding properties on the `Fields` property. A property is defined using Umbraco's [`ConfigurationField`](https://github.com/umbraco/Umbraco-CMS/blob/release-8.0.0/src/Umbraco.Core/PropertyEditors/ConfigurationField.cs) model.

```csharp
public IEnumerable<ConfigurationField> Fields => new ConfigurationField[]
{
    new ConfigurationField
    {
        Key = "alias",
        Name = "Label (name)",
        Description = "[Add a friendly description]",
        View = "textstring"
    }
}
```

##### Accessing contextual content

If you need to access contextual data from the current Umbraco content node, there's an `IContentmentContentContext` service which can be injected into your constructor. The `isParent` flag indicates whether the returned values relate to the current node, or it's parent node (for example, when editing an element item).

```csharp
public class BlogCategoriesDataSource : IContentmentDataSource
{
    private readonly IContentmentContentContext _contentmentContentContext;

    public BlogCategoriesDataSource(IContentmentContentContext contentmentContentContext)
    {
        _contentmentContentContext = contentmentContentContext;
    }

    // <snip>

    public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
    {
        // It's more performant to just get the ID if you only need that...
        // var currentId = _contentmentContentContext.GetCurrentContentId(out bool isParent);

        // ...or you can get the IPublishedContent
        var currentPage = _contentmentContentContext.GetCurrentContent(out bool isParent);

        var blog = currentPage?.AncestorOrSelf<Blog>();
        return blog?.Categories?.Select(x => new DataListItem() { Name = x, Value = x });
    }
}
```

#### Providing custom values for published content models

As explained in the [*How to get the value?*](#how-to-get-the-value) section, the values from your data source will be either `string` or `IEnumerable<string>` by default.

If you need something more than this, your custom data source should implement the [`Umbraco.Community.Contentment.DataEditors.IDataSourceValueConverter`](https://github.com/leekelleher/umbraco-contentment/blob/dev/v6.x/src/Umbraco.Community.Contentment/DataEditors/_/DataSources/IDataSourceValueConverter.cs) interface instead of `IContentmentDataSource`. This interface inherits from `IContentmentDataSource`, but also specifies two new methods that your data source will have to implement - that is the `GetValueType` and `ConvertValue` methods.

With this interface, the `TimeZoneDataSource` class from before could now look like:

```csharp
public class TimeZoneDataSource : IDataSourceValueConverter
{
    public string Name => "Time zones";

    public string Description => "Data source for all the time zones.";

    public string Icon => "icon-globe";

    public OverlaySize OverlaySize => OverlaySize.Small;

    public Dictionary<string, object> DefaultValues => default;

    public IEnumerable<ConfigurationField> Fields => default;

    public IEnumerable<DataListItem> GetItems(Dictionary<string, object> config)
    {
        var items = new List<DataListItem>();

        foreach (var timezone in TimeZoneInfo.GetSystemTimeZones())
        {
            items.Add(new DataListItem
            {
                Name = timezone.DisplayName,
                Value = timezone.Id
            });
        }

        return items;
    }

    public Type GetValueType(Dictionary<string, object> config)
    {
        return typeof(TimeZoneInfo);
    }

    public object ConvertValue(Type type, string value)
    {
        return TimeZoneInfo.FindSystemTimeZoneById(value);
    }
}
```

This ensures that you'll get the value as `IEnumerable<TimeZoneInfo>` instead of `IEnumerable<string>`.


#### Extending with your own custom list editor

For creating your own custom list editor, you will need to create a new C# class that implements the [`Umbraco.Community.Contentment.DataEditors.IContentmentListEditor`](https://github.com/leekelleher/umbraco-contentment/blob/dev/v6.x/src/Umbraco.Community.Contentment/DataEditors/_/IContentmentListEditor.cs) interface.

This interface contains two properties, `PropertyEditorUiAlias` and `DefaultConfig` _(optional)_, and one method `HasMultipleValues(config)` returning a boolean value for whether the list editor can select multiple or single values.

The `PropertyEditorUiAlias` property should set the alias of the property-editor manifest. You can develop this to be whatever you want it to be. The only requirement is that the property-editor component will be passed the data source items, (an object array - a serialization of the `DataListItem` model), accessible from the configuration object under the key of `items`.

#### Using dependency injection (IoC/DI)

It is worth noting that both the `IContentmentDataSource` and `IContentmentListEditor` types support [Umbraco's approach for injecting dependencies](https://our.umbraco.com/documentation/reference/using-ioc/#injecting-dependencies). You can use any registered services, factories, helpers by adding them to the class constructor of your custom data-source/list-editor.

For an example, you can see how this is done with the [`UmbracoContentDataListSource` data-source](https://github.com/leekelleher/umbraco-contentment/blob/dev/v6.x/src/Umbraco.Community.Contentment/DataEditors/DataList/DataSources/UmbracoContentDataListSource.cs#L23-L27).


### How to get the value?

The value for the Data List will either be a single value (`string`) or multiple values (`IEnumerable<string>`), _(depending on how the list-editor implements the `HasMultipleValues(config)` method)._

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
    @foreach (var item in Model.Value<IEnumerable<string>>("dataList"))
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

- For the **URL**, enter "https://www.madskristensen.net/posts/files/countries.xml"
- For the **Items XPath**, enter "/countries/country"
- For the **Name XPath**, enter "text()"
- For the **Value XPath**, enter "@code"

For the **List editor**, select **Item Picker**, _maybe configure it with a nice Globe icon in blue?)_

![Country picker example, using Data List property-editor, with remote XML data source and Item Picker editor](data-list--example-02-countries.png)


### Further reading

If you are interesting in real-world usage of Data List, here are some articles for further inspiration...

- [Content Editor defined dropdowns/checkboxlists and radiobuttonlists in Umbraco v8 with Contentment](https://dev.to/timgeyssens/content-editor-defined-dropdowns-checkboxlists-and-radiobuttonlists-in-umbraco-v8-with-contentment-123f) - a post by Tim Geyssens.
- [Umbraco Image Crop Picker using Contentment Data List](https://dev.to/leekelleher/umbraco-image-crop-picker-using-contentment-data-list-5coi) - a post by Lee Kelleher (me).
- [Creating an Author Picker Using Contentment](https://skrift.io/issues/creating-an-author-picker-using-contentment/) - Paul Seal's article for Skrift magazine, June 2021.
