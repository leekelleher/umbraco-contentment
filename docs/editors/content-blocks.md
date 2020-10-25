<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Content Blocks

Content Blocks is a property-editor used for creating a list of structured content, with each block configurable using an element type.

> If you are using Umbraco 8.7 (or above), this may sound familiar to the [Block List Editor](https://our.umbraco.com/Documentation/Getting-Started/Backoffice/Property-Editors/Built-in-Property-Editors/Block-List-Editor/), and you may be questioning why you should use Content Blocks over the built-in Block List Editor? It's a good question, but you'll find no comparisons or marketing spin from me, I'd recommend that you stick with Umbraco's built-in editors. Content Blocks has subtle differences, it's entirely your choice.

> For long time fans of Umbraco v7.x, if you recall the [Stacked Content](https://our.umbraco.com/packages/backoffice-extensions/stacked-content) editor, then Content Blocks could be considered its spiritual successor.


### How to configure the editor?

> Select the data type, here are the options
> 
> Display mode - Stack or List (extensible)
> 
> Block types - configure element types
>     - element type
>     - name template
>     - overlay size
>     - enable preview
> 
> Enable filter - in overlay
> 
> Max items
> 
> Disable sorting
> 
> Enable dev mode


### How to use the editor?

> Press the Add button
> 
> Select block from overlay
> 
> Edit in panel.
> 
> Options - copy block; create content template


#### Previews

> Razor views, found within the /Views/Partials/Blocks/ folder
> 
> Mention the view-model + view-data properties (icon, index/position)
> 
> NOTE: Preview doesn't work on a new unsaved page. As it has no page context.


### How to get the value?


> `IEnumerable<IPublishedElement>`
> If using ModelsBuilder, it'll be castable to the desired element type model.


### Further reading

> Alternative block editor options (for Umbraco v8)
> Perplex Content Blocks
> Bento editor
