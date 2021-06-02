/* Copyright © 2019 Lee Kelleher.
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using Umbraco.Community.Contentment.Composing;
using Umbraco.Community.Contentment.DataEditors;

// NOTE: This extension method class is deliberately using the Umbraco namespace,
// as to reduce namespace imports and ease the developer experience. [LK]
namespace Umbraco.Core.Composing
{
    public static partial class CompositionExtensions
    {
        public static ContentmentListItemCollectionBuilder ContentmentListItems(this Composition composition)
        {
            return composition.WithCollectionBuilder<ContentmentListItemCollectionBuilder>();
        }

        public static Composition UnlockContentment(this Composition composition)
        {
            composition
                .WithCollectionBuilder<ContentmentListItemCollectionBuilder>()
                    // Data List - Data Sources
                    .Add<CountriesDataListSource>()
                    .Add<CurrenciesDataListSource>()
                    .Add<ExamineDataListSource>()
                    .Add<TimeZoneDataListSource>()
                    .Add<uCssClassNameDataListSource>()
                    .Add<UmbracoContentPropertiesDataListSource>()
                    .Add<UmbracoContentTypesDataListSource>()
                    .Add<UmbracoContentXPathDataListSource>()
                    .Add<UmbracoDictionaryDataListSource>()
                    .Add<UmbracoEntityDataListSource>()
                    .Add<UmbracoImageCropDataListSource>()
                    .Add<UmbracoMembersDataListSource>()
                    .Add<UmbracoMemberGroupDataListSource>()
            ;

            return composition;
        }
    }
}
