<img src="../assets/img/logo.png" alt="Contentment for Umbraco logo" title="A state of Umbraco happiness." height="130" align="right">

## Contentment for Umbraco

**What is a Macro?**

If you are not sure what a Macro is, (within the context of Umbraco), please read the documentation on the Umbraco community website.

- [Our Umbraco - Documentation - Templating - Macros](https://our.umbraco.com/documentation/reference/templating/macros/)


### Render Macro

Umbraco Macros are typically used for functionality on frontend of a website. But what if we could render them within the CMS back-office?

Render Macro is a read-only property-editor to display the HTML output from a Macro template.

> This property-editor has taken inspiration from the community package, [uComponents RenderMacro](http://ucomponents.github.io/data-types/render-macro/) by me, Lee Kelleher.

Similar to the [Notes](notes.md) property-editor, Render Macro can be used to display rich-text instructional messages for content editors.


### How to configure the editor?

In your new Data Type, selected the "[Contentment] Render Macro" option. You will see the following configuration fields.

![Configuration Editor for Render Macro](render-macro--configuration-editor.png)

The main field is "**Macro**", here you can select and configure the Macro to be rendered.

There are also options to **hide the label** on the property itself, and to **hide the property group container**. Selecting these option will enable the rendered macro html to be displayed in full width and outside of the content property panel.


### How to use the editor?

Once you have added the configured Data Type to your Document Type, the rendered macro will be displayed on the content page's property panel.


### How to get the value?

This property-editor is read-only and does not store any value against the property.


### Advanced usage

Render Macro offers a selection of special aliases that can be use to access meta-data from the property itself.

By adding any of the following aliases to your Macro parameters, they will be populated...

- `"__propertyAlias"` will be populated with the property's alias.
- `"__propertyLabel"` will be populated with the property's label (name).
- `"__propertyCulture"` will be populated with the currently selected culture.
- `"__propertyDataTypeKey"` will be populated with the Data Type's GUID value.
- `"__propertyDescription"` will be populated with the property's description.
- `"__propertyHideLabel"` will be the flag of whether the property's label is hidden or not.


### Suggestions and ideas

Inspiration for how a Macro could be used within the CMS back-office...

- Contextual information about the specific property. Meaning a single Macro could be used for multiple instructions.
- Pull in content from an external source, e.g. analytics from the published page's URL.
- Display content or media referenced by other published properties.
- Display a dynamic list of content nodes, with "Open" and "Preview" links, like a Content Picker


### Some example HTML

You can make your macro blend in to the back-office panel just like a standard property:
```

@inherits Umbraco.Cms.Web.Common.Macros.PartialViewMacroPage
@{
	var propertyLabel = Model.MacroParameters["__propertyLabel"] != null ? Model.MacroParameters["__propertyLabel"].ToString() : "Items";
	var propertyDescription = Model.MacroParameters["__propertyDescription"] != null ? Model.MacroParameters["__propertyDescription"].ToString() : "Some info here...";

	IEnumerable<BlogPost> filteredPosts = Umbraco.AssignedContentItem.Children<BlogPost>().Where([Some filtering criteria here]);

	<style>
		.macro-actions {
			flex-basis: 100px;
		}
		/* Large Full-screen*/
		@@media (min-width:1930px) {
			.macro-node {
				width: 600px;
			}
		}


		/* Medium Full-screen*/
		@@media (min-width:1300px) and (max-width:1929px) {
			.macro-node {
				width: 500px;
			}
		}

		/* Small Full-screen*/
		@@media (min-width:1100px) and (max-width:1299px) {
			.macro-node {
				width: 300px;
			}
		}

		/* Tablet Screen*/
		@@media (min-width:768px) and (max-width:1099px) {
			.macro-node {
				width: 450px;
			}
		}

		/* Phone Screen*/
		@@media (min-width:501px) and (max-width:767px) {
			.macro-node {
				width: 300px;
			}
		}

		/* Phone Screen - Smallest*/
		@@media (max-width:500px) {
			.macro-node {
				width: 200px;
			}
		}
	</style>

	<div class="macro-panel">

		<div class="control-group umb-control-group">
			<div class="umb-el-wrap">
				<div class="control-header">
					<label class="control-label">@propertyLabel (@filteredPosts.Count())</label>
					<small class="control-description property-is-calc">@propertyDescription</small>
				</div>

				<div class="controls">
					<div class="umb-property-editor">
						@if (filteredPosts.Any())
						{
							foreach (var post in filteredPosts.OrderByDescending(n => n.CreateDate))
							{
								var icon = "icon-book-alt-2 color-green"; //Look at 'Umbraco.Web.PublishedContentTypeExtensions.GetIcon()' to pull the icon dynamically from the ContentType

								<div class="flex">
									<span aria-hidden="true" class="umb-node-preview__icon umb-icon" icon="@icon">
										<span class="umb-icon__inner">
											<i class="@icon"></i>
										</span>
									</span>
									<div class="umb-node-preview__content macro-node">
										<div class="umb-node-preview__name">@post.Name</div>
										<div class="umb-node-preview__description">@post.Url()</div>
									</div>

									<div class="umb-node-preview__actions macro-actions">
										<button type="button" class="umb-node-preview__action">
											<a href="/umbraco/#/content/content/edit/@post.Id" target="_blank">
												<localize key="general_open">Open</localize>
											</a>
											<span class="sr-only ng-binding">&nbsp;@post.Name...</span>
										</button>
										<button type="button" class="umb-node-preview__action">
											<a href="@post.Url()" target="_blank">
												<localize key="general_preview">Preview</localize>
											</a>
											<span class="sr-only ng-binding">&nbsp;@post.Name</span>
										</button>
									</div>
								</div>
							}
						}
						else
						{
							<i>None</i>
						}
					</div>
				</div>
			</div>
		</div>
	</div>
}

```
