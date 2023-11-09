/* Copyright © 2021 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Linq.Expressions;
using System.Reflection;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Infrastructure.ModelsBuilder;

namespace Umbraco.Extensions
{
    public static class PublishedElementExtensions
    {
        private static readonly Dictionary<PublishedItemType, string> _entityTypeLookup = new Dictionary<PublishedItemType, string>
        {
            { PublishedItemType.Content, UmbConstants.UdiEntityType.Document },
            { PublishedItemType.Element, UmbConstants.UdiEntityType.Element },
            { PublishedItemType.Media, UmbConstants.UdiEntityType.Media },
            { PublishedItemType.Member, UmbConstants.UdiEntityType.Member },
            { PublishedItemType.Unknown, UmbConstants.UdiEntityType.Unknown },
        };

        public static Udi GetUdi<TModel>(this TModel model)
             where TModel : IPublishedElement
        {
            if (model is IPublishedContent content && _entityTypeLookup.TryGetValue(content.ItemType, out var entityType) == true)
            {
                return new GuidUdi(entityType, content.Key);
            }

            return new GuidUdi(UmbConstants.UdiEntityType.Element, model.Key);
        }

        // TODO: [LK] Raise bug with Umbraco. Noticed that `PublishedElementExtensions` and `PublishedContentExtensions` Value calls are different.
        // Unsure why, but the Content one fallback works, and the Element one does not.
        // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Core/Extensions/PublishedElementExtensions.cs#L166-L192
        // https://github.com/umbraco/Umbraco-CMS/blob/v10/contrib/src/Umbraco.Core/Extensions/PublishedContentExtensions.cs#L383-L409

        public static TValue ValueOrDefault<TModel, TValue>(this TModel model, string alias, string culture = null, string segment = null, TValue defaultValue = default)
            where TModel : IPublishedElement
        {
            return model.Value(alias, culture, segment, Fallback.ToDefaultValue, defaultValue) ?? defaultValue;
        }

        public static TValue ValueOrDefaultFor<TModel, TValue>(this TModel model, Expression<Func<TModel, TValue>> property, string culture = null, string segment = null, TValue defaultValue = default)
            where TModel : IPublishedElement
        {
            var alias = GetAlias(model, property);

            return model.Value(alias, culture, segment, Fallback.ToDefaultValue, defaultValue) ?? defaultValue;
        }

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
                var type = assembly.GetType("Umbraco.Extensions.PublishedElementExtensions");
                var method = type.GetMethod(nameof(GetAlias), BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
                var generic = method.MakeGenericMethod(typeof(TModel), typeof(TValue));
                return generic.Invoke(null, new object[] { model, property }) as string;
            }
            catch { /* ಠ_ಠ */ }

            return default;
        }
    }
}
