<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources


#### Examine Query

Populate the data source from an Examine query.


##### How to configure the editor?

First select the Examine index containing the values you need in your Data Source. Then you need to enter the raw Lucene expression to query Examine with. To make the query contextual using the content's page UDI, you can use C# standard `string.Format` syntax, e.g. `+propertyAlias:"{0}"`

Enter the field name to select the name, and the field name to select the value (key) from the Examine record.

Optionally, you can enter the field name to select the icon, and the field name to select the description from the Examine record.

![Configuration Editor for Examine Query](data-source--examine.png)


##### What is the value's object-type?

The value returned from the List editor is an `string` containing the value of the field specified in the configuration.

Depending on the `List editor` used, this may be wrapped in a `List<string>`.