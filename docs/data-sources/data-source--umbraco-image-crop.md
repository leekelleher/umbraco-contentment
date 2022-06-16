<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Data Sources

#### Umbraco Image Crops

This data-source lets you to use crops defined in an Umbraco Image Cropper data-type to populate the items of a compatible editor, e.g. [Data List](../editors/data-list.md).


##### How to configure the editor?

The configuration of the Umbraco image crops data-source has the following options:

![Configuration Editor for Umbraco image crops](data-source--umbraco-image-crop--configuration-editor-01.png)

The **Image Cropper** field will let you select an existing Umbraco Image Cropper data-type select the defined crops from.


##### What is the value's object-type?

The value for the Umbraco image crops item is a `string` of the crop's alias.
Depending on the `List editor` used, this may be wrapped in a `List<string>`.
