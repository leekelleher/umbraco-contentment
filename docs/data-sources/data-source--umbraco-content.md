<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Content

Select a start node to use its children as the data source.


##### How to configure the editor?

The editor give you two options for selecting the start node - by choosing a specific node using a Content Picker:

![Configuration Editor for Umbraco Content](data-source--umbraco-content--configuration-editor-01.png)

\- or by specifying an XPath query that selects the node:

![Configuration Editor for Umbraco Content showing the XPath query field](data-source--umbraco-content--configuration-editor-02.png)

**Note:** If the XPath query returns more than a single node, only the first matching node will be the one that's used as a parent for the data-source.

##### What is the value's object-type?

The value returned from the List editor is an `IPublishedContent`.

Depending on the `List editor` used, this may be wrapped in a `List<IPublishedContent>`.
