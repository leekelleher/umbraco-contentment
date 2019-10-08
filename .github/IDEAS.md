# Contentment Ideas

## Post-BackOffice Installation

- Add the current (logged on) user to a mailing list. Umbraco HQ do this for Umbraco Forms installation.


## Data Editors

- Local Content
  Enable editing of a local file's content, e.g. robots.txt
  Something similar to this, https://our.umbraco.com/packages/website-utilities/umbraco-content-files/, but not as tied to the doctype.


- Template editor (type of thing?) Add columns (blocks). Each one could be an InnerContent-esque editor?
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/grid/dialogs/rowconfig.html#L44-L64

- Color Picker (+ multiple picker)
  Basically a property-editor of the prevalue-editor. Using the Spectrum color picker library.
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/colorpicker/colorpicker.prevalues.html
  or could look to put <umb-color-swatches> in an overlay?
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/components/umb-color-swatches.html
  e.g. https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/common/infiniteeditors/iconpicker/iconpicker.html#L35-L41
  then have a "Multiple Approved Color Picker"?

- Checkbox List - arrange in columns? (e.g. think categories / taxonomy)

- editorService Ideas
  - `embed` - OEmbed Editor - Dave W has one, but once you add the OEmbed, you can't edit it. This may be a limitation of Umbraco's `editorService.embed` feature, but feels like it can be improved.
    https://our.umbraco.com/packages/backoffice-extensions/oembed-picker-property-editor/
  - `linkPicker` - MultiUrlPicker leverages Umbraco's `editorService.linkPicker` feature, but the UI for the overlay is too much! Needs drastically simplifying.
  - `queryBuilder` - could this ever be useful on a content item? Same goes for `editorService.insertCodeSnippet` or `editorService.insertField`?
  - `treePicker` - have a generic picker? where the "section" and "treeAlias", or "entityType" can be defined in the Data Type?
    - File picker, Stylesheet picker, Scripts picker, Member Group picker, Relation Type picker, Languages picker, 
  - `userGroupPicker` - I guess a User Group Picker could be useful as a property-editor?

- Tabbed Textarea
  Like what you see on code examples, (e.g. MDN), with a HTML, CSS, JS tabs. Couple be useful, maybe?

- Data List - CSV Data Source

- 
