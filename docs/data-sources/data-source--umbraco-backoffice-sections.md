<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Backoffice Sections

This data-source will use the available backoffice sections, e.g. Content, Media, etc, to populate the items of a compatible editor, e.g. [Data List](../editors/data-list.md).


##### How to configure the editor?

In your new Data Type, selected the `[Contentment] Data List` option and then the `Umbraco Backoffice Sections` Data source.

There are no configuration fields for this data source.

Please see the [Data List editor page](../editors/data-list.md) for more information on the List editor options.


##### What is the value's object-type?

The value of the editor will be the section alias, as a `string`, which is typically a lowercase version of the section name. Depending on the `List editor` used, this may be wrapped in a `List<string>`.
