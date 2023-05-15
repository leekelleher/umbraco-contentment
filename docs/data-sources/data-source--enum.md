<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### .NET Enumeration

Select an enumeration from a .NET assembly as the data source.


##### How to configure the editor?

Select the desired enumeration, by selecting the containing .NET assembly, and then the enumeration type. You can opt to sort the values alphabetically in the data source too.

![Configuration Editor for .NET Enumeration](data-source--enum.png)



##### What is the value's object-type?

The value returned from the List editor is the configured enumeration type.

Depending on the `List editor` used, this may be wrapped in a `List<T>`.

##### Controlling Enum display in Contentment

Here's the proofread version of the text:

You can use the [DataListItemAttribute](../../src/Umbraco.Community.Contentment/DataEditors/DataList/DataListItemAttribute.cs) to decorate your enum values with additional information for use with Contentment. With this attribute, you can specify properties such as name, description, icon, etc., for individual enum items.

For example:

```csharp
using Umbraco.Community.Contentment.DataEditors;

public enum MyEnum
{
    [DataListItem(Description = "This is the first value", Group = "Group A", Icon = "icon-first")]
    First,

    [DataListItem(Description = "This is the second value", Group = "Group B", Icon = "icon-second")]
    Second,

    [DataListItem(Description = "This is the third value", Group = "Group A", Icon = "icon-third")]
    Third
}
```

The `DataListItemAttribute` contains the following properties:

| Property    | Description                                                      | Type    |
|-------------|------------------------------------------------------------------|---------|
| Description | Provides a description for the Enum value                         | string  |
| Disabled    | Specifies whether the Enum value should be disabled               | bool    |
| Group       | Assigns the Enum value to a specific group                        | string  |
| Icon        | Specifies an icon for the Enum value                              | string  |
| Ignore      | Indicates whether the Enum value should be ignored                | bool    |
| Name        | Provides a custom name for the Enum value (default: field name)   | string  |
| Value       | Specifies a custom value for the Enum value (default: field value)| string  |

The provided text has been proofread and revised for clarity and accuracy.
