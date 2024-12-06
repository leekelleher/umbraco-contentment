<img src="../docs/assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

> contentment /kənˈtɛntm(ə)nt/ - a state of happiness and satisfaction

[![Mozilla Public License](https://img.shields.io/badge/MPL--2.0-orange?label=license)](https://opensource.org/licenses/MPL-2) [![Latest version](https://img.shields.io/nuget/v/Umbraco.Community.Contentment?label=version)](https://marketplace.umbraco.com/package/umbraco.community.contentment) [![NuGet download count](https://img.shields.io/nuget/dt/Umbraco.Community.Contentment?label=downloads)](https://www.nuget.org/packages/Umbraco.Community.Contentment)

> [!IMPORTANT]
> If you are looking for **Contentment for Umbraco 15** (the new backoffice), [please see the latest progress updates](https://github.com/leekelleher/umbraco-contentment/discussions/357)!

### What is it?

This is a collection of Umbraco components that I have developed for use on my own **Umbraco** implementation projects.

Some components may be innovative, others may be a reimagining of existing components, property-editors, features and other community packages.


#### Why should I use it?

I'm sharing my code with the world. It is your choice (and responsibility) whether you would like to use it.
No pressure or marketing spin from me.

**Please do keep in mind that the Contentment for Umbraco package is not a business, it's a hobby project.**

#### What's on offer?

Let's take a look inside...

##### Property Editors

- [Bytes](../docs/editors/bytes.md) - a read-only label to display file sizes in relative bytes.
- [Code Editor](../docs/editors/code-editor.md) - a code snippet editor, _(using the ACE library that is bundled with Umbraco)._
- [Content Blocks](../docs/editors/content-blocks.md) - a block editor, configurable using element types.
- [Data List](../docs/editors/data-list.md) - an editor that combines a custom data source with a custom list editor.
- [Data Picker](../docs/editors/data-picker.md) - advanced picker editor to query a custom data source.
- [Editor Notes](../docs/editors/editor-notes.md) - Similar to [Notes](../docs/editors/notes.md), with enhanced UI options.
- [Icon Picker](../docs/editors/icon-picker.md) - an editor to select an icon (from the Umbraco icon library).
- [List Items](../docs/editors/list-items.md) - an editor to manage items with a name, value, icon and description.
- [Notes](../docs/editors/notes.md) - a read-only label to display rich-text instructional messages for content editors.
- [Number Input](../docs/editors/number-input.md) - a numeric editor, with sizing configurations.
- [Render Macro](../docs/editors/render-macro.md) - a read-only label dynamically generated from an Umbraco Macro.
- [Social Links](../docs/editors/social-links.md) - an editor to manage links for social network platforms.
- [Templated Label](../docs/editors/templated-label.md) - a display label, ideal for showing data from 3rd-party systems.
- [Textbox List](../docs/editors/textbox-list.md) - a multi-textstring editor, adds a textbox for each item in a custom data source.
- [Text Input](../docs/editors/text-input.md) - a textstring editor, configurable with HTML5 options.

#### [Telemetry](../docs/telemetry.md)

Information about the [telemetry feature](../docs/telemetry.md).

#### [Releases](../releases)

Downloads are available on the [releases page](../releases).

#### Installation

_**Please note...**_

- v5.x supports **Umbraco v13.2.0  ** (on .NET 8.0), it will work on the latest Umbraco v13.x releases.
- v4.x supports **Umbraco v8.17.0** (.NET 4.7.2), **Umbraco v10** (.NET 6.0),  **Umbraco v12** (.NET 7.0) and **Umbraco v13** (.NET 8.0), _it mostly likely works on v9 and v11 too._
- v3.x supports **Umbraco v8.17.0** and **Umbraco v9.0.0**, it will work on latest Umbraco v8.x and v9.x releases.
- v2.x supports **Umbraco v8.14.0**, it will still work on latest Umbraco v8.x releases.
- v1.x supports **Umbraco v8.6.1**, it will still work on latest Umbraco v8.x releases.

With Contentment v5, if you require support for Umbraco v8/v9/v10/v11/v12, please use Contentment v4.

To understand more about which Umbraco CMS versions are actively supported by Umbraco HQ, please see [Umbraco's Long-term Support (LTS) and End-of-Life (EOL) policy](https://umbraco.com/products/knowledge-center/long-term-support-and-end-of-life/).

##### NuGet package repository

To [install from NuGet](https://www.nuget.org/packages/Umbraco.Community.Contentment), you can run the following command from the `dotnet` CLI:

    dotnet add package Umbraco.Community.Contentment

> Please note, that the [`Umbraco.Community.Contentment`](https://www.nuget.org/packages/Umbraco.Community.Contentment) NuGet package is the main package for ongoing releases. If you are referencing one of the older NuGet packages, e.g. [`Our.Umbraco.Community.Contentment`](https://www.nuget.org/packages/Our.Umbraco.Community.Contentment) or [`Our.Umbraco.Community.Contentment.Core`](https://www.nuget.org/packages/Our.Umbraco.Community.Contentment.Core/), then please update your package references to use [`Umbraco.Community.Contentment`](https://www.nuget.org/packages/Umbraco.Community.Contentment).


### [Documentation](../docs/)

Documentation for each of the components - with screenshots, use-cases and code examples - can be found in the `/docs` folder.

Please note, you may find other components within Contentment that have not yet been documented.<br>
_Let's call those "life's little surprises"._


#### Known issues

- **Data List**
  - When using the **Umbraco Content** data source with an XPath query, inside a Nested Content or Block List editors, it will not be able to identify the contextual containing node ID. e.g. your XPath query will not work. [See #30 for details.](https://github.com/leekelleher/umbraco-contentment/issues/30)
  - When using the **Umbraco Content** data source with an XPath query that contains a `$` prefix parameter, the preview will not display the items. [See #120 for details.](https://github.com/leekelleher/umbraco-contentment/issues/120)


### [Roadmap](ROADMAP.md)

If you would like to know what is coming up in future releases, then take a look at the [roadmap](ROADMAP.md).


### Support

I'll try to help the best I can, but I've been doing open source for a long time, and I have suffered my fair share of burnout and empathy fatigue.

I reserve the right to address bug reports or feature requests **in my own time**, or ignore them completely.

**Any feedback is welcome and appreciated.** Please keep in mind, I am not your personal support developer.

If you are really stuck, do remember that the Umbraco community is amongst the friendliest on our planet, learn to embrace it.
Ask for help on the [Our Umbraco support forum](https://our.umbraco.com/), or the [Community Discord Server](https://community.umbraco.com/get-involved/community-discord-server/), I am sure someone can help you there.


### Contributions, collaborations, rules of engagement

If you would like to contribute to this project, please [start a discussion](https://github.com/leekelleher/umbraco-contentment/discussions/new/choose) before spending time and energy on a pull request. Your time is precious.

Please make sure that you read the [CONTRIBUTING](CONTRIBUTING.md) guidelines.

This project is governed by a [Code of Conduct](CODE_OF_CONDUCT.md). **Play nice or go elsewhere.** :v::heart::dove:

If you are unhappy with the project or documentation, please help to identify specific issues and work towards resolving them.
Otherwise you are completely free to not use this software, complain on your favourite social network, or go [scream into the void](https://screamintothevoid.com/).

Unacceptable behaviour towards myself (or contributors) may result in being blocked from accessing this repository.


### License

Copyright &copy; [Lee Kelleher](https://leekelleher.com).

All source code is licensed under the [Mozilla Public License](../LICENSE).

<details>
<summary><strong>A note about licensing</strong></summary>

Historically, I used the [MIT license](https://opensource.org/licenses/MIT) for my open-source projects, regretfully I no longer feel MIT reflects my spirit of encouraging open source collaboration.
The main difference with the [Mozilla Public License](https://opensource.org/licenses/MPL-2.0) is that if you make any modifications to the source code **and** distribute those modifications, then you **must** make those changes publicly available.

_More give, less take._

If you do not to intend to modify the source code, then you will still have the same freedoms as when using the MIT license.

For more information about the **Mozilla Public License**, please visit: <https://www.mozilla.org/en-US/MPL/2.0/FAQ/>

</details>

### Acknowledgements

#### Developers

- [Lee Kelleher](https://leekelleher.com) - ([GitHub](https://github.com/leekelleher), [Mastodon](https://umbracocommunity.social/@lee))

<details>
<summary>Current development effort: <b>~1,840+ hours</b> (between 2019-03-13 to 2024-04-19)</summary>

_To give you an idea of how much human developer time/effort has been put into making and maintaining this package._

</details>


#### Special thanks

- Thank you to [Umbrella](https://web.archive.org/web/*/https://umbrellainc.co.uk/) for facilitating the time and resource to help me initiate this project.
- Kudos to [Gibe](https://gibe.digital/) for enabling the development of the [Data Picker](../docs/editors/data-picker.md) editor.


#### Logo

The package logo uses the [Happy](https://thenounproject.com/term/happy/375493/) (by [Nick Bluth](https://thenounproject.com/nickbluth/)) icon from the [Noun Project](https://thenounproject.com), licensed under [CC BY 3.0 US](https://creativecommons.org/licenses/by/3.0/us/).


### Icons

The [Social Links](../docs/editors/social-links.md) editor makes use of social media icons from the Font Awesome library, (as downloaded SVG files). These are licensed under the [Font Awesome Free License](https://fontawesome.com/license/free) agreement, specifically under the [CC BY 4.0 License](https://creativecommons.org/licenses/by/4.0/).
