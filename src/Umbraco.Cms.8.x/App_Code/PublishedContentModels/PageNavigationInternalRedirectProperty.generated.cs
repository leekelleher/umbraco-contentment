//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v8.17.0
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.ModelsBuilder.Embedded;

namespace Umbraco.Web.PublishedModels
{
	/// <summary>[Page] Navigation Internal Redirect Property</summary>
	[PublishedModel("pageNavigationInternalRedirectProperty")]
	public partial class PageNavigationInternalRedirectProperty : PublishedElementModel
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.17.0")]
		public new const string ModelTypeAlias = "pageNavigationInternalRedirectProperty";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.17.0")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.17.0")]
		public new static IPublishedContentType GetModelContentType()
			=> PublishedModelUtility.GetModelContentType(ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.17.0")]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(Expression<Func<PageNavigationInternalRedirectProperty, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(), selector);
#pragma warning restore 0109

		// ctor
		public PageNavigationInternalRedirectProperty(IPublishedElement content)
			: base(content)
		{ }

		// properties

		///<summary>
		/// Redirect to page: To redirect this page an internal page, please select the content page.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "8.17.0")]
		[ImplementPropertyType("umbracoRedirect")]
		public virtual global::Umbraco.Core.Models.PublishedContent.IPublishedContent UmbracoRedirect => this.Value<global::Umbraco.Core.Models.PublishedContent.IPublishedContent>("umbracoRedirect");
	}
}
