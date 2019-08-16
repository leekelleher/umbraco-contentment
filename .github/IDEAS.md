# Contentment Ideas

## Data Editors

- Configuration Settings
  A property-editor that wraps another editor to add a cog icon in the top-right, which would open an overlay with configuration settings.
  Think Vorto, but for extra options.

- Hide Label
  A property-editor wrapper that can add a "Hide Label" field. Apart from that, everything else is an invisible wrapper.

- Textbox
  Customizable textbox, Placeholder text, Default value, Prefix / suffix (input groups), Char limit, Hide label, Html5 types, Data list

- Local Content
  Enable editing of a local file's content, e.g. robots.txt
  Something similar to this, https://our.umbraco.com/packages/website-utilities/umbraco-content-files/, but not as tied to the doctype.

- Elements
  Stacked Content Evo? Take the DocType Editor UI (Group Builder) and apply the concept to Stacked (Inner) Content.
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/components/umb-groups-builder.html
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/common/directives/components/umbgroupsbuilder.directive.js
  For a toolbar menu, maybe add options using the action overlay UI?
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/components/umb-confirm-action.html

- Dimensions - Width x Height combo
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/rte/rte.prevalues.html#L29-L34

- Smaller number input (+ label suffix)
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/rte/rte.prevalues.html#L36-L40

- Template editor (type of thing?) Add columns (blocks). Each one could be an InnerContent-esque editor?
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/grid/dialogs/rowconfig.html#L44-L64

- Color Picker (+ multiple picker)
  Basically a property-editor of the prevalue-editor. Using the Spectrum color picker library.
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/propertyeditors/colorpicker/colorpicker.prevalues.html
  or could look to put <umb-color-swatches> in an overlay?
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/components/umb-color-swatches.html
  e.g. https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/common/infiniteeditors/iconpicker/iconpicker.html#L35-L41
  then have a "Multiple Approved Color Picker"?

- 
