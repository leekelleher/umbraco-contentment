<img src="../assets/img/logo.png" alt="Umbraco Contentment Logo" title="A shoebox of Umbraco happiness." height="130" align="right">

## Umbraco Contentment

### Data List

[A single paragraph introduction for the data editor.]


### How to configure the editor

[A few sentences about the configuration editor + screenshots]


### How to use the editor

[A few sentences about how to use the editor itself + screenshots]


### How to get the value

[A few sentences about how to get the value + value converter / models builder info]

[Include a code sample.]


### Use cases

[Add a couple of use cases for this editor.]

- Umbraco Entity Picker
- Use the [`System.DayOfWeek`](https://docs.microsoft.com/en-us/dotnet/api/system.dayofweek) enum with a checkbox list to display the days of the week.
  This could be used to specify business opening days.
- Country Picker - use XML datasource, show example using `https://madskristensen.net/posts/files/countries.xml`
  XPath: `/countries/country`, with value = `@code` and name = `text()`
- Google Webfonts picker? (using JSON remote datasource)
  e.g. `https://www.googleapis.com/webfonts/v1/webfonts?key=YOUR-API-KEY`
- Show an extreme example of using a custom data-source and custom view, to create something like this: https://our.umbraco.com/packages/backoffice-extensions/ucssclassnamedropdown/

### How to configure as a Parameter Editor

[Add a couple of sentences on how to configure this editor as a Parameter Editor.]

[Include a code snippet.]
