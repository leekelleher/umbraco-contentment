<img src="../docs/assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

# Umbraco Contentment - Roadmap

Here is a _provisional_ roadmap for the Contentment for Umbraco package, to be actioned at my own pleasure.

> To note: I try to follow the [Semantic Versioning](https://semver.org/) standard.<br>
> TL;DR, `[Major].[Minor].[Patch]`, Major = breaking-change; Minor  = feature; Patch = bug-fix.


## v1

### [v1.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/1.0.0)

Initial release.

Compiles against Umbraco CMS **v8.6.1**, _(this was the latest version at the time of initial release)._

Property Editors are:

- [Bytes](../docs/editors/bytes.md)
- [Data List](../docs/editors/data-list.md)
- [Icon Picker](../docs/editors/icon-picker.md)
- [Notes](../docs/editors/notes.md)
- [Render Macro](../docs/editors/render-macro.md)

### [v1.1](https://github.com/leekelleher/umbraco-contentment/releases/tag/1.1.0)

- [Content Blocks](../docs/editors/content-blocks.md)
- Data List: Umbraco Content _(a data source for selecting nodes)_

### [v1.2](https://github.com/leekelleher/umbraco-contentment/releases/tag/1.2.0)

- [Text Input](../docs/editors/text-input.md) _(a reimagining of the textstring editor)_
- [Number Input](../docs/editors/number-input.md) _(a reimagining of the numeric editor)_
- Data List: Templated List _(use custom AngularJS markup to render items)_
- Telemetry

### [v1.3](https://github.com/leekelleher/umbraco-contentment/releases/tag/1.4.0)

- [Code Editor](../docs/editors/code-editor.md) _(using ACE bundled with Umbraco)_
- Data List: Preview _(a real time preview of the configured Data Source and List Editor)_
- Data List: Buttons _(list editor, similar to what folk see used in Umbraco Uno)_
- Data List: Tags _(list editor, visually similar to Umbraco Tags editor)_

### [v1.4](https://github.com/leekelleher/umbraco-contentment/releases/tag/1.4.0)

- _A bunch of (hidden) extra data-sources._ 🤫


## v2

### [v2.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/2.0.0)

- A breaking-change release _(in terms of following [SemVer guidelines](https://semver.org/)),_ of the latest v1.4.x features that compiles against Umbraco CMS **v8.14.0**, _(the most recent version of Umbraco at the time of release)._

### [v2.1](https://github.com/leekelleher/umbraco-contentment/releases/tag/2.1.0)

- Configuration Editor item grouping
- Data List: Unlocking all (hidden 🤫) [data-sources](../docs/data-sources/README.md)

### [v2.2](https://github.com/leekelleher/umbraco-contentment/releases/tag/2.2.0)

- [Templated Label](https://github.com/leekelleher/umbraco-contentment/discussions/100)


## v3

### [v3.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/3.0.0)

- A breaking-change release _(following SemVer guidelines),_ of v2.x features that will compile against both Umbraco **v8.17.0** and **v9.0.0**. If you're interested in the development of this release, please [see my developer's journal.](https://github.com/leekelleher/umbraco-contentment/discussions/105)

### [v3.1](https://github.com/leekelleher/umbraco-contentment/releases/tag/3.1.0)

- [Content Blocks](../docs/editors/content-blocks.md) ["Cards" display mode](https://github.com/leekelleher/umbraco-contentment/pull/194)
- [Textbox List](https://github.com/leekelleher/umbraco-contentment/pull/195)

### [v3.2](https://github.com/leekelleher/umbraco-contentment/releases/tag/3.2.0)

- [Editor Notes](https://github.com/leekelleher/umbraco-contentment/discussions/187)

### [v3.3](https://github.com/leekelleher/umbraco-contentment/releases/tag/3.3.0)

- [Content Blocks](../docs/editors/content-blocks.md) previews on "Blocks" display mode
- [Text Input](../docs/editors/text-input.md): adds prepend/append icon options

### [v3.4](https://github.com/leekelleher/umbraco-contentment/releases/tag/3.4.0)

- Deploy Connectors for Notes, Editor Notes and Render Macro
- Enhancement to Configuration Editor, added `IContentmentListTemplateItem` interface
- Data List: New data-sources for Umbraco Files and Umbraco Templates


## v4

### [v4.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.0.0)

- A breaking-change release _(following SemVer guidelines),_ of v3.x features that will compile against Umbraco **v8.17.0**, **v9.5.0** and **v10.0.0**.

### [v4.1](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.1.0)

- [Social Links](https://github.com/leekelleher/umbraco-contentment/pull/234)

### [v4.2](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.2.0)

- Adds `IContentmentContentContext` for attempting to get the current node ID within Data List data-sources.

### [v4.3](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.3.0)

- A non-breaking-change minor release. Adds support for Umbraco v11, .NET 7.0. _(Whilst still supporting v8.17+, v9.5+ and v10.0+)._

### [v4.4](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.4.0)

- Data List: [Umbraco Content Property Value](https://github.com/leekelleher/umbraco-contentment/pull/287) data-source.
- Data List Item editor _(the one that is used in the User-defined data-source)._

### [v4.5](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.5.0)

- [Data Picker](https://github.com/leekelleher/umbraco-contentment/pull/297)
- Data List: [Umbraco Backoffice Sections](https://github.com/leekelleher/umbraco-contentment/commit/c8a2f0f8552b2ad3a778782f13c45cbb18aff88c) data-source.

### [v4.6](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.6.0)

- A non-breaking-change minor release. Adds support for Umbraco v13, .NET 8.0. _(Whilst still supporting v8.17+, v9, v10, v11 and v12)._

### [v4.7](https://github.com/leekelleher/umbraco-contentment/releases/tag/4.7.0)

- Data Picker: [Data List source bridge](https://github.com/leekelleher/umbraco-contentment/pull/393) _(to enable all the built-in Data List sources within Data Picker)._


## v5

### [v5.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/5.0.0)

- Removes support for previous Umbraco versions: Umbraco 8 (.NET Framework 4.7.2), Umbraco 9 (.NET 5), Umbraco 10 (.NET 6) and Umbraco 11 and 12 (.NET 7).
- Supports Umbraco 13 (.NET 8) only.
- Converts package to be a Razor Class Library, _(e.g. web assets are served from NuGet package)._


## v6

### [v6.0](https://github.com/leekelleher/umbraco-contentment/releases/tag/6.0.0)

- [Support for the new Umbraco backoffice, aka Bellissima](https://github.com/leekelleher/umbraco-contentment/discussions/357).
- ~Adds support for Umbraco 14 (.NET 8) _aka Bellissima_~.
- ~Adds support for Umbraco 15 (.NET 9)~.
- Adds support for Umbraco 16 (.NET 9) and Umbraco 17 (.NET 10).
- Removes support for Umbraco 13 (.NET 8).


## v7

- _Adds support for Umbraco 17 and 18 (.NET 10)._
- _Removes support for Umbraco 16 (.NET 9)._


## Future feature (minor) releases

_Who knows?!_ `¯\_(ツ)_/¯`

...or [suggest your own idea?](https://github.com/leekelleher/umbraco-contentment/discussions/new?category=ideas)
