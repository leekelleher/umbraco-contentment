<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Content Types

Populate the data source using Content Types.


##### How to configure the editor?

Select the content types that should be available in the data source. You can select between Document Types with or without templates, or Element Types.

![Configuration Editor for Umbraco Content Types](data-source--umbraco-content-types.png)

##### What is the value's object-type?

The value returned from the List editor is an `IPublishedContentType`.

Depending on the `List editor` used, this may be wrapped in a `List<IPublishedContentType>`.
