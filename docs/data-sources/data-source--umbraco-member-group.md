<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Member Groups

This data-source enables you to use Umbraco member groups to populate the items of a compatible editor, e.g. [Data List](../editors/data-list.md).


##### How to configure the editor?

The Umbraco member groups data-source does not have any configuration options. It will list all the available member groups.


##### What is the value's object-type?

The value for the Umbraco member groups data-source item will be a `Guid` (of the member group's ID).
Depending on the `List editor` used, this may be wrapped in a `List<Guid>`.

With the `Guid` value you can use the `IMemberGroupService` to query the group data from the database.

