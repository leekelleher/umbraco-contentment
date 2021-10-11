/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System;
using System.Linq.Expressions;
using System.Reflection;
#if NET472
using Umbraco.Core.Models.PublishedContent;
using Umbraco.ModelsBuilder.Embedded;
#else
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.ModelsBuilder;
#endif

#if NET472
namespace Umbraco.Web
#else
namespace Umbraco.Extensions
#endif
{
    public static class PublishedElementExtensions
    {
        public static bool HasValueFor<TModel, TValue>(this TModel model, Expression<Func<TModel, TValue>> property, string culture = null, string segment = null)
            where TModel : IPublishedElement
        {
            var alias = GetAlias(model, property);
            return model.HasValue(alias, culture, segment);
        }

        // NOTE: Bah! `PublishedElementExtensions.GetAlias` is marked as private! It's either copy code, or reflection - here we go!
        // https://github.com/umbraco/Umbraco-CMS/blob/release-8.17.0/src/Umbraco.ModelsBuilder.Embedded/PublishedElementExtensions.cs#L28
        // https://github.com/umbraco/Umbraco-CMS/blob/release-9.0.0/src/Umbraco.Infrastructure/ModelsBuilder/PublishedElementExtensions.cs#L27
        private static string GetAlias<TModel, TValue>(TModel model, Expression<Func<TModel, TValue>> property)
        {
            try
            {
                var assembly = typeof(ApiVersion).Assembly;
#if NET472
                var type = assembly.GetType("Umbraco.Web.PublishedElementExtensions");
#else
                var type = assembly.GetType("Umbraco.Extensions.PublishedElementExtensions");
#endif
                var method = type.GetMethod(nameof(GetAlias), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
                var generic = method.MakeGenericMethod(typeof(TModel), typeof(TValue));
                return generic.Invoke(null, new object[] { model, property }) as string;
            }
            catch { /* ಠ_ಠ */ }

            return default;
        }
    }
}
