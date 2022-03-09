<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

#### File System

This data-source enables you to enter the URL of a file system data source, using it to populate the items of a compatible editor, e.g. [Data List](../editors/data-list.md).

##### How to configure the editor?

The configuration of the file system data-source has the following options:

![image](https://user-images.githubusercontent.com/85704521/157051866-1b3b4cfb-9a86-4c13-97fa-a96bd62cde0b.png)

The first field is **URL** folder path, here you can enter either a remote URL, or a local relative folder path.

e.g. `https://leekelleher.com/umbraco/contentment/` or `~/umbraco/config/lang/`

The next set of fields to enter Filename filter. e.g. `*.css`.

##### What is the value's object-type?

The value for the file system data-source item is a `string`.
