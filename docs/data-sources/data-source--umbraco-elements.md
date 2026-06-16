<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Elements

Select an element folder to use its elements as the data source.


##### How to configure the data-source?

Select an element folder from the Element Library using the folder picker. Leave the folder field empty to use all elements at the root of the Element Library.

Optionally, filter by one or more element types using the element type filter, and set a custom property alias for the thumbnail image used in Cards display mode.


##### What is the value's object-type?

The value returned from the List editor is an `IPublishedElement`.

Depending on the `List editor` used, this may be wrapped in a `List<IPublishedElement>`.

When accessed via the Delivery API, the value is returned as an `IApiElement`.
