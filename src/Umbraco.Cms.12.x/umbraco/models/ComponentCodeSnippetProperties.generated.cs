//------------------------------------------------------------------------------
// <auto-generated>
//   This code was generated by a tool.
//
//    Umbraco.ModelsBuilder.Embedded v11.0.0-rc5+dc1b10f
//
//   Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace Umbraco.Cms.Web.Common.PublishedModels
{
	// Mixin Content Type with alias "componentCodeSnippetProperties"
	/// <summary>[Component] Code Snippet Properties</summary>
	public partial interface IComponentCodeSnippetProperties : IPublishedElement
	{
		/// <summary>Code</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string Code { get; }

		/// <summary>Description</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		string Description { get; }
	}

	/// <summary>[Component] Code Snippet Properties</summary>
	[PublishedModel("componentCodeSnippetProperties")]
	public partial class ComponentCodeSnippetProperties : PublishedElementModel, IComponentCodeSnippetProperties
	{
		// helpers
#pragma warning disable 0109 // new is redundant
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		public new const string ModelTypeAlias = "componentCodeSnippetProperties";
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		public new const PublishedItemType ModelItemType = PublishedItemType.Content;
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public new static IPublishedContentType GetModelContentType(IPublishedSnapshotAccessor publishedSnapshotAccessor)
			=> PublishedModelUtility.GetModelContentType(publishedSnapshotAccessor, ModelItemType, ModelTypeAlias);
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static IPublishedPropertyType GetModelPropertyType<TValue>(IPublishedSnapshotAccessor publishedSnapshotAccessor, Expression<Func<ComponentCodeSnippetProperties, TValue>> selector)
			=> PublishedModelUtility.GetModelPropertyType(GetModelContentType(publishedSnapshotAccessor), selector);
#pragma warning restore 0109

		private IPublishedValueFallback _publishedValueFallback;

		// ctor
		public ComponentCodeSnippetProperties(IPublishedElement content, IPublishedValueFallback publishedValueFallback)
			: base(content, publishedValueFallback)
		{
			_publishedValueFallback = publishedValueFallback;
		}

		// properties

		///<summary>
		/// Code: Paste in your code snippet.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("code")]
		public virtual string Code => GetCode(this, _publishedValueFallback);

		/// <summary>Static getter for Code</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetCode(IComponentCodeSnippetProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "code");

		///<summary>
		/// Description: For display purposes, enter a description of this code snippet.
		///</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[global::System.Diagnostics.CodeAnalysis.MaybeNull]
		[ImplementPropertyType("description")]
		public virtual string Description => GetDescription(this, _publishedValueFallback);

		/// <summary>Static getter for Description</summary>
		[global::System.CodeDom.Compiler.GeneratedCodeAttribute("Umbraco.ModelsBuilder.Embedded", "11.0.0-rc5+dc1b10f")]
		[return: global::System.Diagnostics.CodeAnalysis.MaybeNull]
		public static string GetDescription(IComponentCodeSnippetProperties that, IPublishedValueFallback publishedValueFallback) => that.Value<string>(publishedValueFallback, "description");
	}
}
