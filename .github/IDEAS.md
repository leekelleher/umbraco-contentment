# Contentment Ideas

## Marketing focus

!! Contentment editor experience
!! Really drive home the idea of editor experience in Contentment.


## BackOffice Features

- NuCache viewer dashboard - similar to the Examine dashboard
- Categories - Full category management


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

- Checkbox List - arrange in columns? (e.g. think categories / taxonomy) Try using the `columns: 3;` CSS rule?

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

- Data List - Tags-esque List Editor - so not "real" tags, but a same interaction/experience.
- Data List - Button Group (needs rename), e.g. https://twitter.com/leekelleher/status/1031862617084710912

- TinyMCE - push the boundaries beyond Umbraco's defaults?
  - How's about a TinyMCE that is 30px height, Distraction Free mode, all menu items disabled.
  - Only keyboard shortcuts would be enabled. Try to make it become a single-line rich-textbox.

- Text Input - Change Font-Size within Input Field Based on Length (similar to Facebook's post experience, after 85 chars)
  - https://web-design-weekly.com/snippets/change-font-size-within-input-field-based-on-length/

- 
