<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

### Code Editor

Code Editor is a property-editor that is used to enter code snippets (as content), makes use of [AWS Cloud 9's Ace editor](https://ace.c9.io/) library that is distributed with Umbraco.

This is useful if you would like to embed a snippet of HTML or JavaScript on a content page, or have a full HTML property-editor.


### How to configure the editor?

In your new Data Type, selected the "[Contentment] Code Editor" option. You will see the following configuration fields.

![Configuration Editor for Code Editor](code-editor--configuration-editor.png)

The first field is **Language mode**, this is used to select the programming language mode, for code syntax highlighting. The default mode is "Razor", meaning that you can use a combination of HTML, CSS, JavaScript and Razor syntax.

The next field is **Theme**, which is used to set the visual appearance of the code editor. The default _(and only)_ theme is "Chrome".

> **Please note,** by default, Umbraco ships a streamlined set of programming language modes and themes.
> 
> If you would like to add more modes and themes, you can do this by [downloading the latest pre-packaged version of the Ace editor](https://github.com/ajaxorg/ace-builds/releases) and copy any of the `mode-*` or `theme-*` files from the `src-min-noconflict` folder over to the `~/umbraco/lib/ace-builds/src-min-noconflict/` folder in you Umbraco installation.
> 
> Once you've done this, you can reload the Data Type screen, and the new programming language modes and themes will appear in the dropdown options for the fields above.


The **Font size** field is used to set the font size, the value must be a [valid CSS font-size value](https://developer.mozilla.org/en-US/docs/Web/CSS/font-size), e.g. `14px`, `80%`, `0.8em`, etc. The default size is "`small`".

The **Word wrapping** option can enable the code editor to wrap the text around to the following line.

The next two fields, **Minimum lines** and **Maximum lines**, are used to set the default height size of the code editor. If you would like the height to auto-scale forever, then set the maximum number to something ridiculously high. If left empty, the height of the code editor will remain at a fixed height and not auto-scale.


### How to use the editor?

Once you have added the configured Data Type to your Document Type, the Code Editor editor will be displayed on the content page's property panel.

![Code Editor property-editor](code-editor--property-editor-01.png)


### How to get the value?

The value for the Code Editor is a `string`.

Programmatically, you would access the value exactly the same as Umbraco's Textarea editor, [see Umbraco's documentation for code snippet examples](https://our.umbraco.com/Documentation/Getting-Started/Backoffice/Property-Editors/Built-in-Property-Editors/Textarea/#mvc-view-example).

If you are wanting to display the code content as a pre-formatted code snippet, I would recommend using the `<pre>` and `<code>` tags.

Using Umbraco's Models Builder...

```cshtml
<pre><code>@Html.Raw(Model.CodeEditor)</code></pre>
```

Without ModelsBuilder...

Weakly-typed...

```cshtml
<pre><code>@Html.Raw(Model.Value("codeEditor"))</code></pre>
```

Strongly-typed...

```cshtml
<pre><code>@Html.Raw(Model.Value<string>("codeEditor"))</code></pre>
```

For code syntax highlighting, the following JavaScript libraries are quite popular:

- [Prism.js](https://prismjs.com/)
- [highlight.js](https://highlightjs.org/)
