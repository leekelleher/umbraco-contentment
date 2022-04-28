<img src="../docs/assets/img/logo.png" alt="Contentment for Umbraco Logo" title="A state of Umbraco happiness." height="130" align="right">

# Ideas for Contentment


## Data Editors

- TinyMCE - push the boundaries beyond Umbraco's defaults?
  - How's about a TinyMCE that is 30px height, Distraction Free mode, all menu items disabled.
  - Only keyboard shortcuts would be enabled. Try to make it become a single-line rich-textbox.

- [Data List: Groups](https://github.com/leekelleher/umbraco-contentment/discussions/90)

- Text Input - Change Font-Size within Input Field Based on Length (similar to Facebook's post experience, after 85 chars)
  - https://web-design-weekly.com/snippets/change-font-size-within-input-field-based-on-length/

- Nested Content - Reimagined. Reach for its heart, imagine a new landscape, another atmosphere, and see how it goes.

- Cascade - conceptually similar to (internal) Cascading Dropdown List, but compatible with any single-value editor(s).

- Wizard - overlay panel with a step wizard, each containing multiple fields, that populate the next set of fields. (Similar to the cascading editor, above.)
  Making use of the [`<umb-pagination>` directive](https://github.com/umbraco/Umbraco-CMS/search?q=general_previous)?


### Internally used editors

Could these internally used editors have potential as standalone property editors?

- Cascading Dropdown List
- Configuration Editor
- Data List Item editor
- Data Table
- Macro Picker


### Validation

None of the editors have explicit validation implemented, (only because I've never researched it). Let's explore the validation approaches for the editors.


## Localization

Remove hard-coded English labels, move them to the language XML file.


## Tracking

- v8.6+ `IDataValueReferenceFactory`, `IDataValueReference`


## Support for Umbraco Deploy?

- Implement artifact dependency logic for `IValueConnector`, `IDataTypeConfigurationConnector`? _(to support Umbraco Deploy)_


## Deprecation considerations

It's good to re-evaluate whether certain features are still relevant. Review the [telemetry stats](https://leekelleher.com/umbraco/contentment/telemetry/) for usage data.

