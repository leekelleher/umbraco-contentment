# Contentment Ideas

## Data Editors

- Configuration Settings
  A property-editor that wraps another editor to add a cog icon in the top-right, which would open an overlay with configuration settings.
  Think Vorto, but for extra options.

- Hide Label
  A property-editor wrapper that can add a "Hide Label" field. Apart from that, everything else is an invisible wrapper.

- Textbox
  Customizable textbox, Placeholder text, Default value, Prefix / suffix (input groups), Char limit, Hide label, Html5 types, Data list

- Remote Content
  Enter a URL into a textbox, the value-converter will request the URL, cache and return its contents as the property value.

- Local Content
  Enable editing of a local file's content, e.g. robots.txt
  Something similar to this, https://our.umbraco.com/packages/website-utilities/umbraco-content-files/, but not as tied to the doctype.

- Elements
  Stacked Content Evo? Take the DocType Editor UI (Group Builder) and apply the concept to Stacked (Inner) Content.
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/components/umb-groups-builder.html
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/common/directives/components/umbgroupsbuilder.directive.js

- Data List - Cards (Grid Selector)
  An alternative view, to display items as cards. Similar to the UI for picking a DocType Template.
  https://github.com/umbraco/Umbraco-CMS/blob/release-8.1.0/src/Umbraco.Web.UI.Client/src/views/documenttypes/views/templates/templates.html#L12
  https://github.com/umbraco/Umbraco-CMS/blob/6bfcc7bb34ce1db607b2d7e39e1ae85fd059a6c0/src/Umbraco.Web.UI.Client/src/views/components/umb-grid-selector.html

- 
