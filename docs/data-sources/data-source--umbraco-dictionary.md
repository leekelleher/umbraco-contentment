<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Dictionary Item

This data-source lets you use the child dictionary items from a selected parent dictionary item to populate the options of a compatible editor, e.g. [Data List](../editors/data-list.md).


##### How to configure the editor?

The configuration of the Umbraco dictionary item data-source has the following options:

![Configuration Editor for Umbraco dictionary item](data-source--umbraco-dictionary--configuration-editor-01.png)

The **Dictionary item** field will let you select the parent dictionary item to display the child items from.


##### What is the value's object-type?

The value for the Umbraco dictionary data-source item is a `string` of the dictionary item's key (alias).
Depending on the `List editor` used, this may be wrapped in a `List<string>`.

